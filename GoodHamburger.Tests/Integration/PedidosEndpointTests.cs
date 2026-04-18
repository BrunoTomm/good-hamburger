using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using GoodHamburger.Application.Auth.DTOs;
using GoodHamburger.Application.Common;
using GoodHamburger.Application.Pedidos.DTOs;

namespace GoodHamburger.Tests.Integration;

public sealed class PedidosEndpointTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<string> ObterTokenAsync()
    {
        var email = $"teste_{Guid.NewGuid()}@test.com";
        var response = await _client.PostAsJsonAsync("/auth/register", new { email, senha = "senha123" });
        var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return auth!.Token;
    }

    [Fact]
    public async Task CriarPedido_ComSanduicheValido_Retorna201()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ObterTokenAsync());

        var response = await _client.PostAsJsonAsync("/pedidos", new { itens = new[] { "X Burger" } });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var pedido = await response.Content.ReadFromJsonAsync<PedidoDto>();
        pedido!.Itens.Should().HaveCount(1);
    }

    [Fact]
    public async Task CriarPedido_ComItemDuplicado_Retorna422()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ObterTokenAsync());

        var response = await _client.PostAsJsonAsync("/pedidos", new { itens = new[] { "X Burger", "X Egg" } });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task CriarPedido_ComComboCompleto_RetornaDesconto20Porcento()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ObterTokenAsync());

        var response = await _client.PostAsJsonAsync("/pedidos", new
        {
            itens = new[] { "X Burger", "Batata Frita", "Refrigerante" }
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var pedido = await response.Content.ReadFromJsonAsync<PedidoDto>();
        pedido!.PercentualDesconto.Should().Be(0.20m);
    }

    [Fact]
    public async Task ListarPedidos_SemAutenticacao_Retorna401()
    {
        _client.DefaultRequestHeaders.Authorization = null;
        var response = await _client.GetAsync("/pedidos");
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ObterPedido_IdInexistente_Retorna404()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ObterTokenAsync());
        var response = await _client.GetAsync($"/pedidos/{Guid.NewGuid()}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeletarPedido_PedidoExistente_Retorna204()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await ObterTokenAsync());

        var create = await _client.PostAsJsonAsync("/pedidos", new { itens = new[] { "X Bacon" } });
        create.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await create.Content.ReadAsStringAsync();
        var pedido = System.Text.Json.JsonSerializer.Deserialize<PedidoDto>(content,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        pedido!.Id.Should().NotBe(Guid.Empty);

        var delete = await _client.DeleteAsync($"/pedidos/{pedido.Id}");
        delete.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getAfter = await _client.GetAsync($"/pedidos/{pedido.Id}");
        getAfter.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
