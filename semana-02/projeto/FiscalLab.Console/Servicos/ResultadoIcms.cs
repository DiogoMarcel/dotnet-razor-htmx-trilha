// ============================================================================
// ResultadoIcms — record, porque é resultado de cálculo: dado puro.
//
// Por que não devolver só o decimal do valor: quem recebe precisa saber
// COM QUAL alíquota e sobre QUAL base aquele valor saiu. Auditoria fiscal
// pergunta exatamente isso. Devolver só o total joga a memória de cálculo
// no lixo, e ela é obrigatória no XML da NF-e.
// ============================================================================

namespace FiscalLab.Servicos;

/// <param name="BaseCalculo">Base do imposto, já arredondada a 2 casas.</param>
/// <param name="Aliquota">Fração, não percentual: 0.18m = 18%.</param>
/// <param name="Valor">BaseCalculo * Aliquota, arredondado a 2 casas.</param>
public record ResultadoIcms(decimal BaseCalculo, decimal Aliquota, decimal Valor)
{
    /// <summary>18% em vez de 0,18 — só para exibição.</summary>
    public decimal AliquotaPercentual => Aliquota * 100m;

    public override string ToString() =>
        $"base {BaseCalculo:N2} x {AliquotaPercentual:N0}% = {Valor:N2}";
}
