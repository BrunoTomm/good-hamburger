using System.Net.Http.Json;
using GoodHamburger.Web.Models;
using Microsoft.JSInterop;

namespace GoodHamburger.Web.Services;

public sealed class AuthService(HttpClient http, IJSRuntime js)
{
    private const string TokenKey = "auth_token";

    public event Action? AuthChanged;

    public async Task<bool> LoginAsync(string email, string senha)
    {
        var response = await http.PostAsJsonAsync("auth/login", new LoginRequest(email, senha));
        if (!response.IsSuccessStatusCode) return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, auth!.Token);
        http.DefaultRequestHeaders.Authorization = new("Bearer", auth.Token);
        AuthChanged?.Invoke();
        return true;
    }

    public async Task<bool> RegisterAsync(string email, string senha)
    {
        var response = await http.PostAsJsonAsync("auth/register", new RegisterRequest(email, senha));
        if (!response.IsSuccessStatusCode) return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, auth!.Token);
        http.DefaultRequestHeaders.Authorization = new("Bearer", auth.Token);
        AuthChanged?.Invoke();
        return true;
    }

    public async Task LogoutAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        http.DefaultRequestHeaders.Authorization = null;
        AuthChanged?.Invoke();
    }

    public async Task InitializeAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        if (token is not null)
            http.DefaultRequestHeaders.Authorization = new("Bearer", token);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        return token is not null;
    }

    public async Task<string?> GetEmailAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        if (token is null) return null;
        var parts = token.Split('.');
        if (parts.Length != 3) return null;
        var payload = parts[1];
        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        using var doc = System.Text.Json.JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("email", out var email) ? email.GetString() : null;
    }
}