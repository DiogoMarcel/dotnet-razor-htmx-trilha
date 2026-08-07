// ============================================================================
// CalculadoraIcms — regras SIMPLIFICADAS. Não é a legislação real.
//
// A legislação real tem substituição tributária, DIFAL, redução de base,
// convênio por NCM, protocolo entre estados... Nada disso está aqui.
// O objetivo é praticar expressão switch e arredondamento, não tributar.
// ============================================================================

using FiscalLab.Domain;

namespace FiscalLab.Servicos;

public static class CalculadoraIcms
{
    // HashSet e não array: a busca é O(1) por hash, não O(n) varrendo tudo.
    // Com 7 elementos a diferença é irrelevante — mas o hábito certo é este,
    // e com a tabela real de NCM (milhares de linhas) deixa de ser detalhe.
    private static readonly HashSet<string> SulSudeste =
        new(StringComparer.OrdinalIgnoreCase) { "PR", "SC", "RS", "SP", "RJ", "MG", "ES" };
    // StringComparer.OrdinalIgnoreCase: "sp" acha "SP". Ordinal e não
    // CurrentCulture porque comparação de código de UF é byte a byte, não
    // depende de idioma — e comparação cultural é mais lenta e imprevisível.

    /// <summary>
    /// Calcula o ICMS de um item. Alíquotas: mesmo estado 18%,
    /// Sul/Sudeste -> N/NE/CO 7%, demais interestaduais 12%,
    /// Simples Nacional 0% (recolhe pelo DAS).
    /// </summary>
    public static ResultadoIcms Calcular(
        ItemNota item,
        string ufOrigem,
        string ufDestino,
        RegimeTributario regime)
    {
        ArgumentNullException.ThrowIfNull(item);

        string origem = NormalizarUf(ufOrigem, nameof(ufOrigem));
        string destino = NormalizarUf(ufDestino, nameof(ufDestino));

        // ---------------------------------------------------------------
        // EXPRESSÃO switch, não escada de if.
        //
        // A diferença não é estética: a expressão switch DEVOLVE um valor,
        // então o compilador exige que todo caminho produza um decimal.
        // Numa escada de if é possível esquecer um else e a variável fica
        // sem valor — ou pior, fica com o default (0% de imposto).
        //
        // A ORDEM importa: o primeiro padrão que casa ganha. Simples
        // Nacional vem primeiro porque zera tudo, independente das UFs.
        // ---------------------------------------------------------------
        decimal aliquota = (origem, destino) switch
        {
            // `_ when` : padrão de descarte com guarda. Ignora a tupla e
            // decide só pelo regime.
            _ when regime == RegimeTributario.SimplesNacional => 0.00m,

            // Padrão posicional desconstruindo a tupla em duas variáveis.
            (var o, var d) when o == d => 0.18m,

            (var o, var d) when SulSudeste.Contains(o) && !SulSudeste.Contains(d) => 0.07m,

            // Cai aqui: N/NE/CO -> qualquer, e Sul/Sudeste -> Sul/Sudeste.
            _ => 0.12m
        };

        // Base arredondada ANTES de multiplicar. Se arredondasse só no fim,
        // a memória de cálculo impressa (base x alíquota) não fecharia com o
        // valor — e auditoria fiscal confere isso na mão.
        decimal baseCalculo = Arredondar(item.Total);
        decimal valor = Arredondar(baseCalculo * aliquota);

        return new ResultadoIcms(baseCalculo, aliquota, valor);
    }

    // ------------------------------------------------------------------------
    // MidpointRounding.AwayFromZero, nunca o padrão.
    //
    // O padrão do .NET é ToEven (arredondamento bancário): 2,345 -> 2,34,
    // porque 4 é par. A Receita espera 2,35. Um centavo por item, vezes
    // 40.000 itens no mês, é rejeição de SPED.
    //
    // Isto NÃO é preferência de estilo. É requisito.
    // ------------------------------------------------------------------------
    private static decimal Arredondar(decimal valor) =>
        Math.Round(valor, 2, MidpointRounding.AwayFromZero);

    private static string NormalizarUf(string uf, string nomeParametro)
    {
        if (string.IsNullOrWhiteSpace(uf))
            throw new ArgumentException("UF é obrigatória", nomeParametro);

        string limpa = uf.Trim().ToUpperInvariant();
        // ToUpperInvariant e não ToUpper(): ToUpper usa a cultura da máquina.
        // Em turco, ToUpper('i') dá 'İ' — e "iso" viraria "İSO". Bug real,
        // conhecido como "Turkish I problem". Para código/identificador,
        // sempre a variante Invariant.

        if (limpa.Length != 2)
            throw new ArgumentException($"UF deve ter 2 letras, veio '{uf}'", nomeParametro);

        return limpa;
    }
}
