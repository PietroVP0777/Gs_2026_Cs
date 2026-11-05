using Microsoft.AspNetCore.Mvc;
using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;
using TransitoClaro.Api.Services;

namespace TransitoClaro.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RelatosController : ControllerBase
{
    private readonly IRelatoService _service;

    public RelatosController(IRelatoService service) => _service = service;

    /// <summary>Lista relatos de passageiros, do mais recente para o mais antigo.</summary>
    [HttpGet]
    public async Task<ActionResult<ResultadoPaginado<RelatoDto>>> Listar(
        [FromQuery] int? linhaId, [FromQuery] TipoOcorrencia? tipo,
        [FromQuery] int pagina = 1, [FromQuery] int tamanho = 20, CancellationToken ct = default)
        => Ok(await _service.ListarAsync(linhaId, tipo, pagina, tamanho, ct));

    /// <summary>Obtém um relato pelo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelatoDto>> Obter(int id, CancellationToken ct)
        => Ok(await _service.ObterAsync(id, ct));

    /// <summary>Envia um relato anônimo (atraso, lotação ou falha operacional).</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelatoDto>> Criar([FromBody] RelatoCriarDto dto, CancellationToken ct)
    {
        var criado = await _service.CriarAsync(dto, ct);
        return CreatedAtAction(nameof(Obter), new { id = criado.Id }, criado);
    }

    /// <summary>Remove um relato.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _service.RemoverAsync(id, ct);
        return NoContent();
    }

    /// <summary>Indicador de impacto: total de relatos e linhas com mais atrasos no período.</summary>
    [HttpGet("estatisticas")]
    public async Task<ActionResult<EstatisticasDto>> Estatisticas([FromQuery] int horas = 24, CancellationToken ct = default)
        => Ok(await _service.EstatisticasAsync(horas, ct));
}
