// Data/FilmBaseContext.cs
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Plan to Watch" },
                new Category { Id = 2, Name = "Favorites" },
                new Category { Id = 3, Name = "Completed" }
            );
        }
    }
}