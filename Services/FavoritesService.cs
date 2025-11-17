using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Weatherfrontend.Services
{
    public class FavoritesService
    {
        private readonly HttpClient _http;
        private readonly AuthState _authState;
        private readonly string _baseUrl = "https://weatherbackendapi-fbceb0brdmdqgmca.southindia-01.azurewebsites.net/api/favorites";

        public FavoritesService(HttpClient http, AuthState authState)
        {
            _http = http;
            _authState = authState;
        }

        // GET /api/favorites/get

        public async Task<List<string>> GetFavoritesAsync()
        {
            try
            {
                var token = _authState.Token;
                if (string.IsNullOrEmpty(token))
                    throw new System.Exception("Missing JWT token. Please log in again.");

                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/get");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _http.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadFromJsonAsync<FavoritesResponse>();
                    return json?.Favorites ?? new List<string>();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new System.Exception($"Failed to fetch favorites: {error}");
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"❌ GetFavoritesAsync error: {ex.Message}");
                return new List<string>();
            }
        }

        // POST /api/favorites/add
        public async Task<bool> AddFavoriteAsync(string city)
        {
            try
            {
                var token = _authState.Token;
                if (string.IsNullOrEmpty(token))
                    throw new System.Exception("Missing JWT token. Please log in again.");

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/add");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = JsonContent.Create(new { City = city });

                var response = await _http.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"❌ AddFavoriteAsync error: {ex.Message}");
                return false;
            }
        }

        // DELETE /api/favorites/remove
        public async Task<bool> RemoveFavoriteAsync(string city)
        {
            try
            {
                var token = _authState.Token;
                if (string.IsNullOrEmpty(token))
                    throw new System.Exception("Missing JWT token. Please log in again.");

                var request = new HttpRequestMessage(HttpMethod.Delete, $"{_baseUrl}/remove");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = JsonContent.Create(new { City = city });

                var response = await _http.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"❌ RemoveFavoriteAsync error: {ex.Message}");
                return false;
            }
        }

        public class FavoritesResponse
        {
            public string Message { get; set; }
            public List<string> Favorites { get; set; }
        }
    }
}
