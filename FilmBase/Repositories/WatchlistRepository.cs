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
            return await _context.Categories
                .Include(c => c.WatchlistItems)
                .ToListAsync();
        }

        public async Task AddToWatchlistFromTmdbAsync(int tmdbId, string title, string posterPath, int categoryId)
        {
            // 1. Check if the movie already exists locally
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.TmdbId == tmdbId);

            // 2. If not, add it to the database
            if (movie == null)
            {
                movie = new Movie
                {
                    TmdbId = tmdbId,
                    Title = title ?? "Unknown",
                    PosterPath = posterPath ?? string.Empty
                };
                _context.Movies.Add(movie);
                await _context.SaveChangesAsync(); // Saves so the movie gets an Id
            }

            // 3. Create the watchlist link
            var watchlistItem = new WatchlistItem
            {
                TmdbId = movie.Id,
                CategoryId = categoryId
            };

            _context.WatchlistItems.Add(watchlistItem);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}