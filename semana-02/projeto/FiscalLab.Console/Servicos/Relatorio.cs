// ============================================================================
// Relatorio — agrupamento por emitente, SEM LINQ.
//
// O ponto desta aula é o Dictionary. A alternativa ingênua seria:
//
//     foreach (var cnpj in cnpjsDistintos)          // n
//         foreach (var nota in notas)               // n
//             if (nota.Cnpj == cnpj) ...
//
// Isso é O(n²): 10.000 notas dão 100 milhões de comparações. Com Dictionary
// é O(n) — uma passada, e cada busca por chave é tempo constante por hash.
// Em Delphi o equivalente é TDictionary<string, T>; a ideia é a mesma.
//
// Na Semana 3 este arquivo vira ~6 linhas com GroupBy. Escrever na mão
// primeiro é o que impede o LINQ de virar mágica.
// ============================================================================

using System.Globalization;

namespace FiscalLab.Servicos;

public static class Relatorio
{
    /// <summary>
    /// Agrupa notas por CNPJ do emitente. Uma passada na lista.
    /// </summary>
    public static List<LinhaRelatorio> AgruparPorEmitente(IReadOnlyList<NotaFiscalCsv> notas)
    {
        ArgumentNullException.ThrowIfNull(notas);

        // Tupla nomeada como valor: (int Quantidade, decimal Total).
        // É um ValueTuple — struct, ou seja, TIPO POR VALOR. Consequência
        // direta: `out var acumulado` devolve uma CÓPIA. Alterar a cópia não
        // altera o dicionário, por isso a reatribuição explícita abaixo.
        // Se fosse uma class, bastaria acumulado.Total += ... e pronto.
        // É valor vs referência aparecendo em código real, não em slide.
        var mapa = new Dictionary<string, (int Quantidade, decimal Total)>();

        foreach (var nota in notas)
        {
            if (mapa.TryGetValue(nota.CnpjEmitente, out var acumulado))
                mapa[nota.CnpjEmitente] = (acumulado.Quantidade + 1, acumulado.Total + nota.Valor);
            else
                mapa[nota.CnpjEmitente] = (1, nota.Valor);
        }

        var linhas = new List<LinhaRelatorio>(mapa.Count);
        // Capacidade inicial informada: evita o dobro-e-copia interno da List
        // quando já se sabe o tamanho final.

        foreach (var par in mapa)
            linhas.Add(new LinhaRelatorio(par.Key, par.Value.Quantidade, par.Value.Total));

        // A ordem de iteração de um Dictionary NÃO É GARANTIDA. Nunca dependa
        // dela. Quem quiser ordem chama OrdenarPorValorDesc.
        return linhas;
    }

    /// <summary>
    /// Exercício 6: ordena por valor total, decrescente. Muta a lista recebida.
    /// </summary>
    public static void OrdenarPorValorDesc(List<LinhaRelatorio> linhas)
    {
        ArgumentNullException.ThrowIfNull(linhas);

        // ------------------------------------------------------------------
        // O (a, b) => ... é uma EXPRESSÃO LAMBDA, e o tipo dela aqui é
        // Comparison<LinhaRelatorio> — um DELEGATE, isto é, um tipo cujo
        // valor é "um método". O equivalente exato do `type TComparison =
        // reference to function(...)` do Delphi.
        //
        // Sort não sabe comparar LinhaRelatorio, e não tem como saber: só
        // você sabe se o relatório ordena por valor, por CNPJ ou por data.
        // Então Sort implementa o ALGORITMO (quicksort) e recebe o CRITÉRIO
        // como parâmetro. Isso é inversão de controle.
        //
        // b.CompareTo(a) e não a.CompareTo(b): trocar a ordem dos operandos
        // inverte o sinal do resultado, e é assim que se faz decrescente.
        // CompareTo devolve <0, 0 ou >0 — nunca compare com -1 ou 1.
        //
        // Todo o LINQ é isso: OrderBy, Where e Select são métodos que recebem
        // delegates. Se este parágrafo fez sentido, a Semana 3 é fácil.
        // ------------------------------------------------------------------
        linhas.Sort((a, b) => b.ValorTotal.CompareTo(a.ValorTotal));
    }

    /// <summary>
    /// Imprime o relatório formatado. Marca com * o CNPJ que não passa no DV.
    /// </summary>
    public static void Imprimir(IReadOnlyList<LinhaRelatorio> linhas, CultureInfo cultura)
    {
        ArgumentNullException.ThrowIfNull(linhas);

        // {texto,-20} alinha à esquerda em 20 colunas. {valor,15:N2} alinha à
        // direita em 15 e formata com 2 casas e separador de milhar.
        // Número alinha à direita SEMPRE: é o que permite comparar magnitude
        // batendo o olho na coluna.
        Console.WriteLine(string.Format(cultura,
            "{0,-20} {1,7} {2,15} {3,15}",
            "CNPJ", "Notas", "Valor total", "Ticket médio"));

        Console.WriteLine(new string('-', 60));

        int totalNotas = 0;
        decimal totalValor = 0m;
        bool temCnpjInvalido = false;

        foreach (var linha in linhas)
        {
            string marca = linha.CnpjValido ? " " : "*";

            Console.WriteLine(string.Format(cultura,
                "{0,-19}{1} {2,7} {3,15:N2} {4,15:N2}",
                linha.CnpjFormatado,
                marca,
                linha.QuantidadeNotas,
                linha.ValorTotal,
                linha.TicketMedio));

            totalNotas += linha.QuantidadeNotas;
            totalValor += linha.ValorTotal;

            if (!linha.CnpjValido)
                temCnpjInvalido = true;
        }

        Console.WriteLine(new string('-', 60));

        // Ticket médio geral: total / total de notas. NÃO é a média dos
        // tickets médios das linhas — média de médias só coincide se todos os
        // grupos tiverem o mesmo tamanho. Erro clássico de relatório.
        decimal ticketGeral = totalNotas == 0
            ? 0m
            : Math.Round(totalValor / totalNotas, 2, MidpointRounding.AwayFromZero);

        Console.WriteLine(string.Format(cultura,
            "{0,-20} {1,7} {2,15:N2} {3,15:N2}",
            "TOTAL", totalNotas, totalValor, ticketGeral));

        if (temCnpjInvalido)
        {
            Console.WriteLine();
            Console.WriteLine("* CNPJ reprovado no dígito verificador.");
        }
    }
}
