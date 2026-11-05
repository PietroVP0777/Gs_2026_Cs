using Microsoft.AspNetCore.Mvc;
using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;
using TransitoClaro.Api.Services;

namespace TransitoClaro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class LinhasController : ControllerBase
{
    private readonly ILinhaService _service;

    public LinhasController(ILinhaService service) => _service = service;

    /// <summary>Lista as linhas de metrô, trem e BRT (filtro opcional por tipo).</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LinhaDto>>> Listar([FromQuery] TipoTransporte? tipo, CancellationToken ct)
        => Ok(await _service.ListarAsync(tipo, ct));

    /// <summary>Obtém uma linha pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LinhaDto>> Obter(int id, CancellationToken ct)
        => Ok(await _service.ObterAsync(id, ct));

    /// <summary>Lista as estações de uma linha, em ordem.</summary>
    [HttpGet("{id:int}/estacoes")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<EstacaoDto>>> ListarEstacoes(int id, CancellationToken ct)
        => Ok(await _service.ListarEstacoesAsync(id, ct));

    /// <summary>Cadastra uma nova linha.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LinhaDto>> Criar([FromBody] LinhaSalvarDto dto, CancellationToken ct)
    {
        var criada = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(Obter), new { id = criada.Id }, criada);
    }

    /// <summary>Atualiza uma linha existente.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LinhaDto>> Atualizar(int id, [FromBody] LinhaSalvarDto dto, CancellationToken ct)
        => Ok(await _service.AtualizarAsync(id, dto, ct));

    /// <summary>Remove uma linha (e seus relatos e estações).</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }
}
