using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FilmBase.Data;
using FilmBase.Models;
using FilmBase.Repositories;

namespace FilmBase.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly IWatchlistRepository _repository;
        private readonly FilmBaseContext _context;

        // Inject BOTH the repository and the database context
        public WatchlistController(IWatchlistRepository repository, FilmBaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _repository.GetAllCategoriesWithItemsAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchlist(int tmdbId, string title, string posterPath, int categoryId)
        {
            if (tmdbId > 0 && categoryId > 0)
            {
                await _repository.AddToWatchlistFromTmdbAsync(tmdbId, title, posterPath, categoryId);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Movies");
        }
        [HttpPost]
        public async Task<IActionResult> Toggle(int tmdbId, string title, string posterPath, int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.WatchlistItems)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category != null)
            {
                var existingItem = category.WatchlistItems.FirstOrDefault(i => i.TmdbId == tmdbId);
                if (existingItem != null)
                {
                    // Movie is in list, so remove it
                    _context.WatchlistItems.Remove(existingItem);
                }
                else
                {
                    // Movie is not in list, so add it
                    category.WatchlistItems.Add(new WatchlistItem
                    {
                        TmdbId = tmdbId,
                        Title = title,
                        PosterPath = posterPath,
                        CategoryId = categoryId
                    });
                }
                await _context.SaveChangesAsync();
            }

            // Redirect back to the exact movie details page you were just on
            return RedirectToAction("Details", "Movies", new { id = tmdbId });
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryFromDetails(string categoryName, int returnTmdbId)
        {
            if (!string.IsNullOrWhiteSpace(categoryName))
            {
                _context.Categories.Add(new Category { Name = categoryName });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Details", "Movies", new { id = returnTmdbId });
        }
    }
}