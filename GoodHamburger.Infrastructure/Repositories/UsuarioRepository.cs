using GoodHamburger.Domain.Auth;
using GoodHamburger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public sealed class UsuarioRepository(AppDbContext context) : IUsuarioRepository
{
    public async Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken ct = default) =>
        await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<bool> ExisteAsync(string email, CancellationToken ct = default) =>
        await context.Usuarios.AnyAsync(u => u.Email == email, ct);

    public async Task AdicionarAsync(Usuario usuario, CancellationToken ct = default) =>
        await context.Usuarios.AddAsync(usuario, ct);
}
