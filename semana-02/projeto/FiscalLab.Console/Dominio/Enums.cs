// ============================================================================
// Enums do domínio fiscal.
//
// EXCEÇÃO CONSCIENTE à regra "um tipo público por arquivo": enum é um
// conjunto fechado de constantes, não tem comportamento, e o GUIA-PROJETO
// pede explicitamente um arquivo Enums.cs. Se um destes enums crescer e
// ganhar métodos de extensão, aí sim ele merece arquivo próprio.
// ============================================================================

namespace FiscalLab.Domain;

/// <summary>
/// Regime tributário do emitente. Os números são explícitos porque vão para
/// o banco e para o campo CRT do XML da NF-e. Reordenar um enum sem número
/// explícito muda o significado dos dados já gravados, silenciosamente.
/// </summary>
public enum RegimeTributario
{
    SimplesNacional = 1,
    LucroPresumido = 2,
    LucroReal = 3
}

/// <summary>
/// Situação da nota. É uma máquina de estados: EmDigitacao -> Autorizada ->
/// Cancelada. As transições válidas estão em <see cref="NotaFiscal"/>, não aqui —
/// o enum guarda o estado, a entidade guarda as regras de mudança de estado.
/// </summary>
public enum SituacaoNota
{
    EmDigitacao = 0,
    Autorizada = 1,
    Cancelada = 2
}
