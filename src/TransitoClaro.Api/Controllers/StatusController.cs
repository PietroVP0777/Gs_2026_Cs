using Microsoft.AspNetCore.Mvc;
using TransitoClaro.Api.Dtos;
using TransitoClaro.Api.Services;

namespace TransitoClaro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StatusController : ControllerBase
{
    private readonly IStatusService _service;

    public StatusController(IStatusService service) => _service = service;

    /// <summary>Status atual de todas as linhas (as mais problemáticas primeiro).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StatusLinhaDto>>> ObterTodas(
        [FromQuery] int janelaMinutos = 60, CancellationToken ct = default)
        => Ok(await _service.ObterTodasAsync(janelaMinutos, ct));

    /// <summary>Status atual de uma linha, calculado a partir dos relatos recentes.</summary>
    [HttpGet("{linhaId:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StatusLinhaDto>> Obter(
        int linhaId, [FromQuery] int janelaMinutos = 60, CancellationToken ct = default)
        => Ok(await _service.ObterAsync(linhaId, janelaMinutos, ct));
}
