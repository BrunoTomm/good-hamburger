using GoodHamburger.Application.Cardapio.Queries.ObterCardapio;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace GoodHamburger.Api.Controllers;

[ApiController]
[Route("cardapio")]
public sealed class CardapioController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [OutputCache(Duration = 3600)]
    public async Task<IActionResult> Obter(CancellationToken ct)
    {
        var resultado = await mediator.Send(new ObterCardapioQuery(), ct);
        return Ok(resultado);
    }
}
