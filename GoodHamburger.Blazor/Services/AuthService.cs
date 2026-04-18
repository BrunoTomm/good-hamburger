using System.Net.Http.Json;
using GoodHamburger.Blazor.Models;
using Microsoft.JSInterop;

namespace GoodHamburger.Blazor.Services;

public sealed class AuthService(HttpClient http, IJSRuntime js)
{
    private const string TokenKey = "auth_token";

    public async Task<bool> LoginAsync(string email, string senha)
    {
        var response = await http.PostAsJsonAsync("auth/login", new LoginRequest(email, senha));
        if (!response.IsSuccessStatusCode) return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, auth!.Token);
        http.DefaultRequestHeaders.Authorization = new("Bearer", auth.Token);
        return true;
    }

    public async Task<bool> RegisterAsync(string email, string senha)
    {
        var response = await http.PostAsJsonAsync("auth/register", new RegisterRequest(email, senha));
        if (!response.IsSuccessStatusCode) return false;

        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        await js.InvokeVoidAsync("localStorage.setItem", TokenKey, auth!.Token);
        http.DefaultRequestHeaders.Authorization = new("Bearer", auth.Token);
        return true;
    }

    public async Task LogoutAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        http.DefaultRequestHeaders.Authorization = null;
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
}
