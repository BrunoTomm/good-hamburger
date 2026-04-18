using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GoodHamburger.Api.Extensions;
using GoodHamburger.Application.Pedidos.Commands.AtualizarPedido;
using GoodHamburger.Application.Pedidos.Commands.CriarPedido;
using GoodHamburger.Application.Pedidos.Commands.RemoverPedido;
using GoodHamburger.Application.Pedidos.Queries.ListarPedidos;
using GoodHamburger.Application.Pedidos.Queries.ObterPedido;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.Api.Controllers;

[Authorize]
[ApiController]
[Route("pedidos")]
public sealed class PedidosController(IMediator mediator) : ControllerBase
{
    private Guid UsuarioId => Guid.Parse(
        User.FindFirstValue(JwtRegisteredClaimNames.Sub)
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("User claim not found."));

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int tamanhoPagina = 10, CancellationToken ct = default)
    {
        var resultado = await mediator.Send(new ListarPedidosQuery(UsuarioId, pagina, tamanhoPagina), ct);
        return Ok(resultado);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new ObterPedidoQuery(id, UsuarioId), ct);
        return resultado.ToActionResult(this);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoRequest request, CancellationToken ct)
    {
        var resultado = await mediator.Send(new CriarPedidoCommand(UsuarioId, request.Itens), ct);
        return resultado.IsSuccess
            ? CreatedAtAction(nameof(ObterPorId), new { id = resultado.Value.Id }, resultado.Value)
            : resultado.ToActionResult(this);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarPedidoRequest request, CancellationToken ct)
    {
        var resultado = await mediator.Send(new AtualizarPedidoCommand(id, UsuarioId, request.Itens), ct);
        return resultado.ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Remover(Guid id, CancellationToken ct)
    {
        var resultado = await mediator.Send(new RemoverPedidoCommand(id, UsuarioId), ct);
        return resultado.ToActionResult(this);
    }
}

public sealed record CriarPedidoRequest(List<string> Itens);
public sealed record AtualizarPedidoRequest(List<string> Itens);
