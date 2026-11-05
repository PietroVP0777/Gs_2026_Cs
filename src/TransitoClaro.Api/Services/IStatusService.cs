using TransitoClaro.Api.Dtos;

namespace TransitoClaro.Api.Services;

public interface IStatusService
{
    Task<StatusLinhaDto> ObterAsync(int linhaId, int janelaMinutos, CancellationToken ct = default);
    Task<IReadOnlyList<StatusLinhaDto>> ObterTodasAsync(int janelaMinutos, CancellationToken ct = default);
}
