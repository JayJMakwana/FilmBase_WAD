// Services/IMovieApiService.cs
using FilmBase.Models.DTOs;

namespace FilmBase.Services
{
    public interface IMovieApiService
    {
        Task<IEnumerable<TmdbMovieDto>> SearchMoviesAsync(string query);
        Task<IEnumerable<TmdbMovieDto>> GetTrendingMoviesAsync();
        Task<IEnumerable<TmdbMovieDto>> GetTopRatedMoviesAsync();
        Task<TmdbMovieDto> GetMovieDetailsAsync(int id);
    }
}