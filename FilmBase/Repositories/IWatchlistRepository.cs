using FilmBase.Models;

namespace FilmBase.Repositories
{
    public interface IWatchlistRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesWithItemsAsync();
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task AddToWatchlistFromTmdbAsync(int tmdbId, string title, string posterPath, int categoryId);
    }
}