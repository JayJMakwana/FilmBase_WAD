using Google.Cloud.Firestore;

namespace FilmBase.Models
{
    [FirestoreData]
    public class CloudMovie
    {
        [FirestoreProperty("tmdbId")]
        public int TmdbId { get; set; }

        [FirestoreProperty("title")]
        public string Title { get; set; } = string.Empty;

        [FirestoreProperty("posterPath")]
        public string PosterPath { get; set; } = string.Empty;

        [FirestoreProperty("mediaType")]
        public string MediaType { get; set; } = "movie";
    }

    [FirestoreData]
    public class CloudWatchlist
    {
        public string Id { get; set; } = string.Empty;

        [FirestoreProperty("name")]
        public string Name { get; set; } = string.Empty;

        [FirestoreProperty("items")]
        public List<CloudMovie> Items { get; set; } = new List<CloudMovie>();
    }

    public class UserProfile
    {
        public string Uid { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<string> Friends { get; set; } = new List<string>();
        public List<string> FriendRequests { get; set; } = new List<string>();
    }

    public class FriendsViewModel
    {
        public List<UserProfile> Friends { get; set; } = new List<UserProfile>();
        public List<UserProfile> IncomingRequests { get; set; } = new List<UserProfile>();
        public List<UserProfile> SearchResults { get; set; } = new List<UserProfile>();
        public string? SearchQuery { get; set; }
    }
}