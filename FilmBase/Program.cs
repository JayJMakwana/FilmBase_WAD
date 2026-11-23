using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using FilmBase.Data;
using FilmBase.Repositories;
using FilmBase.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// 1. Register Firestore
var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];
var credPath = Path.Combine(builder.Environment.ContentRootPath, builder.Configuration["Firebase:CredentialsPath"] ?? "firebase-key.json");
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credPath);
builder.Services.AddSingleton(_ => FirestoreDb.Create(firebaseProjectId));

// 2. Register Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
    });

// 3. Register Services (Keeping SQLite temporarily until Part 2 migration)
builder.Services.AddDbContext<FilmBaseContext>(options =>
    options.UseSqlite("Data Source=filmbase_v2.db"));
builder.Services.AddHttpClient<IMovieApiService, TmdbService>();
builder.Services.AddHttpClient<FirebaseAuthService>();
builder.Services.AddScoped<FirestoreService>();
builder.Services.AddScoped<IWatchlistRepository, WatchlistRepository>();
builder.Services.AddMemoryCache();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Order matters: Authentication MUST come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Movies}/{action=Index}/{id?}");

app.Run();