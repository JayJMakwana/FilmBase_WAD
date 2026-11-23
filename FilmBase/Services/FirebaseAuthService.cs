using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Cloud.Firestore;

namespace FilmBase.Services
{
    public class FirebaseAuthResponse
    {
        [JsonPropertyName("localId")]
        public string Uid { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("idToken")]
        public string IdToken { get; set; } = string.Empty;
    }

    public class FirebaseAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly FirestoreDb _firestore;
        private readonly string _apiKey;

        public FirebaseAuthService(HttpClient httpClient, FirestoreDb firestore, IConfiguration config)
        {
            _httpClient = httpClient;
            _firestore = firestore;
            _apiKey = config["Firebase:ApiKey"] ?? throw new Exception("Firebase ApiKey missing.");
        }

        public async Task<(bool Success, string? Uid, string? Error)> RegisterAsync(string email, string password, string username)
        {
            var payload = new { email, password, returnSecureToken = true };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={_apiKey}", content);

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return (false, null, ExtractError(json));
            }

            var authData = JsonSerializer.Deserialize<FirebaseAuthResponse>(json);
            if (authData == null) return (false, null, "Registration failed.");

            // Create user profile document in Firestore
            var userDoc = _firestore.Collection("users").Document(authData.Uid);
            await userDoc.SetAsync(new Dictionary<string, object>
            {
                { "uid", authData.Uid },
                { "email", email.ToLower() },
                { "username", username },
                { "friends", new List<string>() },
                { "friendRequests", new List<string>() },
                { "createdAt", Timestamp.GetCurrentTimestamp() }
            });

            return (true, authData.Uid, null);
        }

        public async Task<(bool Success, string? Uid, string? Username, string? Error)> LoginAsync(string email, string password)
        {
            var payload = new { email, password, returnSecureToken = true };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={_apiKey}", content);

            var json = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return (false, null, null, ExtractError(json));
            }

            var authData = JsonSerializer.Deserialize<FirebaseAuthResponse>(json);
            if (authData == null) return (false, null, null, "Login failed.");

            // Fetch username from Firestore
            var snapshot = await _firestore.Collection("users").Document(authData.Uid).GetSnapshotAsync();
            string username = snapshot.Exists && snapshot.TryGetValue("username", out string name) ? name : email.Split('@')[0];

            return (true, authData.Uid, username, null);
        }

        private string ExtractError(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return doc.RootElement.GetProperty("error").GetProperty("message").GetString() ?? "Authentication error.";
            }
            catch
            {
                return "Authentication error.";
            }
        }
    }
}