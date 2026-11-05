namespace TransitoClaro.Api.Domain;

/// <summary>Estratégia que transforma relatos recentes em um status simples para o cidadão.</summary>
public interface IClassificadorStatus
{
    StatusLinha Classificar(IReadOnlyCollection<RelatoCidadao> relatosRecentes);
}

public class ClassificadorPorRelatos : IClassificadorStatus
{
    public const double AtrasoCriticoMin = 15;
    public const double AtrasoModeradoMin = 5;
    public const double ProporcaoLimite = 0.5;
    public const int FalhasCritico = 3;

    public StatusLinha Classificar(IReadOnlyCollection<RelatoCidadao> relatosRecentes)
    {
        if (relatosRecentes.Count == 0)
            return StatusLinha.Normal;

        var atrasos = relatosRecentes
            .Where(r => r.Tipo == TipoOcorrencia.Atraso && r.MinutosAtraso.HasValue)
            .Select(r => r.MinutosAtraso!.Value)
            .ToList();
        double atrasoMedio = atrasos.Count > 0 ? atrasos.Average() : 0;

        var comLotacao = relatosRecentes.Where(r => r.NivelLotacao.HasValue).ToList();
        double propLotado = comLotacao.Count == 0 ? 0
            : comLotacao.Count(r => r.NivelLotacao == NivelLotacao.Lotado) / (double)comLotacao.Count;
        double propCheioOuMais = comLotacao.Count == 0 ? 0
            : comLotacao.Count(r => r.NivelLotacao >= NivelLotacao.Cheio) / (double)comLotacao.Count;

        int falhas = relatosRecentes.Count(r => r.Tipo == TipoOcorrencia.FalhaOperacional);

        if (atrasoMedio >= AtrasoCriticoMin || propLotado >= ProporcaoLimite || falhas >= FalhasCritico)
            return StatusLinha.Critico;

        if (atrasoMedio >= AtrasoModeradoMin || propCheioOuMais >= ProporcaoLimite || falhas >= 1)
            return StatusLinha.Moderado;

        return StatusLinha.Normal;
    }
}
