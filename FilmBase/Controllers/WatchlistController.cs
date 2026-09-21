// Controllers/WatchlistController.cs
using Microsoft.AspNetCore.Mvc;
using FilmBase.Models;
using FilmBase.Repositories;
namespace FilmBase.Controllers
{
    public class WatchlistController : Controller
    {
        private readonly IWatchlistRepository _repository;

        public WatchlistController(IWatchlistRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _repository.GetAllCategoriesWithItemsAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWatchlist(WatchlistItem item)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddToWatchlistAsync(item);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Movies");
        }
    }
}