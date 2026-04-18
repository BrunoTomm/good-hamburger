using FluentValidation;
using GoodHamburger.Application.Auth.DTOs;
using GoodHamburger.Application.Common.Interfaces;
using GoodHamburger.Domain.Auth;
using GoodHamburger.Domain.Common;
using MediatR;

namespace GoodHamburger.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Email, string Senha) : IRequest<Result<AuthResponseDto>>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Senha).NotEmpty();
    }
}

public sealed class LoginCommandHandler(IUsuarioRepository repository, IJwtService jwtService)
    : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
    {
        var usuario = await repository.ObterPorEmailAsync(request.Email, ct);

        if (usuario is null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.PasswordHash))
            return Result.Failure<AuthResponseDto>("Credenciais inválidas.");

        var token = jwtService.GerarToken(usuario);
        return Result.Success(new AuthResponseDto(token, DateTime.UtcNow.AddHours(8)));
    }
}
