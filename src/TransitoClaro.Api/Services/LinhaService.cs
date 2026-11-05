using Microsoft.EntityFrameworkCore;
using TransitoClaro.Api.Data;
using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;

namespace TransitoClaro.Api.Services;

public class LinhaService : ILinhaService
{
    private readonly AppDbContext _db;

    public LinhaService(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<LinhaDto>> ListarAsync(TipoTransporte? tipo, CancellationToken ct = default)
    {
        var consulta = _db.Linhas.AsNoTracking().AsQueryable();
        if (tipo.HasValue)
            consulta = consulta.Where(l => l.Tipo == tipo.Value);

        var linhas = await consulta.OrderBy(l => l.Tipo).ThenBy(l => l.Nome).ToListAsync(ct);
        return linhas.Select(l => l.ParaDto()).ToList();
    }

    public async Task<LinhaDto> ObterAsync(int id, CancellationToken ct = default)
        => (await BuscarAsync(id, ct)).ParaDto();

    public async Task<IReadOnlyList<EstacaoDto>> ListarEstacoesAsync(int linhaId, CancellationToken ct = default)
    {
        if (!await _db.Linhas.AnyAsync(l => l.Id == linhaId, ct))
            throw new NaoEncontradoException($"Linha {linhaId} não encontrada.");

        var estacoes = await _db.Estacoes.AsNoTracking()
            .Where(e => e.LinhaId == linhaId)
            .OrderBy(e => e.Ordem)
            .ToListAsync(ct);

        return estacoes.Select(e => e.ParaDto()).ToList();
    }

    public async Task<LinhaDto> CriarAsync(LinhaSalvarDto dto, CancellationToken ct = default)
    {
        await GarantirNomeUnicoAsync(dto.Nome, null, ct);

        var linha = new Linha
        {
            Nome = dto.Nome.Trim(),
            Tipo = dto.Tipo!.Value,
            Operadora = dto.Operadora.Trim(),
            Cor = dto.Cor?.Trim()
        };
        _db.Linhas.Add(linha);
        await _db.SaveChangesAsync(ct);
        return linha.ParaDto();
    }

    public async Task<LinhaDto> AtualizarAsync(int id, LinhaSalvarDto dto, CancellationToken ct = default)
    {
        var linha = await BuscarAsync(id, ct);
        await GarantirNomeUnicoAsync(dto.Nome, id, ct);

        linha.Nome = dto.Nome.Trim();
        linha.Tipo = dto.Tipo!.Value;
        linha.Operadora = dto.Operadora.Trim();
        linha.Cor = dto.Cor?.Trim();
        await _db.SaveChangesAsync(ct);
        return linha.ParaDto();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var linha = await BuscarAsync(id, ct);
        _db.Linhas.Remove(linha);
        await _db.SaveChangesAsync(ct);
    }

    private async Task<Linha> BuscarAsync(int id, CancellationToken ct)
        => await _db.Linhas.FirstOrDefaultAsync(l => l.Id == id, ct)
           ?? throw new NaoEncontradoException($"Linha {id} não encontrada.");

    private async Task GarantirNomeUnicoAsync(string nome, int? ignorarId, CancellationToken ct)
    {
        var nomeLimpo = nome.Trim();
        bool existe = await _db.Linhas.AnyAsync(l => l.Nome == nomeLimpo && l.Id != ignorarId, ct);
        if (existe)
            throw new DomainException($"Já existe uma linha chamada '{nomeLimpo}'.");
    }
}
