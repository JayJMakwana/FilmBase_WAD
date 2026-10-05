// Services/IMovieApiService.cs
using FilmBase.Models.DTOs;

namespace FilmBase.Services
{
    public interface IMovieApiService
    {
        Task<TmdbMovieDto> GetMovieDetailsAsync(int id);
        Task<IEnumerable<TmdbMovieDto>> SearchMoviesAsync(string query, int page = 1);
        Task<IEnumerable<TmdbMovieDto>> DiscoverMoviesAsync(string filterType, string filterValue, int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetTrendingMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetTopRatedMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetNewReleasedMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetOscarsWinningMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetEnglishMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetHindiMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetAnimationMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetHorrorMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetSciFiMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetGujaratiMoviesAsync(int page = 1);
        Task<IEnumerable<TmdbMovieDto>> GetSouthIndianMoviesAsync(int page = 1);
    }
}