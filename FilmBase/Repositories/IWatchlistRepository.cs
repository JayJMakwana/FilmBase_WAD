// Repositories/IWatchlistRepository.cs
using FilmBase.Models;

namespace FilmBase.Repositories
{
    public interface IWatchlistRepository
    {
        Task<IEnumerable<Category>> GetAllCategoriesWithItemsAsync();
        Task AddToWatchlistAsync(WatchlistItem item);
    }
}