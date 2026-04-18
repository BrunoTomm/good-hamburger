using FluentValidation;
using GoodHamburger.Application.Auth.DTOs;
using GoodHamburger.Application.Common.Interfaces;
using GoodHamburger.Domain.Auth;
using GoodHamburger.Domain.Common;
using MediatR;

namespace GoodHamburger.Application.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Senha) : IRequest<Result<AuthResponseDto>>;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.Senha).NotEmpty().MinimumLength(6);
    }
}

public sealed class RegisterCommandHandler(
    IUsuarioRepository repository,
    IUnitOfWork unitOfWork,
    IJwtService jwtService)
    : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken ct)
    {
        if (await repository.ExisteAsync(request.Email, ct))
            return Result.Failure<AuthResponseDto>("E-mail já cadastrado.");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
        var usuario = Usuario.Criar(request.Email, hash);

        await repository.AdicionarAsync(usuario, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var token = jwtService.GerarToken(usuario);
        return Result.Success(new AuthResponseDto(token, DateTime.UtcNow.AddHours(8)));
    }
}
