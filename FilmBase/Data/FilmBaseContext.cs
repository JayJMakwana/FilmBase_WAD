using FilmBase.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmBase.Data
{
    public class FilmBaseContext : DbContext
    {
        public FilmBaseContext(DbContextOptions<FilmBaseContext> options) : base(options) { }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<WatchlistItem> WatchlistItems { get; set; }
    }
}