namespace TransitoClaro.Api.Domain;

/// <summary>
/// Relato anônimo enviado por um passageiro (atraso, lotação ou falha).
/// Não guarda nenhum dado pessoal. A criação passa por <see cref="Criar"/>,
/// que garante as regras de negócio.
/// </summary>
public class RelatoCidadao : EntidadeBase
{
    public const int MaxMinutosAtraso = 240;
    public const int MaxDescricao = 280;

    public int LinhaId { get; private set; }
    public Linha? Linha { get; private set; }
    public int? EstacaoId { get; private set; }
    public Estacao? Estacao { get; private set; }
    public TipoOcorrencia Tipo { get; private set; }
    public NivelLotacao? NivelLotacao { get; private set; }
    public int? MinutosAtraso { get; private set; }
    public string? Descricao { get; private set; }
    public DateTime CriadoEmUtc { get; private set; }

    private RelatoCidadao() { } // exigido pelo EF Core

    public static RelatoCidadao Criar(
        int linhaId,
        int? estacaoId,
        TipoOcorrencia tipo,
        NivelLotacao? nivelLotacao,
        int? minutosAtraso,
        string? descricao,
        DateTime agoraUtc)
    {
        if (linhaId <= 0)
            throw new DomainException("A linha é obrigatória.");

        if (tipo == TipoOcorrencia.Atraso)
        {
            if (minutosAtraso is null || minutosAtraso <= 0)
                throw new DomainException("Para relatar atraso, informe os minutos de atraso (maior que zero).");
            if (minutosAtraso > MaxMinutosAtraso)
                throw new DomainException($"Atraso máximo aceito: {MaxMinutosAtraso} minutos.");
        }

        if (tipo == TipoOcorrencia.Lotacao && nivelLotacao is null)
            throw new DomainException("Para relatar lotação, informe o nível de lotação.");

        if (descricao is { Length: > MaxDescricao })
            throw new DomainException($"A descrição deve ter no máximo {MaxDescricao} caracteres.");

        return new RelatoCidadao
        {
            LinhaId = linhaId,
            EstacaoId = estacaoId,
            Tipo = tipo,
            NivelLotacao = nivelLotacao,
            MinutosAtraso = tipo == TipoOcorrencia.Atraso ? minutosAtraso : null,
            Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim(),
            CriadoEmUtc = agoraUtc
        };
    }
}
