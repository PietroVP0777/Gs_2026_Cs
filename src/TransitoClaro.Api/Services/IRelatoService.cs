using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;

namespace TransitoClaro.Api.Services;

public interface IRelatoService
{
    Task<ResultadoPaginado<RelatoDto>> ListarAsync(int? linhaId, TipoOcorrencia? tipo, int pagina, int tamanho, CancellationToken ct = default);
    Task<RelatoDto> ObterAsync(int id, CancellationToken ct = default);
    Task<RelatoDto> CriarAsync(RelatoCriarDto dto, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
    Task<EstatisticasDto> EstatisticasAsync(int horas, CancellationToken ct = default);
}
