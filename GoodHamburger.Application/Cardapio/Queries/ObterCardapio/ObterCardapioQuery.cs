using GoodHamburger.Application.Cardapio.DTOs;
using MediatR;
using DomainCardapio = GoodHamburger.Domain.Cardapio.Cardapio;

namespace GoodHamburger.Application.Cardapio.Queries.ObterCardapio;

public sealed record ObterCardapioQuery : IRequest<IReadOnlyList<ItemCardapioDto>>;

public sealed class ObterCardapioQueryHandler : IRequestHandler<ObterCardapioQuery, IReadOnlyList<ItemCardapioDto>>
{
    public Task<IReadOnlyList<ItemCardapioDto>> Handle(ObterCardapioQuery request, CancellationToken ct)
    {
        IReadOnlyList<ItemCardapioDto> resultado = DomainCardapio.Itens
            .Select(i => new ItemCardapioDto(i.Nome, i.Preco, i.Tipo.ToString(), i.Categoria))
            .ToList();

        return Task.FromResult(resultado);
    }
}
