using Microsoft.AspNetCore.Mvc;
using TransitoClaro.Api.Domain;

namespace TransitoClaro.Api.Infra;

/// <summary>Converte exceções de domínio em respostas HTTP padronizadas (ProblemDetails).</summary>
public class MiddlewareDeExcecoes
{
    private readonly RequestDelegate _next;

    public MiddlewareDeExcecoes(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _next(contexto);
        }
        catch (NaoEncontradoException ex)
        {
            await Responder(contexto, StatusCodes.Status404NotFound, "Recurso não encontrado", ex.Message);
        }
        catch (DomainException ex)
        {
            await Responder(contexto, StatusCodes.Status400BadRequest, "Requisição inválida", ex.Message);
        }
    }

    private static Task Responder(HttpContext contexto, int status, string titulo, string detalhe)
    {
        contexto.Response.StatusCode = status;
        return contexto.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = detalhe
        });
    }
}
