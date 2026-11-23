using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FilmBase.Services;

namespace FilmBase.Controllers
{
    [Authorize]
    public class WatchlistController : Controller
    {
        private readonly FirestoreService _firestore;

        public WatchlistController(FirestoreService firestore)
        {
            _firestore = firestore;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index()
        {
            var lists = await _firestore.GetUserWatchlistsAsync(CurrentUserId);
            return View(lists);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("Index");

            var lists = await _firestore.GetUserWatchlistsAsync(CurrentUserId);
            var list = lists.FirstOrDefault(l => l.Id == id);

            if (list == null)
                return RedirectToAction("Index");

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int tmdbId, string title, string posterPath, string categoryId, string mediaType = "movie", string? returnTo = null)
        {
            if (!string.IsNullOrEmpty(categoryId) && tmdbId > 0)
            {
                await _firestore.ToggleMovieInListAsync(CurrentUserId, categoryId, tmdbId, title, posterPath, mediaType);
            }

            if (returnTo == "list")
                return RedirectToAction("Details", new { id = categoryId });

            if (returnTo == "watchlist")
                return RedirectToAction("Index");

            return RedirectToAction("Details", "Movies", new { id = tmdbId, mediaType });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryFromDetails(string categoryName, int returnTmdbId, string returnMediaType = "movie")
        {
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                await _firestore.CreateWatchlistAsync(CurrentUserId, categoryName);
            }

            if (returnTmdbId > 0)
                return RedirectToAction("Details", "Movies", new { id = returnTmdbId, mediaType = returnMediaType });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteList(string categoryId)
        {
            if (!string.IsNullOrEmpty(categoryId))
            {
                await _firestore.DeleteWatchlistAsync(CurrentUserId, categoryId);
            }

            return RedirectToAction("Index");
        }
    }
}