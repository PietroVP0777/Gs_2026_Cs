using Microsoft.EntityFrameworkCore;
using TransitoClaro.Api.Data;
using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;
using TransitoClaro.Api.Infra;

namespace TransitoClaro.Api.Services;

public class RelatoService : IRelatoService
{
    private readonly AppDbContext _db;
    private readonly IRelogio _relogio;

    public RelatoService(AppDbContext db, IRelogio relogio)
    {
        _db = db;
        _relogio = relogio;
    }

    public async Task<ResultadoPaginado<RelatoDto>> ListarAsync(
        int? linhaId, TipoOcorrencia? tipo, int pagina, int tamanho, CancellationToken ct = default)
    {
        pagina = Math.Max(1, pagina);
        tamanho = Math.Clamp(tamanho, 1, 100);

        var consulta = _db.Relatos.AsNoTracking().AsQueryable();
        if (linhaId.HasValue) consulta = consulta.Where(r => r.LinhaId == linhaId.Value);
        if (tipo.HasValue) consulta = consulta.Where(r => r.Tipo == tipo.Value);

        int total = await consulta.CountAsync(ct);

        var itens = await consulta
            .OrderByDescending(r => r.CriadoEmUtc)
            .ThenByDescending(r => r.Id)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(r => new RelatoDto
            {
                Id = r.Id,
                LinhaId = r.LinhaId,
                LinhaNome = r.Linha!.Nome,
                EstacaoId = r.EstacaoId,
                EstacaoNome = r.Estacao != null ? r.Estacao.Nome : null,
                Tipo = r.Tipo,
                NivelLotacao = r.NivelLotacao,
                MinutosAtraso = r.MinutosAtraso,
                Descricao = r.Descricao,
                CriadoEmUtc = r.CriadoEmUtc
            })
            .ToListAsync(ct);

        return new ResultadoPaginado<RelatoDto> { Itens = itens, Pagina = pagina, Tamanho = tamanho, Total = total };
    }

    public async Task<RelatoDto> ObterAsync(int id, CancellationToken ct = default)
    {
        var r = await _db.Relatos.AsNoTracking()
            .Include(x => x.Linha).Include(x => x.Estacao)
            .FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NaoEncontradoException($"Relato {id} não encontrado.");
        return ParaDto(r);
    }

    public async Task<RelatoDto> CriarAsync(RelatoCriarDto dto, CancellationToken ct = default)
    {
        var linha = await _db.Linhas.AsNoTracking().FirstOrDefaultAsync(l => l.Id == dto.LinhaId, ct)
            ?? throw new NaoEncontradoException($"Linha {dto.LinhaId} não encontrada.");

        Estacao? estacao = null;
        if (dto.EstacaoId.HasValue)
        {
            estacao = await _db.Estacoes.AsNoTracking().FirstOrDefaultAsync(e => e.Id == dto.EstacaoId.Value, ct)
                ?? throw new NaoEncontradoException($"Estação {dto.EstacaoId} não encontrada.");
            if (estacao.LinhaId != linha.Id)
                throw new DomainException("A estação informada não pertence à linha escolhida.");
        }

        var relato = RelatoCidadao.Criar(
            linha.Id, estacao?.Id, dto.Tipo!.Value, dto.NivelLotacao,
            dto.MinutosAtraso, dto.Descricao, _relogio.AgoraUtc);

        _db.Relatos.Add(relato);
        await _db.SaveChangesAsync(ct);

        return new RelatoDto
        {
            Id = relato.Id, LinhaId = linha.Id, LinhaNome = linha.Nome,
            EstacaoId = estacao?.Id, EstacaoNome = estacao?.Nome,
            Tipo = relato.Tipo, NivelLotacao = relato.NivelLotacao,
            MinutosAtraso = relato.MinutosAtraso, Descricao = relato.Descricao,
            CriadoEmUtc = relato.CriadoEmUtc
        };
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var relato = await _db.Relatos.FirstOrDefaultAsync(r => r.Id == id, ct)
            ?? throw new NaoEncontradoException($"Relato {id} não encontrado.");
        _db.Relatos.Remove(relato);
        await _db.SaveChangesAsync(ct);
    }

    /// <summary>Indicador de impacto: volume de relatos e linhas mais afetadas por atraso no período.</summary>
    public async Task<EstatisticasDto> EstatisticasAsync(int horas, CancellationToken ct = default)
    {
        horas = Math.Clamp(horas, 1, 24 * 30);
        var limite = _relogio.AgoraUtc.AddHours(-horas);

        var relatos = await _db.Relatos.AsNoTracking()
            .Include(r => r.Linha)
            .Where(r => r.CriadoEmUtc >= limite)
            .ToListAsync(ct);

        var porTipo = relatos
            .GroupBy(r => r.Tipo)
            .ToDictionary(g => g.Key.ToString(), g => g.Count());

        var ranking = relatos
            .Where(r => r.Tipo == TipoOcorrencia.Atraso && r.MinutosAtraso.HasValue)
            .GroupBy(r => new { r.LinhaId, Nome = r.Linha!.Nome })
            .Select(g => new LinhaAtrasoDto
            {
                LinhaId = g.Key.LinhaId,
                Linha = g.Key.Nome,
                TotalAtrasos = g.Count(),
                AtrasoMedioMinutos = Math.Round(g.Average(r => r.MinutosAtraso!.Value), 1)
            })
            .OrderByDescending(x => x.TotalAtrasos)
            .ThenByDescending(x => x.AtrasoMedioMinutos)
            .Take(5)
            .ToList();

        return new EstatisticasDto
        {
            PeriodoHoras = horas,
            TotalRelatos = relatos.Count,
            PorTipo = porTipo,
            LinhasComMaisAtrasos = ranking
        };
    }

    private static RelatoDto ParaDto(RelatoCidadao r) => new()
    {
        Id = r.Id, LinhaId = r.LinhaId, LinhaNome = r.Linha?.Nome ?? string.Empty,
        EstacaoId = r.EstacaoId, EstacaoNome = r.Estacao?.Nome,
        Tipo = r.Tipo, NivelLotacao = r.NivelLotacao, MinutosAtraso = r.MinutosAtraso,
        Descricao = r.Descricao, CriadoEmUtc = r.CriadoEmUtc
    };
}
