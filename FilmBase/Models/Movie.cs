// Models/Movie.cs
namespace FilmBase.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string ImdbId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public List<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
    }
}