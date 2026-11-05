namespace TransitoClaro.Api.Domain;

/// <summary>Violação de regra de negócio (vira HTTP 400).</summary>
public class DomainException : Exception
{
    public DomainException(string mensagem) : base(mensagem) { }
}

/// <summary>Recurso inexistente (vira HTTP 404).</summary>
public class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem) { }
}
