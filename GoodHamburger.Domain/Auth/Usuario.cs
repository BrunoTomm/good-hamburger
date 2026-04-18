using GoodHamburger.Domain.Common;

namespace GoodHamburger.Domain.Auth;

public sealed class Usuario : BaseEntity
{
    private Usuario() { }

    public static Usuario Criar(string email, string passwordHash) => new()
    {
        Email = email,
        PasswordHash = passwordHash
    };

    public required string Email { get; init; }
    public required string PasswordHash { get; init; }
}
