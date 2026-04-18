using FluentValidation;
using GoodHamburger.Application.Common.Interfaces;
using GoodHamburger.Application.Pedidos.DTOs;
using GoodHamburger.Domain.Common;
using GoodHamburger.Domain.Pedidos;
using MediatR;
using static GoodHamburger.Domain.Cardapio.Cardapio;

namespace GoodHamburger.Application.Pedidos.Commands.CriarPedido;

public sealed record CriarPedidoCommand(Guid UsuarioId, List<string> NomesItens) : IRequest<Result<PedidoDto>>;

public sealed class CriarPedidoCommandValidator : AbstractValidator<CriarPedidoCommand>
{
    public CriarPedidoCommandValidator()
    {
        RuleFor(c => c.NomesItens).NotEmpty().WithMessage("O pedido deve ter ao menos um item.");
        RuleFor(c => c.NomesItens).Must(itens => itens.Any(n =>
            Itens.Any(c => string.Equals(c.Nome, n, StringComparison.OrdinalIgnoreCase) && c.Tipo == TipoItem.Sanduiche)))
            .WithMessage("O pedido deve conter ao menos um sanduíche.");
    }
}

public sealed class CriarPedidoCommandHandler(IPedidoRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<CriarPedidoCommand, Result<PedidoDto>>
{
    public async Task<Result<PedidoDto>> Handle(CriarPedidoCommand request, CancellationToken ct)
    {
        var pedido = Pedido.Criar(request.UsuarioId).Value;

        foreach (var nome in request.NomesItens)
        {
            var itemCardapio = Encontrar(nome);
            if (itemCardapio is null)
                return Result.Failure<PedidoDto>($"Item '{nome}' não encontrado no cardápio.");

            var resultado = pedido.AdicionarItem(new ItemPedido(itemCardapio.Tipo, itemCardapio.Nome, itemCardapio.Preco));
            if (resultado.IsFailure)
                return Result.Failure<PedidoDto>(resultado.Error!);
        }

        await repository.AdicionarAsync(pedido, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new PedidoDto(
            pedido.Id,
            pedido.Itens.Select(i => new ItemPedidoDto(i.Tipo.ToString(), i.Nome, i.Preco)).ToList(),
            pedido.Subtotal,
            pedido.PercentualDesconto,
            pedido.Desconto,
            pedido.Total,
            pedido.CriadoEm,
            pedido.AtualizadoEm));
    }
}
