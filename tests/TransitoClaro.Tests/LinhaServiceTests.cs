using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;
using TransitoClaro.Api.Services;

namespace TransitoClaro.Tests;

public class LinhaServiceTests
{
    [Fact]
    public async Task Listar_ComFiltroDeTipo_DeveRetornarSoDoTipo()
    {
        using var db = BancoDeTeste.Criar();
        var servico = new LinhaService(db);

        var trens = await servico.ListarAsync(TipoTransporte.Trem);

        Assert.Equal(2, trens.Count);
        Assert.All(trens, l => Assert.Equal(TipoTransporte.Trem, l.Tipo));
    }

    [Fact]
    public async Task Criar_NomeDuplicado_DeveLancarDomainException()
    {
        using var db = BancoDeTeste.Criar();
        var servico = new LinhaService(db);

        await Assert.ThrowsAsync<DomainException>(() => servico.CriarAsync(new LinhaSalvarDto
        {
            Nome = "Linha 1-Azul", Tipo = TipoTransporte.Metro, Operadora = "Qualquer"
        }));
    }

    [Fact]
    public async Task CriarEAtualizar_DevePersistirAlteracoes()
    {
        using var db = BancoDeTeste.Criar();
        var servico = new LinhaService(db);

        var criada = await servico.CriarAsync(new LinhaSalvarDto
        {
            Nome = "Linha 15-Prata", Tipo = TipoTransporte.Metro, Operadora = "Metrô SP", Cor = "Prata"
        });
        var atualizada = await servico.AtualizarAsync(criada.Id, new LinhaSalvarDto
        {
            Nome = "Linha 15-Prata (Monotrilho)", Tipo = TipoTransporte.Metro, Operadora = "Metrô SP"
        });

        Assert.Equal("Linha 15-Prata (Monotrilho)", atualizada.Nome);
    }

    [Fact]
    public async Task ListarEstacoes_DeveVirEmOrdem()
    {
        using var db = BancoDeTeste.Criar();
        var servico = new LinhaService(db);

        var estacoes = await servico.ListarEstacoesAsync(3);

        Assert.Equal(new[] { 1, 2, 3, 4 }, estacoes.Select(e => e.Ordem).ToArray());
    }

    [Fact]
    public async Task Obter_Inexistente_DeveLancarNaoEncontrado()
    {
        using var db = BancoDeTeste.Criar();
        var servico = new LinhaService(db);

        await Assert.ThrowsAsync<NaoEncontradoException>(() => servico.ObterAsync(404));
    }
}
