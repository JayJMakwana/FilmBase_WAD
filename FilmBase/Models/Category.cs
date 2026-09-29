// Models/Category.cs
namespace FilmBase.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public List<WatchlistItem> WatchlistItems { get; set; } = new List<WatchlistItem>();
    }
}