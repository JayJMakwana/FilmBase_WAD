using System.Text.Json;
using System.Text.Json.Serialization;
using FilmBase.Models.DTOs;

namespace FilmBase.Services
{
    // 1. Create a wrapper class to catch the "results" array from TMDB
    public class TmdbSearchResponse
    {
        [JsonPropertyName("results")]
        public List<TmdbMovieDto> Results { get; set; } = new List<TmdbMovieDto>();
    }

    public class TmdbService : IMovieApiService
    {
        private readonly HttpClient _httpClient;

        // Put your Cloudflare Worker URL here
        private readonly string _workerUrl = "https://filmbase-asp.jaym15993.workers.dev";

        public TmdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<TmdbMovieDto>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<TmdbMovieDto>();

            // 2. Call your Cloudflare worker
            var response = await _httpClient.GetAsync($"{_workerUrl}/search/movie?query={Uri.EscapeDataString(query)}");

            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            // 3. Deserialize into the wrapper, not directly into a list
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<TmdbSearchResponse>(jsonString, options);

            return data?.Results ?? new List<TmdbMovieDto>();
        }
        public async Task<IEnumerable<TmdbMovieDto>> GetTrendingMoviesAsync()
        {
            var response = await _httpClient.GetAsync($"{_workerUrl}/trending/movie/week");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbSearchResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data?.Results ?? new List<TmdbMovieDto>();
        }
        public async Task<IEnumerable<TmdbMovieDto>> GetTopRatedMoviesAsync()
        {
            var response = await _httpClient.GetAsync($"{_workerUrl}/movie/top_rated");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<TmdbSearchResponse>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return data?.Results ?? new List<TmdbMovieDto>();
        }
        public async Task<TmdbMovieDto> GetMovieDetailsAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_workerUrl}/movie/{id}?append_to_response=credits");
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();

            // Notice we deserialize directly into the DTO here, not the SearchResponse wrapper, 
            // because TMDB returns a single object for movie details, not an array.
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<TmdbMovieDto>(jsonString, options) ?? new TmdbMovieDto();
        }
    }
}