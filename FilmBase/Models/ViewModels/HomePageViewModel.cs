using FilmBase.Models.DTOs;

namespace FilmBase.Models
{
    public class HomePageViewModel
    {
        public IEnumerable<TmdbMovieDto> TrendingMovies { get; set; } = new List<TmdbMovieDto>();
        public IEnumerable<TmdbMovieDto> TopRatedMovies { get; set; } = new List<TmdbMovieDto>();
        public IEnumerable<TmdbMovieDto> NewReleasedMovies { get; set; } = new List<TmdbMovieDto>();
        public IEnumerable<TmdbMovieDto> OscarsWinningMovies { get; set; } = new List<TmdbMovieDto>();
        public IEnumerable<TmdbMovieDto> EnglishMovies { get; set; } = new List<TmdbMovieDto>();
        public IEnumerable<TmdbMovieDto> HindiMovies { get; set; } = new List<TmdbMovieDto>();
    }
}