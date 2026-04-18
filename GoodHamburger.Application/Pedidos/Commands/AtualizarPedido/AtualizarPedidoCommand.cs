using FluentValidation;
using GoodHamburger.Application.Common.Interfaces;
using GoodHamburger.Application.Pedidos.DTOs;
using GoodHamburger.Domain.Common;
using GoodHamburger.Domain.Pedidos;
using MediatR;
using static GoodHamburger.Domain.Cardapio.Cardapio;

namespace GoodHamburger.Application.Pedidos.Commands.AtualizarPedido;

public sealed record AtualizarPedidoCommand(Guid Id, Guid UsuarioId, List<string> NomesItens) : IRequest<Result<PedidoDto>>;

public sealed class AtualizarPedidoCommandValidator : AbstractValidator<AtualizarPedidoCommand>
{
    public AtualizarPedidoCommandValidator()
    {
        RuleFor(c => c.NomesItens).NotEmpty().WithMessage("O pedido deve ter ao menos um item.");
        RuleFor(c => c.NomesItens).Must(itens => itens.Any(n =>
            Itens.Any(c => string.Equals(c.Nome, n, StringComparison.OrdinalIgnoreCase) && c.Tipo == TipoItem.Sanduiche)))
            .WithMessage("O pedido deve conter ao menos um sanduíche.");
    }
}

public sealed class AtualizarPedidoCommandHandler(IPedidoRepository repository, IUnitOfWork unitOfWork)
    : IRequestHandler<AtualizarPedidoCommand, Result<PedidoDto>>
{
    public async Task<Result<PedidoDto>> Handle(AtualizarPedidoCommand request, CancellationToken ct)
    {
        var pedido = await repository.ObterPorIdAsync(request.Id, request.UsuarioId, ct);
        if (pedido is null)
            return Result.Failure<PedidoDto>("Pedido não encontrado.");

        var novosItens = new List<ItemPedido>();
        foreach (var nome in request.NomesItens)
        {
            var itemCardapio = Encontrar(nome);
            if (itemCardapio is null)
                return Result.Failure<PedidoDto>($"Item '{nome}' não encontrado no cardápio.");

            novosItens.Add(new ItemPedido(itemCardapio.Tipo, itemCardapio.Nome, itemCardapio.Preco));
        }

        var resultado = pedido.Atualizar(novosItens);
        if (resultado.IsFailure)
            return Result.Failure<PedidoDto>(resultado.Error!);

        repository.Atualizar(pedido);
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
