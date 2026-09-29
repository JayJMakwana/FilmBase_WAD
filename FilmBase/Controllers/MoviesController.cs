using Microsoft.AspNetCore.Mvc;
using FilmBase.Services;
using FilmBase.Repositories;
using FilmBase.Models; // Required for HomePageViewModel

namespace FilmBase.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieApiService _movieService;
        private readonly IWatchlistRepository _watchlistRepository;

        public MoviesController(IMovieApiService movieService, IWatchlistRepository watchlistRepository)
        {
            _movieService = movieService;
            _watchlistRepository = watchlistRepository;
        }

        // 1. HOME PAGE: Loads the horizontal rows
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomePageViewModel
            {
                TrendingMovies = await _movieService.GetTrendingMoviesAsync(),
                TopRatedMovies = await _movieService.GetTopRatedMoviesAsync()
            };

            return View(viewModel);
        }

        // 2. SEARCH PAGE: Handles queries from the search bar
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<FilmBase.Models.DTOs.TmdbMovieDto>());
            }

            var movies = await _movieService.SearchMoviesAsync(query);
            return View(movies);
        }

        // 3. DETAILS PAGE: Shows plot and watchlist dropdown
        public async Task<IActionResult> Details(int id)
        {
            var movieDetails = await _movieService.GetMovieDetailsAsync(id);

            ViewBag.UserLists = await _watchlistRepository.GetAllCategoriesWithItemsAsync();

            return View(movieDetails);
        }
    }
}