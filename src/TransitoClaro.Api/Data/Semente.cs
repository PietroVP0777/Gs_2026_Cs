using Microsoft.EntityFrameworkCore;
using TransitoClaro.Api.Domain;

namespace TransitoClaro.Api.Data;

/// <summary>
/// Dados iniciais de exemplo (linhas e estações de São Paulo) para a API já subir com algo útil.
/// Conferir/ajustar com as fontes oficiais antes de qualquer uso real.
/// Nenhum dado pessoal é utilizado.
/// </summary>
public static class Semente
{
    public static void Aplicar(ModelBuilder mb)
    {
        mb.Entity<Linha>().HasData(
            new Linha { Id = 1, Nome = "Linha 1-Azul", Tipo = TipoTransporte.Metro, Operadora = "Metrô SP", Cor = "Azul" },
            new Linha { Id = 2, Nome = "Linha 2-Verde", Tipo = TipoTransporte.Metro, Operadora = "Metrô SP", Cor = "Verde" },
            new Linha { Id = 3, Nome = "Linha 3-Vermelha", Tipo = TipoTransporte.Metro, Operadora = "Metrô SP", Cor = "Vermelha" },
            new Linha { Id = 4, Nome = "Linha 11-Coral", Tipo = TipoTransporte.Trem, Operadora = "CPTM", Cor = "Coral" },
            new Linha { Id = 5, Nome = "Linha 9-Esmeralda", Tipo = TipoTransporte.Trem, Operadora = "ViaMobilidade", Cor = "Esmeralda" },
            new Linha { Id = 6, Nome = "Corredor Expresso Tiradentes", Tipo = TipoTransporte.Brt, Operadora = "SPTrans", Cor = null });

        mb.Entity<Estacao>().HasData(
            E(1, 1, "Jabaquara", 1), E(2, 1, "Paraíso", 2), E(3, 1, "Luz", 3), E(4, 1, "Tucuruvi", 4),
            E(5, 2, "Vila Madalena", 1), E(6, 2, "Consolação", 2), E(7, 2, "Paraíso", 3), E(8, 2, "Vila Prudente", 4),
            E(9, 3, "Palmeiras-Barra Funda", 1), E(10, 3, "Sé", 2), E(11, 3, "Tatuapé", 3), E(12, 3, "Corinthians-Itaquera", 4),
            E(13, 4, "Luz", 1), E(14, 4, "Brás", 2), E(15, 4, "Tatuapé", 3), E(16, 4, "Corinthians-Itaquera", 4),
            E(17, 5, "Osasco", 1), E(18, 5, "Pinheiros", 2), E(19, 5, "Santo Amaro", 3), E(20, 5, "Grajaú", 4),
            E(21, 6, "Terminal Parque Dom Pedro II", 1), E(22, 6, "Terminal Sacomã", 2));
    }

    private static Estacao E(int id, int linhaId, string nome, int ordem)
        => new() { Id = id, LinhaId = linhaId, Nome = nome, Ordem = ordem };
}
