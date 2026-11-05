namespace TransitoClaro.Api.Domain;

public class Linha : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public TipoTransporte Tipo { get; set; }
    public string Operadora { get; set; } = string.Empty;
    public string? Cor { get; set; }

    public ICollection<Estacao> Estacoes { get; set; } = new List<Estacao>();
    public ICollection<RelatoCidadao> Relatos { get; set; } = new List<RelatoCidadao>();
}
