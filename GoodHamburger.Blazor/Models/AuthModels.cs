namespace GoodHamburger.Blazor.Models;

public record LoginRequest(string Email, string Senha);
public record RegisterRequest(string Email, string Senha);
public record AuthResponse(string Token, DateTime ExpiraEm);
