using TransitoClaro.Api.Domain;

namespace TransitoClaro.Tests;

public class ClassificadorTests
{
    private static readonly DateTime Agora = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);
    private readonly IClassificadorStatus _classificador = new ClassificadorPorRelatos();

    private static RelatoCidadao Atraso(int min)
        => RelatoCidadao.Criar(1, null, TipoOcorrencia.Atraso, null, min, null, Agora);

    private static RelatoCidadao Lotacao(NivelLotacao n)
        => RelatoCidadao.Criar(1, null, TipoOcorrencia.Lotacao, n, null, null, Agora);

    private static RelatoCidadao Falha()
        => RelatoCidadao.Criar(1, null, TipoOcorrencia.FalhaOperacional, null, null, null, Agora);

    [Fact]
    public void SemRelatos_DeveSerNormal()
        => Assert.Equal(StatusLinha.Normal, _classificador.Classificar(new List<RelatoCidadao>()));

    [Fact]
    public void AtrasoMedioAlto_DeveSerCritico()
        => Assert.Equal(StatusLinha.Critico, _classificador.Classificar(new[] { Atraso(20), Atraso(16) }));

    [Fact]
    public void AtrasoMedioPequeno_DeveSerModerado()
        => Assert.Equal(StatusLinha.Moderado, _classificador.Classificar(new[] { Atraso(6), Atraso(8) }));

    [Fact]
    public void AtrasoMuitoPequeno_DeveSerNormal()
        => Assert.Equal(StatusLinha.Normal, _classificador.Classificar(new[] { Atraso(2) }));

    [Fact]
    public void MetadeLotada_DeveSerCritico()
        => Assert.Equal(StatusLinha.Critico,
            _classificador.Classificar(new[] { Lotacao(NivelLotacao.Lotado), Lotacao(NivelLotacao.Tranquilo) }));

    [Fact]
    public void MaioriaCheia_DeveSerModerado()
        => Assert.Equal(StatusLinha.Moderado,
            _classificador.Classificar(new[] { Lotacao(NivelLotacao.Cheio), Lotacao(NivelLotacao.Cheio), Lotacao(NivelLotacao.Vazio) }));

    [Fact]
    public void UmaFalha_DeveSerModerado_TresFalhas_DeveSerCritico()
    {
        Assert.Equal(StatusLinha.Moderado, _classificador.Classificar(new[] { Falha() }));
        Assert.Equal(StatusLinha.Critico, _classificador.Classificar(new[] { Falha(), Falha(), Falha() }));
    }
}
