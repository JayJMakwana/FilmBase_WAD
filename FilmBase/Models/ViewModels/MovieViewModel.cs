// Models/ViewModels/MovieViewModel.cs
namespace FilmBase.Models.ViewModels
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Overview { get; set; } = string.Empty;
        public string PosterUrl { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new List<string>();
    }
}