// ============================================================================
// DÍVIDA 1 — INVERSÃO DE CONTROLE (Semana 2, Q11)
//
// Você já sabe: (a, b) => ... é uma lambda, o tipo é um delegate, e o
// equivalente Delphi é `reference to function`. Isso está fechado.
//
// O que faltou: POR QUE um método aceita código como parâmetro.
//
// Esta demo responde isso mostrando, não explicando. Eu escrevi UM algoritmo
// de ordenação (Ordenar, abaixo) e vou usá-lo três vezes com resultados
// completamente diferentes — sem alterar uma linha dele.
// ============================================================================

namespace Quitacao.Demos;

public record Nota(int Numero, string Cnpj, decimal Valor);

public static class Demo1InversaoDeControle
{
    // ------------------------------------------------------------------------
    // O ALGORITMO. Escrito uma vez.
    //
    // Repare no que ele NÃO faz: não menciona Nota, não menciona Valor, não
    // sabe o que é "maior". Ele só sabe TROCAR e PERCORRER.
    //
    // É um bubble sort de propósito — o algoritmo real não importa aqui, o que
    // importa é onde entra a decisão.
    // ------------------------------------------------------------------------
    private static void Ordenar<T>(List<T> lista, Comparison<T> comparar)
    {
        for (int i = 0; i < lista.Count - 1; i++)
            for (int j = 0; j < lista.Count - 1 - i; j++)
                // ESTA linha é a única que decide ordem. E ela não decide nada:
                // ela PERGUNTA. Quem responde é o código que veio de fora.
                if (comparar(lista[j], lista[j + 1]) > 0)
                    (lista[j], lista[j + 1]) = (lista[j + 1], lista[j]);
    }
    // (a, b) = (b, a) é troca por tupla. Sem variável temporária.

    public static void Executar()
    {
        Console.WriteLine("DÍVIDA 1 — inversão de controle");
        Console.WriteLine(new string('-', 70));
        Console.WriteLine();
        Console.WriteLine("Massa (na ordem em que foi criada):");

        var original = new List<Nota>
        {
            new(131, "11222333000181",   95.90m),
            new(1042, "45612378000105", 38910.50m),
            new(37, "11222333000181",  2075.00m),
            new(6,  "33445566000199",   420.00m),
        };

        foreach (var n in original)
            Console.WriteLine($"    NF {n.Numero,-5} {n.Cnpj}  {n.Valor,10:N2}");

        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA a ordem dos NÚMEROS DE NOTA em cada caso,");
        Console.WriteLine("  >>> ANTES de olhar o resultado. Escreva em PREVISOES.md.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // MESMO Ordenar. Três critérios. Três resultados.
        // ---------------------------------------------------------------

        var a = new List<Nota>(original);
        Ordenar(a, (x, y) => x.Valor.CompareTo(y.Valor));
        Mostrar("(a) valor crescente        (x, y) => x.Valor.CompareTo(y.Valor)", a);

        var b = new List<Nota>(original);
        Ordenar(b, (x, y) => y.Valor.CompareTo(x.Valor));
        Mostrar("(b) valor DECRESCENTE      (x, y) => y.Valor.CompareTo(x.Valor)", b);

        var c = new List<Nota>(original);
        Ordenar(c, (x, y) => x.Numero.CompareTo(y.Numero));
        Mostrar("(c) número crescente       (x, y) => x.Numero.CompareTo(y.Numero)", c);

        // Critério COMPOSTO: CNPJ, e dentro do mesmo CNPJ, valor decrescente.
        // Note que o algoritmo continua sem saber que isso existe.
        var d = new List<Nota>(original);
        Ordenar(d, (x, y) =>
        {
            int porCnpj = string.CompareOrdinal(x.Cnpj, y.Cnpj);
            return porCnpj != 0 ? porCnpj : y.Valor.CompareTo(x.Valor);
        });
        Mostrar("(d) CNPJ, depois valor desc  — critério COMPOSTO", d);

        Console.WriteLine();
        Console.WriteLine("  O MÉTODO Ordenar NÃO FOI ALTERADO entre (a), (b), (c) e (d).");
        Console.WriteLine("  Uma implementação. Quatro comportamentos.");
        Console.WriteLine();
        Console.WriteLine("  É ISSO a resposta de 'por que um método aceita código':");
        Console.WriteLine("    Ordenar implementa o que NÃO VARIA (percorrer e trocar).");
        Console.WriteLine("    Você fornece o que VARIA (o que significa 'maior').");
        Console.WriteLine("    Ordenar chama o SEU código. Inversão de controle.");
        Console.WriteLine();
        Console.WriteLine("  As alternativas seriam piores:");
        Console.WriteLine("    - Nota implementar IComparable: fixa UMA ordem no tipo.");
        Console.WriteLine("      O caso (d) ficaria impossível sem criar outro tipo.");
        Console.WriteLine("    - Escrever 4 versões de Ordenar: 4 vezes o mesmo bug de índice.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // O delegate é um VALOR. Prova.
        //
        // Na prova você escreveu "não é um valor de fato". É. Se não fosse,
        // as três linhas abaixo não compilariam.
        // ---------------------------------------------------------------
        Console.WriteLine("  Delegate é um VALOR — guardável, passável, devolvível:");

        Comparison<Nota> porValorDesc = (x, y) => y.Valor.CompareTo(x.Valor);
        Comparison<Nota> porNumero = (x, y) => x.Numero.CompareTo(y.Numero);

        // Uma LISTA de critérios. Se lambda não fosse valor, isto seria impossível.
        var criterios = new List<(string Nome, Comparison<Nota> Criterio)>
        {
            ("valor desc", porValorDesc),
            ("número asc", porNumero),
        };

        foreach (var (nome, criterio) in criterios)
        {
            var copia = new List<Nota>(original);
            Ordenar(copia, criterio);
            Console.WriteLine($"    {nome,-12} -> {string.Join(", ", copia.Select(n => n.Numero))}");
        }

        Console.WriteLine();
        Console.WriteLine("  E um método pode DEVOLVER um delegate — aqui, um critério");
        Console.WriteLine("  construído em tempo de execução a partir de um parâmetro:");

        foreach (bool desc in new[] { false, true })
        {
            var copia = new List<Nota>(original);
            Ordenar(copia, CriarCriterioValor(desc));
            Console.WriteLine($"    desc={desc,-5} -> {string.Join(", ", copia.Select(n => n.Numero))}");
        }

        Console.WriteLine();
        Console.WriteLine("  Na Semana 3, LINQ é esta mesma ideia com açúcar sintático:");
        Console.WriteLine("    Where(x => ...)   recebe Func<T, bool>");
        Console.WriteLine("    Select(x => ...)  recebe Func<T, TResult>");
        Console.WriteLine("    Sum(x => ...)     recebe Func<T, decimal>");
        Console.WriteLine("  Método genérico com a mecânica + critério como delegate.");
    }

    // Devolve um delegate. O `desc` fica CAPTURADO dentro dele — é isso que
    // um ponteiro de função puro não conseguiria fazer, e por isso delegate
    // é um objeto (método + alvo + variáveis capturadas), não só um endereço.
    private static Comparison<Nota> CriarCriterioValor(bool descendente) =>
        descendente
            ? (x, y) => y.Valor.CompareTo(x.Valor)
            : (x, y) => x.Valor.CompareTo(y.Valor);

    private static void Mostrar(string titulo, List<Nota> lista)
    {
        Console.WriteLine($"  {titulo}");
        Console.WriteLine($"      -> {string.Join(", ", lista.Select(n => n.Numero))}");
    }
}
