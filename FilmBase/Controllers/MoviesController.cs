// Controllers/MoviesController.cs
using FilmBase.Models.ViewModels;
using FilmBase.Services;
using Microsoft.AspNetCore.Mvc;

namespace FilmBase.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieApiService _movieApi;

        public MoviesController(IMovieApiService movieApi)
        {
            _movieApi = movieApi;
        }

        public async Task<IActionResult> Index(string searchQuery = "")
        {
            var movies = await _movieApi.SearchMoviesAsync(searchQuery);
            return View(movies);
        }

        public async Task<IActionResult> Details(string id) 
        {
            var movie = await _movieApi.GetMovieByIdAsync(id);

            if (movie == null) return NotFound();

            return View(movie);
        }
    }
}