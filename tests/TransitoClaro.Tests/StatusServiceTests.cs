using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;
using TransitoClaro.Api.Services;

namespace TransitoClaro.Tests;

public class StatusServiceTests
{
    private readonly RelogioFalso _relogio = new();

    [Fact]
    public async Task Obter_RelatosRecentes_DeveClassificarCritico()
    {
        using var db = BancoDeTeste.Criar();
        var relatos = new RelatoService(db, _relogio);
        var status = new StatusService(db, new ClassificadorPorRelatos(), _relogio);

        await relatos.CriarAsync(new RelatoCriarDto { LinhaId = 4, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 25 });

        var resultado = await status.ObterAsync(4, 60);

        Assert.Equal(StatusLinha.Critico, resultado.Status);
        Assert.Equal(25.0, resultado.AtrasoMedioMinutos);
        Assert.Equal(1, resultado.TotalRelatos);
    }

    [Fact]
    public async Task Obter_RelatoForaDaJanela_DeveSerIgnorado()
    {
        using var db = BancoDeTeste.Criar();
        var relatos = new RelatoService(db, _relogio);
        var status = new StatusService(db, new ClassificadorPorRelatos(), _relogio);

        await relatos.CriarAsync(new RelatoCriarDto { LinhaId = 4, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 25 });
        _relogio.AgoraUtc = _relogio.AgoraUtc.AddHours(3); // relato fica velho demais

        var resultado = await status.ObterAsync(4, 60);

        Assert.Equal(StatusLinha.Normal, resultado.Status);
        Assert.Equal(0, resultado.TotalRelatos);
    }

    [Fact]
    public async Task Obter_LinhaInexistente_DeveLancarNaoEncontrado()
    {
        using var db = BancoDeTeste.Criar();
        var status = new StatusService(db, new ClassificadorPorRelatos(), _relogio);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => status.ObterAsync(999, 60));
    }

    [Fact]
    public async Task ObterTodas_DeveColocarLinhaCriticaPrimeiro()
    {
        using var db = BancoDeTeste.Criar();
        var relatos = new RelatoService(db, _relogio);
        var status = new StatusService(db, new ClassificadorPorRelatos(), _relogio);

        await relatos.CriarAsync(new RelatoCriarDto { LinhaId = 5, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 30 });

        var todas = await status.ObterTodasAsync(60);

        Assert.Equal(6, todas.Count);
        Assert.Equal(5, todas[0].LinhaId);
        Assert.Equal(StatusLinha.Critico, todas[0].Status);
    }
}
