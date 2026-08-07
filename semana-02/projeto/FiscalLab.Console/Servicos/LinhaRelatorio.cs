// ============================================================================
// LinhaRelatorio — uma linha do relatório por emitente.
//
// record, e não class: é dado de saída, comparado por valor, descartado
// depois de impresso. Não tem identidade nem ciclo de vida.
// ============================================================================

namespace FiscalLab.Servicos;

public record LinhaRelatorio(string Cnpj, int QuantidadeNotas, decimal ValorTotal)
{
    /// <summary>
    /// Ticket médio calculado, não armazenado — deriva dos outros dois campos.
    /// Guardar seria uma terceira fonte da mesma verdade.
    /// </summary>
    public decimal TicketMedio =>
        QuantidadeNotas == 0
            ? 0m
            // Divisão por zero em decimal LANÇA DivideByZeroException — não
            // devolve NaN nem Infinity como double faz. Mais uma razão para
            // decimal em contexto financeiro: o erro aparece, não se propaga.
            : Math.Round(ValorTotal / QuantidadeNotas, 2, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Se o CNPJ do arquivo passa no dígito verificador. Fica aqui e não no
    /// agrupamento porque é informação de EXIBIÇÃO — o relatório precisa
    /// mostrar o dado sujo marcado, não escondê-lo.
    /// </summary>
    public bool CnpjValido => ValidadorCnpj.EhValido(Cnpj);

    public string CnpjFormatado => ValidadorCnpj.Formatar(Cnpj);
}
