// Models/Movie.cs
namespace FilmBase.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterPath { get; set; } = string.Empty;
        public string ReleaseDate { get; set; } = string.Empty;
    }
}