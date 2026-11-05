using TransitoClaro.Api.Domain;
using TransitoClaro.Api.Dtos;

namespace TransitoClaro.Api.Services;

public static class Mapeamentos
{
    public static LinhaDto ParaDto(this Linha l) => new()
    {
        Id = l.Id, Nome = l.Nome, Tipo = l.Tipo, Operadora = l.Operadora, Cor = l.Cor
    };

    public static EstacaoDto ParaDto(this Estacao e) => new()
    {
        Id = e.Id, Nome = e.Nome, Ordem = e.Ordem, LinhaId = e.LinhaId
    };
}
