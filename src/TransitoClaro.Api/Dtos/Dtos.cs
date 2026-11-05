using System.ComponentModel.DataAnnotations;
using TransitoClaro.Api.Domain;

namespace TransitoClaro.Api.Dtos;

public class LinhaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public TipoTransporte Tipo { get; set; }
    public string Operadora { get; set; } = string.Empty;
    public string? Cor { get; set; }
}

public class LinhaSalvarDto
{
    [Required, StringLength(80, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public TipoTransporte? Tipo { get; set; }

    [Required, StringLength(80, MinimumLength = 2)]
    public string Operadora { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Cor { get; set; }
}

public class EstacaoDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public int LinhaId { get; set; }
}

public class RelatoCriarDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe uma linha válida.")]
    public int LinhaId { get; set; }

    public int? EstacaoId { get; set; }

    [Required]
    public TipoOcorrencia? Tipo { get; set; }

    public NivelLotacao? NivelLotacao { get; set; }

    [Range(1, RelatoCidadao.MaxMinutosAtraso)]
    public int? MinutosAtraso { get; set; }

    [StringLength(RelatoCidadao.MaxDescricao)]
    public string? Descricao { get; set; }
}

public class RelatoDto
{
    public int Id { get; set; }
    public int LinhaId { get; set; }
    public string LinhaNome { get; set; } = string.Empty;
    public int? EstacaoId { get; set; }
    public string? EstacaoNome { get; set; }
    public TipoOcorrencia Tipo { get; set; }
    public NivelLotacao? NivelLotacao { get; set; }
    public int? MinutosAtraso { get; set; }
    public string? Descricao { get; set; }
    public DateTime CriadoEmUtc { get; set; }
}

public class ResultadoPaginado<T>
{
    public IReadOnlyList<T> Itens { get; set; } = Array.Empty<T>();
    public int Pagina { get; set; }
    public int Tamanho { get; set; }
    public int Total { get; set; }
}

public class StatusLinhaDto
{
    public int LinhaId { get; set; }
    public string Linha { get; set; } = string.Empty;
    public TipoTransporte Tipo { get; set; }
    public StatusLinha Status { get; set; }
    public double AtrasoMedioMinutos { get; set; }
    public NivelLotacao? LotacaoPredominante { get; set; }
    public int TotalRelatos { get; set; }
    public int JanelaMinutos { get; set; }
    /// <summary>Mensagem em linguagem simples para o passageiro.</summary>
    public string Mensagem { get; set; } = string.Empty;
}

public class LinhaAtrasoDto
{
    public int LinhaId { get; set; }
    public string Linha { get; set; } = string.Empty;
    public int TotalAtrasos { get; set; }
    public double AtrasoMedioMinutos { get; set; }
}

public class EstatisticasDto
{
    public int PeriodoHoras { get; set; }
    public int TotalRelatos { get; set; }
    public Dictionary<string, int> PorTipo { get; set; } = new();
    public IReadOnlyList<LinhaAtrasoDto> LinhasComMaisAtrasos { get; set; } = Array.Empty<LinhaAtrasoDto>();
}
