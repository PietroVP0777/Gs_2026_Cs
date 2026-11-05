using Microsoft.EntityFrameworkCore;
using TransitoClaro.Api.Data;
using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;
using TransitoClaro.Api.Infra;

namespace TransitoClaro.Api.Services;

/// <summary>Responde à pergunta principal do passageiro: "como está minha linha agora?"</summary>
public class StatusService : IStatusService
{
    private readonly AppDbContext _db;
    private readonly IClassificadorStatus _classificador;
    private readonly IRelogio _relogio;

    public StatusService(AppDbContext db, IClassificadorStatus classificador, IRelogio relogio)
    {
        _db = db;
        _classificador = classificador;
        _relogio = relogio;
    }

    public async Task<StatusLinhaDto> ObterAsync(int linhaId, int janelaMinutos, CancellationToken ct = default)
    {
        janelaMinutos = NormalizarJanela(janelaMinutos);

        var linha = await _db.Linhas.AsNoTracking().FirstOrDefaultAsync(l => l.Id == linhaId, ct)
            ?? throw new NaoEncontradoException($"Linha {linhaId} não encontrada.");

        var limite = _relogio.AgoraUtc.AddMinutes(-janelaMinutos);
        var relatos = await _db.Relatos.AsNoTracking()
            .Where(r => r.LinhaId == linhaId && r.CriadoEmUtc >= limite)
            .ToListAsync(ct);

        return Montar(linha, relatos, janelaMinutos);
    }

    public async Task<IReadOnlyList<StatusLinhaDto>> ObterTodasAsync(int janelaMinutos, CancellationToken ct = default)
    {
        janelaMinutos = NormalizarJanela(janelaMinutos);

        var linhas = await _db.Linhas.AsNoTracking().ToListAsync(ct);
        var limite = _relogio.AgoraUtc.AddMinutes(-janelaMinutos);
        var relatos = await _db.Relatos.AsNoTracking()
            .Where(r => r.CriadoEmUtc >= limite)
            .ToListAsync(ct);

        var porLinha = relatos.ToLookup(r => r.LinhaId);

        return linhas
            .Select(l => Montar(l, porLinha[l.Id].ToList(), janelaMinutos))
            .OrderByDescending(s => s.Status)
            .ThenByDescending(s => s.AtrasoMedioMinutos)
            .ThenBy(s => s.Linha)
            .ToList();
    }

    private StatusLinhaDto Montar(Linha linha, List<RelatoCidadao> relatos, int janelaMinutos)
    {
        var status = _classificador.Classificar(relatos);

        double atrasoMedio = relatos
            .Where(r => r.Tipo == TipoOcorrencia.Atraso && r.MinutosAtraso.HasValue)
            .Select(r => (double)r.MinutosAtraso!.Value)
            .DefaultIfEmpty(0)
            .Average();

        NivelLotacao? lotacao = relatos
            .Where(r => r.NivelLotacao.HasValue)
            .GroupBy(r => r.NivelLotacao!.Value)
            .OrderByDescending(g => g.Count())
            .ThenByDescending(g => g.Key)
            .Select(g => (NivelLotacao?)g.Key)
            .FirstOrDefault();

        return new StatusLinhaDto
        {
            LinhaId = linha.Id,
            Linha = linha.Nome,
            Tipo = linha.Tipo,
            Status = status,
            AtrasoMedioMinutos = Math.Round(atrasoMedio, 1),
            LotacaoPredominante = lotacao,
            TotalRelatos = relatos.Count,
            JanelaMinutos = janelaMinutos,
            Mensagem = MensagemPara(status, relatos.Count, atrasoMedio)
        };
    }

    private static string MensagemPara(StatusLinha status, int totalRelatos, double atrasoMedio) => status switch
    {
        _ when totalRelatos == 0 => "Sem relatos recentes. Nenhum problema informado pelos passageiros.",
        StatusLinha.Critico => $"Atenção: a linha está com problemas (atraso médio de {atrasoMedio:0} min ou muita lotação). Considere uma rota alternativa.",
        StatusLinha.Moderado => "A linha tem alguns problemas relatados. Saia com um pouco mais de antecedência.",
        _ => "A linha está funcionando normalmente segundo os relatos recentes."
    };

    private static int NormalizarJanela(int minutos) => Math.Clamp(minutos, 5, 720);
}
