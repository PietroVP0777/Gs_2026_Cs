using Microsoft.EntityFrameworkCore;
using TransitoClaro.Api.Data;
using TransitoClaro.Api.Infra;

namespace TransitoClaro.Tests;

public class RelogioFalso : IRelogio
{
    public DateTime AgoraUtc { get; set; } = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);
}

public static class BancoDeTeste
{
    /// <summary>Cria um banco em memória isolado (já com os dados de semente).</summary>
    public static AppDbContext Criar()
    {
        var opcoes = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new AppDbContext(opcoes);
        db.Database.EnsureCreated();
        return db;
    }
}
