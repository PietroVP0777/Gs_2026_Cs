using TransitoClaro.Api.Domain;

namespace TransitoClaro.Tests;

public class RelatoCidadaoTests
{
    private static readonly DateTime Agora = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_AtrasoValido_DeveCriarRelato()
    {
        var r = RelatoCidadao.Criar(1, null, TipoOcorrencia.Atraso, null, 12, "  Trem parado  ", Agora);

        Assert.Equal(12, r.MinutosAtraso);
        Assert.Equal("Trem parado", r.Descricao);
        Assert.Equal(Agora, r.CriadoEmUtc);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(241)]
    public void Criar_AtrasoComMinutosInvalidos_DeveLancarDomainException(int? minutos)
    {
        Assert.Throws<DomainException>(() =>
            RelatoCidadao.Criar(1, null, TipoOcorrencia.Atraso, null, minutos, null, Agora));
    }

    [Fact]
    public void Criar_LotacaoSemNivel_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            RelatoCidadao.Criar(1, null, TipoOcorrencia.Lotacao, null, null, null, Agora));
    }

    [Fact]
    public void Criar_SemLinha_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() =>
            RelatoCidadao.Criar(0, null, TipoOcorrencia.FalhaOperacional, null, null, null, Agora));
    }

    [Fact]
    public void Criar_DescricaoMuitoLonga_DeveLancarDomainException()
    {
        var longa = new string('x', RelatoCidadao.MaxDescricao + 1);
        Assert.Throws<DomainException>(() =>
            RelatoCidadao.Criar(1, null, TipoOcorrencia.FalhaOperacional, null, null, longa, Agora));
    }

    [Fact]
    public void Criar_LotacaoNaoGuardaMinutosDeAtraso()
    {
        var r = RelatoCidadao.Criar(1, null, TipoOcorrencia.Lotacao, NivelLotacao.Cheio, 30, null, Agora);
        Assert.Null(r.MinutosAtraso);
    }
}
