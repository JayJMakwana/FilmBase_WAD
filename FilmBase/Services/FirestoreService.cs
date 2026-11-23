using Google.Cloud.Firestore;
using FilmBase.Models;

namespace FilmBase.Services
{
    public class FirestoreService
    {
        private readonly FirestoreDb _db;

        public FirestoreService(FirestoreDb db)
        {
            _db = db;
        }

        // ==================== WATCHLISTS ====================

        public async Task<List<CloudWatchlist>> GetUserWatchlistsAsync(string uid)
        {
            var snapshot = await _db.Collection("users").Document(uid).Collection("watchlists").GetSnapshotAsync();
            var lists = new List<CloudWatchlist>();

            foreach (var doc in snapshot.Documents)
            {
                var list = doc.ConvertTo<CloudWatchlist>();
                list.Id = doc.Id;
                lists.Add(list);
            }

            return lists.OrderBy(l => l.Name).ToList();
        }

        public async Task CreateWatchlistAsync(string uid, string listName)
        {
            var newList = new CloudWatchlist
            {
                Name = listName.Trim(),
                Items = new List<CloudMovie>()
            };

            await _db.Collection("users").Document(uid).Collection("watchlists").AddAsync(newList);
        }

        public async Task DeleteWatchlistAsync(string uid, string listId)
        {
            await _db.Collection("users").Document(uid).Collection("watchlists").Document(listId).DeleteAsync();
        }

        public async Task ToggleMovieInListAsync(string uid, string listId, int tmdbId, string title, string posterPath, string mediaType = "movie")
        {
            var docRef = _db.Collection("users").Document(uid).Collection("watchlists").Document(listId);
            var snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return;

            var type = mediaType?.ToLower() == "tv" ? "tv" : "movie";
            var list = snapshot.ConvertTo<CloudWatchlist>();
            var existing = list.Items.FirstOrDefault(m => m.TmdbId == tmdbId && m.MediaType == type);

            if (existing != null)
            {
                list.Items.Remove(existing);
            }
            else
            {
                list.Items.Add(new CloudMovie
                {
                    TmdbId = tmdbId,
                    Title = title ?? "Unknown",
                    PosterPath = posterPath ?? string.Empty,
                    MediaType = type
                });
            }

            await docRef.UpdateAsync("items", list.Items);
        }

        // ==================== FRIENDS ====================

        public async Task<UserProfile?> GetUserProfileAsync(string uid)
        {
            var doc = await _db.Collection("users").Document(uid).GetSnapshotAsync();
            if (!doc.Exists) return null;

            return new UserProfile
            {
                Uid = doc.Id,
                Username = doc.TryGetValue("username", out string name) ? name : "User",
                Email = doc.TryGetValue("email", out string email) ? email : "",
                Friends = doc.TryGetValue("friends", out List<string> friends) ? friends : new List<string>(),
                FriendRequests = doc.TryGetValue("friendRequests", out List<string> reqs) ? reqs : new List<string>()
            };
        }

        public async Task<List<UserProfile>> SearchUsersAsync(string query, string currentUid)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<UserProfile>();

            var q = query.Trim().ToLower();
            var snapshot = await _db.Collection("users").GetSnapshotAsync();
            var results = new List<UserProfile>();

            foreach (var doc in snapshot.Documents)
            {
                if (doc.Id == currentUid) continue;

                var username = doc.TryGetValue("username", out string name) ? name : "";
                var email = doc.TryGetValue("email", out string em) ? em : "";

                if (username.ToLower().Contains(q) || email.ToLower().Contains(q))
                {
                    results.Add(new UserProfile
                    {
                        Uid = doc.Id,
                        Username = username,
                        Email = email,
                        Friends = doc.TryGetValue("friends", out List<string> f) ? f : new List<string>(),
                        FriendRequests = doc.TryGetValue("friendRequests", out List<string> r) ? r : new List<string>()
                    });
                }
            }

            return results;
        }

        public async Task SendFriendRequestAsync(string fromUid, string toUid)
        {
            var targetRef = _db.Collection("users").Document(toUid);
            await targetRef.UpdateAsync("friendRequests", FieldValue.ArrayUnion(fromUid));
        }

        public async Task AcceptFriendRequestAsync(string currentUid, string requesterUid)
        {
            var currentRef = _db.Collection("users").Document(currentUid);
            var requesterRef = _db.Collection("users").Document(requesterUid);

            await currentRef.UpdateAsync(new Dictionary<string, object>
            {
                { "friendRequests", FieldValue.ArrayRemove(requesterUid) },
                { "friends", FieldValue.ArrayUnion(requesterUid) }
            });

            await requesterRef.UpdateAsync("friends", FieldValue.ArrayUnion(currentUid));
        }

        public async Task DeclineFriendRequestAsync(string currentUid, string requesterUid)
        {
            var currentRef = _db.Collection("users").Document(currentUid);
            await currentRef.UpdateAsync("friendRequests", FieldValue.ArrayRemove(requesterUid));
        }

        public async Task RemoveFriendAsync(string currentUid, string friendUid)
        {
            await _db.Collection("users").Document(currentUid).UpdateAsync("friends", FieldValue.ArrayRemove(friendUid));
            await _db.Collection("users").Document(friendUid).UpdateAsync("friends", FieldValue.ArrayRemove(currentUid));
        }
    }
}