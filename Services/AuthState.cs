using Microsoft.JSInterop;
using System.Text.Json;

namespace Weatherfrontend.Services
{
    public class AuthState
    {
        private readonly IJSRuntime js;
        private const string StorageKey = "auth_state";

        public bool IsLoggedIn { get; private set; }
        public string? Email { get; private set; }
        public string? UserId { get; private set; }
        public string? Token { get; private set; }

        public event Action? OnChange;

        public AuthState(IJSRuntime js)
        {
            this.js = js;
        }

        public async Task LoadStateAsync()
        {
            var json = await js.InvokeAsync<string>("localStorage.getItem", StorageKey);

            if (string.IsNullOrWhiteSpace(json))
                return;

            var state = JsonSerializer.Deserialize<AuthModel>(json);
            if (state == null)
                return;

            IsLoggedIn = state.IsLoggedIn;
            Email = state.Email;
            UserId = state.UserId;
            Token = state.Token;

            Notify();
        }

        public async Task LoginAsync(string userId, string email, string token)
        {
            IsLoggedIn = true;
            Email = email;
            UserId = userId;
            Token = token;

            var json = JsonSerializer.Serialize(new AuthModel
            {
                IsLoggedIn = true,
                Email = email,
                UserId = userId,
                Token = token
            });

            await js.InvokeVoidAsync("localStorage.setItem", StorageKey, json);

            Notify();
        }

        public async Task LogoutAsync()
        {
            await js.InvokeVoidAsync("localStorage.removeItem", StorageKey);

            IsLoggedIn = false;
            Email = null;
            UserId = null;
            Token = null;

            Notify();
        }

        private void Notify() => OnChange?.Invoke();

        private class AuthModel
        {
            public bool IsLoggedIn { get; set; }
            public string? Email { get; set; }
            public string? UserId { get; set; }
            public string? Token { get; set; }
        }
    }
}
