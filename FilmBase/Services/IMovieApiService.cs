// Services/IMovieApiService.cs
using FilmBase.Models.DTOs;

namespace FilmBase.Services
{
    public interface IMovieApiService
    {
        Task<IEnumerable<TmdbMovieDto>> GetTrendingMoviesAsync();
        Task<IEnumerable<TmdbMovieDto>> GetTopRatedMoviesAsync();
        Task<TmdbMovieDto> GetMovieDetailsAsync(int id);

        Task<IEnumerable<TmdbMovieDto>> SearchMoviesAsync(string query, int page = 1);
        Task<IEnumerable<TmdbMovieDto>> DiscoverMoviesAsync(string filterType, string filterValue, int page = 1);
    }
}