namespace TransitoClaro.Api.Infra;

/// <summary>Abstração do relógio: permite controlar o tempo nos testes.</summary>
public interface IRelogio
{
    DateTime AgoraUtc { get; }
}

public class RelogioSistema : IRelogio
{
    public DateTime AgoraUtc => DateTime.UtcNow;
}
