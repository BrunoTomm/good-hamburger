using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GoodHamburger.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validação falhou: {Errors}", ex.Errors);
            context.Response.StatusCode = 422;
            context.Response.ContentType = "application/problem+json";

            var problem = new ValidationProblemDetails(
                ex.Errors.GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()))
            {
                Title = "Erro de validação",
                Status = 422,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.5.21"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro inesperado");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Title = "Erro interno",
                Status = 500,
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
