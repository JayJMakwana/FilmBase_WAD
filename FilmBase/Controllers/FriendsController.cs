using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FilmBase.Models;
using FilmBase.Services;

namespace FilmBase.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly FirestoreService _firestore;

        public FriendsController(FirestoreService firestore)
        {
            _firestore = firestore;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index(string? search)
        {
            var me = await _firestore.GetUserProfileAsync(CurrentUserId);
            var vm = new FriendsViewModel { SearchQuery = search };

            if (me != null)
            {
                foreach (var friendUid in me.Friends)
                {
                    var profile = await _firestore.GetUserProfileAsync(friendUid);
                    if (profile != null) vm.Friends.Add(profile);
                }

                foreach (var reqUid in me.FriendRequests)
                {
                    var profile = await _firestore.GetUserProfileAsync(reqUid);
                    if (profile != null) vm.IncomingRequests.Add(profile);
                }
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                vm.SearchResults = await _firestore.SearchUsersAsync(search, CurrentUserId);
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> SendRequest(string targetUid)
        {
            await _firestore.SendFriendRequestAsync(CurrentUserId, targetUid);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRequest(string requesterUid)
        {
            await _firestore.AcceptFriendRequestAsync(CurrentUserId, requesterUid);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeclineRequest(string requesterUid)
        {
            await _firestore.DeclineFriendRequestAsync(CurrentUserId, requesterUid);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string friendUid)
        {
            await _firestore.RemoveFriendAsync(CurrentUserId, friendUid);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Watchlists(string friendUid)
        {
            var me = await _firestore.GetUserProfileAsync(CurrentUserId);
            if (me == null || !me.Friends.Contains(friendUid))
            {
                return RedirectToAction("Index");
            }

            var friendProfile = await _firestore.GetUserProfileAsync(friendUid);
            var friendLists = await _firestore.GetUserWatchlistsAsync(friendUid);

            ViewData["FriendUid"] = friendUid;
            ViewData["FriendName"] = friendProfile?.Username ?? "Friend";
            return View(friendLists);
        }

        public async Task<IActionResult> ListDetails(string friendUid, string listId)
        {
            var me = await _firestore.GetUserProfileAsync(CurrentUserId);
            if (me == null || !me.Friends.Contains(friendUid) || string.IsNullOrEmpty(listId))
            {
                return RedirectToAction("Index");
            }

            var friendProfile = await _firestore.GetUserProfileAsync(friendUid);
            var friendLists = await _firestore.GetUserWatchlistsAsync(friendUid);
            var targetList = friendLists.FirstOrDefault(l => l.Id == listId);

            if (targetList == null)
            {
                return RedirectToAction("Watchlists", new { friendUid });
            }

            ViewData["FriendUid"] = friendUid;
            ViewData["FriendName"] = friendProfile?.Username ?? "Friend";
            return View(targetList);
        }
    }
}