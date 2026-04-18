namespace GoodHamburger.Application.Cardapio.DTOs;

public sealed record ItemCardapioDto(string Nome, decimal Preco, string Tipo, string Categoria);
public sealed record GrupoCardapioDto(string Categoria, IReadOnlyList<ItemCardapioDto> Itens);
