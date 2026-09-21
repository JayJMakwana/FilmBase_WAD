// Services/OmdbService.cs
using System.Text.Json;
using FilmBase.Models.DTOs;

namespace FilmBase.Services
{
    public class OmdbService : IMovieApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "3adcf164";
        private readonly string _baseUrl = "https://www.omdbapi.com/";

        public OmdbService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<OmdbMovieDto>> SearchMoviesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return new List<OmdbMovieDto>();

            var response = await _httpClient.GetAsync($"{_baseUrl}?s={query}&apikey={_apiKey}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var searchResult = JsonSerializer.Deserialize<OmdbSearchResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return searchResult?.Search ?? new List<OmdbMovieDto>();
        }

        public async Task<OmdbMovieDto?> GetMovieByIdAsync(string imdbId)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}?i={imdbId}&apikey={_apiKey}");
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<OmdbMovieDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }

    public class OmdbSearchResponse
    {
        public List<OmdbMovieDto>? Search { get; set; }
    }
}