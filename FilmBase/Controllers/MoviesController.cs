using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using FilmBase.Services;
using FilmBase.Models;
using System.Security.Claims;

namespace FilmBase.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieApiService _movieService;
        private readonly FirestoreService _firestore;
        private readonly IMemoryCache _cache;

        public MoviesController(IMovieApiService movieService, FirestoreService firestore, IMemoryCache cache)
        {
            _movieService = movieService;
            _firestore = firestore;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            if (!_cache.TryGetValue("HomePageData", out HomePageViewModel? viewModel) || viewModel == null)
            {
                // Fire all 11 API calls simultaneously
                var trendingTask = _movieService.GetTrendingMoviesAsync();
                var seriesTask = _movieService.GetPopularSeriesAsync();
                var topRatedTask = _movieService.GetTopRatedMoviesAsync();
                var newReleasesTask = _movieService.GetNewReleasedMoviesAsync();
                var oscarsTask = _movieService.GetOscarsWinningMoviesAsync();
                var englishTask = _movieService.GetEnglishMoviesAsync();
                var hindiTask = _movieService.GetHindiMoviesAsync();
                var animationTask = _movieService.GetAnimationMoviesAsync();
                var horrorTask = _movieService.GetHorrorMoviesAsync();
                var scifiTask = _movieService.GetSciFiMoviesAsync();
                var gujaratiTask = _movieService.GetGujaratiMoviesAsync();

                // Wait for all of them to finish together
                await Task.WhenAll(
                    trendingTask, seriesTask, topRatedTask, newReleasesTask,
                    oscarsTask, englishTask, hindiTask, animationTask,
                    horrorTask, scifiTask, gujaratiTask
                );

                viewModel = new HomePageViewModel
                {
                    TrendingMovies = await trendingTask,
                    PopularSeries = await seriesTask,
                    TopRatedMovies = await topRatedTask,
                    NewReleasedMovies = await newReleasesTask,
                    OscarsWinningMovies = await oscarsTask,
                    EnglishMovies = await englishTask,
                    HindiMovies = await hindiTask,
                    AnimationMovies = await animationTask,
                    HorrorMovies = await horrorTask,
                    SciFiMovies = await scifiTask,
                    GujaratiMovies = await gujaratiTask
                };

                // Cache the home page data for 30 minutes
                _cache.Set("HomePageData", viewModel, TimeSpan.FromMinutes(30));
            }

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

        public async Task<IActionResult> Details(int id, string mediaType = "movie")
        {
            var movieDetails = await _movieService.GetMovieDetailsAsync(id, mediaType);

            if (User.Identity?.IsAuthenticated == true)
            {
                var uid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                ViewBag.UserLists = await _firestore.GetUserWatchlistsAsync(uid);
            }
            else
            {
                ViewBag.UserLists = new List<CloudWatchlist>();
            }

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