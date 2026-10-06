using Microsoft.AspNetCore.Mvc;
using FilmBase.Services;
using FilmBase.Repositories;
using FilmBase.Models;

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

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomePageViewModel
            {
                TrendingMovies = await _movieService.GetTrendingMoviesAsync(),
                PopularSeries = await _movieService.GetPopularSeriesAsync(),
                TopRatedMovies = await _movieService.GetTopRatedMoviesAsync(),
                NewReleasedMovies = await _movieService.GetNewReleasedMoviesAsync(),
                OscarsWinningMovies = await _movieService.GetOscarsWinningMoviesAsync(),
                EnglishMovies = await _movieService.GetEnglishMoviesAsync(),
                HindiMovies = await _movieService.GetHindiMoviesAsync(),
                AnimationMovies = await _movieService.GetAnimationMoviesAsync(),
                HorrorMovies = await _movieService.GetHorrorMoviesAsync(),
                SciFiMovies = await _movieService.GetSciFiMoviesAsync(),
                GujaratiMovies = await _movieService.GetGujaratiMoviesAsync()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View(new List<FilmBase.Models.DTOs.TmdbMovieDto>());
            }

            var movies = await _movieService.SearchMoviesAsync(query);
            return View(movies);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movieDetails = await _movieService.GetMovieDetailsAsync(id);
            ViewBag.UserLists = await _watchlistRepository.GetAllCategoriesWithItemsAsync();
            return View(movieDetails);
        }

        public async Task<IActionResult> Discover(string query, string filterType, string filterValue, string filterName, int page = 1, bool ajax = false)
        {
            ViewData["CurrentQuery"] = query;
            ViewData["FilterName"] = filterName;
            ViewData["FilterType"] = filterType;
            ViewData["FilterValue"] = filterValue;

            IEnumerable<FilmBase.Models.DTOs.TmdbMovieDto> movies = new List<FilmBase.Models.DTOs.TmdbMovieDto>();

            if (!string.IsNullOrEmpty(query))
            {
                movies = await _movieService.SearchMoviesAsync(query, page);
            }
            else if (!string.IsNullOrEmpty(filterType) && !string.IsNullOrEmpty(filterValue))
            {
                movies = await _movieService.DiscoverMoviesAsync(filterType, filterValue, page);
            }

            if (ajax)
            {
                return PartialView("_MovieCards", movies);
            }

            return View(movies);
        }

        public async Task<IActionResult> Browse(string category, int page = 1, bool ajax = false)
        {
            IEnumerable<FilmBase.Models.DTOs.TmdbMovieDto> movies = new List<FilmBase.Models.DTOs.TmdbMovieDto>();
            string viewTitle = "Browse Collection";

            switch (category?.ToLower())
            {
                case "newreleases":
                    movies = await _movieService.GetNewReleasedMoviesAsync(page);
                    viewTitle = "New Releases";
                    break;
                case "trending":
                    movies = await _movieService.GetTrendingMoviesAsync(page);
                    viewTitle = "Trending This Week";
                    break;
                case "series":
                    movies = await _movieService.GetPopularSeriesAsync(page);
                    viewTitle = "Popular TV Series";
                    break;
                case "toprated":
                    movies = await _movieService.GetTopRatedMoviesAsync(page);
                    viewTitle = "Top Rated of All Time";
                    break;
                case "oscars":
                    movies = await _movieService.GetOscarsWinningMoviesAsync(page);
                    viewTitle = "Critically Acclaimed";
                    break;
                case "hindi":
                    movies = await _movieService.GetHindiMoviesAsync(page);
                    viewTitle = "Popular Hindi Movies";
                    break;
                case "english":
                    movies = await _movieService.GetEnglishMoviesAsync(page);
                    viewTitle = "Popular English Movies";
                    break;
                case "animation":
                    movies = await _movieService.GetAnimationMoviesAsync(page);
                    viewTitle = "Animation Movies";
                    break;
                case "horror":
                    movies = await _movieService.GetHorrorMoviesAsync(page);
                    viewTitle = "Horror Movies";
                    break;
                case "scifi":
                    movies = await _movieService.GetSciFiMoviesAsync(page);
                    viewTitle = "Sci-Fi Movies";
                    break;
                case "gujarati":
                    movies = await _movieService.GetGujaratiMoviesAsync(page);
                    viewTitle = "Gujarati Cinema";
                    break;
            }

            if (ajax)
            {
                return PartialView("_MovieCards", movies);
            }

            ViewData["CategorySlug"] = category;
            ViewData["CategoryTitle"] = viewTitle;
            return View(movies);
        }
    }
}