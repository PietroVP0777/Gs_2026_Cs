using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;
using TransitoClaro.Api.Services;

namespace TransitoClaro.Tests;

public class RelatoServiceTests
{
    private readonly RelogioFalso _relogio = new();

    private RelatoService CriarServico(Api.Data.AppDbContext db) => new(db, _relogio);

    [Fact]
    public async Task Criar_ComDadosValidos_DevePersistirRelato()
    {
        using var db = BancoDeTeste.Criar();
        var servico = CriarServico(db);

        var criado = await servico.CriarAsync(new RelatoCriarDto
        {
            LinhaId = 4, EstacaoId = 14, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 10
        });

        Assert.True(criado.Id > 0);
        Assert.Equal("Linha 11-Coral", criado.LinhaNome);
        Assert.Equal("Brás", criado.EstacaoNome);
        Assert.Equal(1, db.Relatos.Count());
    }

    [Fact]
    public async Task Criar_LinhaInexistente_DeveLancarNaoEncontrado()
    {
        using var db = BancoDeTeste.Criar();
        var servico = CriarServico(db);

        await Assert.ThrowsAsync<NaoEncontradoException>(() =>
            servico.CriarAsync(new RelatoCriarDto { LinhaId = 999, Tipo = TipoOcorrencia.FalhaOperacional }));
    }

    [Fact]
    public async Task Criar_EstacaoDeOutraLinha_DeveLancarDomainException()
    {
        using var db = BancoDeTeste.Criar();
        var servico = CriarServico(db);

        // Estação 14 (Brás) pertence à linha 4, não à linha 1
        await Assert.ThrowsAsync<DomainException>(() =>
            servico.CriarAsync(new RelatoCriarDto { LinhaId = 1, EstacaoId = 14, Tipo = TipoOcorrencia.FalhaOperacional }));
    }

    [Fact]
    public async Task Listar_DeveFiltrarPorLinhaEOrdenarDoMaisRecente()
    {
        using var db = BancoDeTeste.Criar();
        var servico = CriarServico(db);

        await servico.CriarAsync(new RelatoCriarDto { LinhaId = 1, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 5 });
        _relogio.AgoraUtc = _relogio.AgoraUtc.AddMinutes(10);
        await servico.CriarAsync(new RelatoCriarDto { LinhaId = 1, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 9 });
        await servico.CriarAsync(new RelatoCriarDto { LinhaId = 2, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 3 });

        var resultado = await servico.ListarAsync(1, null, 1, 10);

        Assert.Equal(2, resultado.Total);
        Assert.Equal(9, resultado.Itens[0].MinutosAtraso);
    }

    [Fact]
    public async Task Estatisticas_DeveRankearLinhasPorAtrasos()
    {
        using var db = BancoDeTeste.Criar();
        var servico = CriarServico(db);

        await servico.CriarAsync(new RelatoCriarDto { LinhaId = 3, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 10 });
        await servico.CriarAsync(new RelatoCriarDto { LinhaId = 3, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 20 });
        await servico.CriarAsync(new RelatoCriarDto { LinhaId = 1, Tipo = TipoOcorrencia.Atraso, MinutosAtraso = 5 });
        await servico.CriarAsync(new RelatoCriarDto { LinhaId = 1, Tipo = TipoOcorrencia.Lotacao, NivelLotacao = NivelLotacao.Lotado });

        var stats = await servico.EstatisticasAsync(24);

        Assert.Equal(4, stats.TotalRelatos);
        Assert.Equal(3, stats.LinhasComMaisAtrasos[0].LinhaId);
        Assert.Equal(15.0, stats.LinhasComMaisAtrasos[0].AtrasoMedioMinutos);
        Assert.Equal(2, stats.PorTipo["Atraso"]);
    }

    [Fact]
    public async Task Remover_RelatoInexistente_DeveLancarNaoEncontrado()
    {
        using var db = BancoDeTeste.Criar();
        var servico = CriarServico(db);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => servico.RemoverAsync(123));
    }
}
