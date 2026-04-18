# Good Hamburger API

Sistema de registro de pedidos para a lanchonete Good Hamburger.

## Executando

### Pré-requisitos

- .NET 8 SDK
- SQL Server (local ou Docker)

### Configuração

Edite a connection string em `GoodHamburger.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=GoodHamburger;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### Rodando a API

```bash
cd GoodHamburger.Api
dotnet run
```

A migration é aplicada automaticamente na inicialização. A API sobe em `http://localhost:5000`.

Documentação interativa (Scalar): `http://localhost:5000/scalar/v1`

### Rodando o Blazor

```bash
cd GoodHamburger.Blazor
dotnet run
```

### Rodando os testes

```bash
dotnet test
```

## Endpoints

| Método | Rota | Auth | Descrição |
|--------|------|------|-----------|
| POST | /auth/register | — | Cadastro |
| POST | /auth/login | — | Login |
| GET | /cardapio | — | Lista o cardápio |
| GET | /pedidos | Bearer | Lista pedidos (paginado) |
| GET | /pedidos/{id} | Bearer | Consulta pedido |
| POST | /pedidos | Bearer | Cria pedido |
| PUT | /pedidos/{id} | Bearer | Atualiza pedido |
| DELETE | /pedidos/{id} | Bearer | Remove pedido (soft delete) |

### Criando um pedido

```bash
# 1. Registrar
curl -X POST http://localhost:5000/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"dev@test.com","senha":"senha123"}'

# 2. Usar o token retornado
curl -X POST http://localhost:5000/pedidos \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"itens":["X Burger","Batata Frita","Refrigerante"]}'
```

### Regras de negócio

- Todo pedido deve conter exatamente um sanduíche
- Itens duplicados do mesmo tipo são rejeitados
- Descontos aplicados automaticamente:
  - Sanduíche + Batata + Refrigerante → 20%
  - Sanduíche + Refrigerante → 15%
  - Sanduíche + Batata → 10%

## Arquitetura

**Clean Architecture** em 4 camadas com dependência unidirecional:

```
Domain → sem dependências externas
Application → depende de Domain
Infrastructure → depende de Domain + Application
Api → depende de todos
```

**Decisões técnicas:**

- **CQRS com MediatR**: commands e queries separados, um arquivo por operação.
- **Result Pattern**: domínio retorna `Result<T>` — zero exceções para fluxo de negócio. Erros são valores, não exceções.
- **Domain Events**: `Pedido` emite `PedidoCriadoEvent` / `PedidoAtualizadoEvent` in-process via MediatR. Handlers podem ser acoplados sem tocar no aggregate.
- **Soft Delete + Global Query Filter**: `DELETE` marca `DeletadoEm` no banco. O EF Core filtra automaticamente via `HasQueryFilter` — nenhuma query precisa se preocupar com registros deletados.
- **Cálculo de desconto no domínio**: `switch expression` com pattern matching, não no handler. Lógica de negócio onde deveria estar.
- **Problem Details (RFC 7807)**: todos os erros seguem o mesmo contrato JSON — `type`, `title`, `detail`, `status`.
- **Output Cache no cardápio**: o cardápio é estático; cacheado por 1 hora sem dependência externa.
- **Scalar**: substituição do Swagger UI, mais limpa e moderna.

**O que ficou fora:**

- Refresh token (fora de escopo)
- Roles/permissões avançadas
- Docker Compose
- CI/CD
- Frontend Blazor integrado com HTTPS (requer configuração de CORS)
