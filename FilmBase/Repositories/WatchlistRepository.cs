// Repositories/WatchlistRepository.cs
using Microsoft.EntityFrameworkCore;
using FilmBase.Data;
using FilmBase.Models;

namespace FilmBase.Repositories
{
    public class WatchlistRepository : IWatchlistRepository
    {
        private readonly FilmBaseContext _context;

        public WatchlistRepository(FilmBaseContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesWithItemsAsync()
        {
            // Eagerly load the WatchlistItems and their associated Movies
            return await _context.Categories
                .Include(c => c.WatchlistItems)
                    .ThenInclude(w => w.Movie)
                .ToListAsync();
        }

        public async Task AddToWatchlistAsync(WatchlistItem item)
        {
            _context.WatchlistItems.Add(item);
            await _context.SaveChangesAsync();
        }
    }
}