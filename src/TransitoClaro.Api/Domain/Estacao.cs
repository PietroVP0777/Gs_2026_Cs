namespace TransitoClaro.Api.Domain;

public class Estacao : EntidadeBase
{
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public int LinhaId { get; set; }
    public Linha? Linha { get; set; }
}
