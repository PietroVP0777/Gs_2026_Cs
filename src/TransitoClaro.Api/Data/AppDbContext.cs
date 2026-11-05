using Microsoft.EntityFrameworkCore;
using TransitoClaro.Api.Domain;

namespace TransitoClaro.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Linha> Linhas => Set<Linha>();
    public DbSet<Estacao> Estacoes => Set<Estacao>();
    public DbSet<RelatoCidadao> Relatos => Set<RelatoCidadao>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Linha>(e =>
        {
            e.ToTable("Linhas");
            e.Property(x => x.Nome).IsRequired().HasMaxLength(80);
            e.Property(x => x.Operadora).IsRequired().HasMaxLength(80);
            e.Property(x => x.Cor).HasMaxLength(30);
            e.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(20);
            e.HasIndex(x => x.Nome).IsUnique();
            e.HasMany(x => x.Estacoes).WithOne(x => x.Linha).HasForeignKey(x => x.LinhaId).OnDelete(DeleteBehavior.Cascade);
            e.HasMany(x => x.Relatos).WithOne(x => x.Linha).HasForeignKey(x => x.LinhaId).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Estacao>(e =>
        {
            e.ToTable("Estacoes");
            e.Property(x => x.Nome).IsRequired().HasMaxLength(80);
        });

        mb.Entity<RelatoCidadao>(e =>
        {
            e.ToTable("Relatos");
            e.Property(x => x.Tipo).HasConversion<string>().HasMaxLength(30);
            e.Property(x => x.NivelLotacao).HasConversion<string>().HasMaxLength(20);
            e.Property(x => x.Descricao).HasMaxLength(RelatoCidadao.MaxDescricao);
            e.HasOne(x => x.Estacao).WithMany().HasForeignKey(x => x.EstacaoId).OnDelete(DeleteBehavior.SetNull);
            e.HasIndex(x => new { x.LinhaId, x.CriadoEmUtc });
        });

        Semente.Aplicar(mb);
    }
}
