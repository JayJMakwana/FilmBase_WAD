namespace FilmBase.Models
{
    public class WatchlistItem
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string? Title { get; set; }
        public string? PosterPath { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}