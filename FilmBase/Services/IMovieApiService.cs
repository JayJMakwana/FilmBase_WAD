// Services/IMovieApiService.cs
using FilmBase.Models.DTOs;

namespace FilmBase.Services
{
    public interface IMovieApiService
    {
        Task<IEnumerable<OmdbMovieDto>> SearchMoviesAsync(string query);
        Task<OmdbMovieDto?> GetMovieByIdAsync(string imdbId);
    }
}

namespace FilmBase.Models.DTOs
{
    public class OmdbMovieDto
    {
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string imdbID { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
    }
}