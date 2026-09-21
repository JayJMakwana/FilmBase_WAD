using Microsoft.EntityFrameworkCore;
using FilmBase.Data;
using FilmBase.Repositories;
using FilmBase.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 1. Register the SQLite Database Context
builder.Services.AddDbContext<FilmBaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Register the OMDb API Service with HttpClient
builder.Services.AddHttpClient<IMovieApiService, OmdbService>();

// 3. Register the Watchlist Repository
builder.Services.AddScoped<IWatchlistRepository, WatchlistRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Set the default route to open the MoviesController immediately
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();