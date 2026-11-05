using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;

namespace TransitoClaro.Api.Services;

public interface ILinhaService
{
    Task<IReadOnlyList<LinhaDto>> ListarAsync(TipoTransporte? tipo, CancellationToken ct = default);
    Task<LinhaDto> ObterAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<EstacaoDto>> ListarEstacoesAsync(int linhaId, CancellationToken ct = default);
    Task<LinhaDto> CriarAsync(LinhaSalvarDto dto, CancellationToken ct = default);
    Task<LinhaDto> AtualizarAsync(int id, LinhaSalvarDto dto, CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}
