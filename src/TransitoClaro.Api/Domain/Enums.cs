namespace TransitoClaro.Api.Domain;

public enum TipoTransporte { Metro = 1, Trem = 2, Brt = 3 }

public enum TipoOcorrencia { Atraso = 1, Lotacao = 2, FalhaOperacional = 3 }

// A ordem importa: valores maiores = mais lotado
public enum NivelLotacao { Vazio = 1, Tranquilo = 2, Cheio = 3, Lotado = 4 }

public enum StatusLinha { Normal = 1, Moderado = 2, Critico = 3 }
