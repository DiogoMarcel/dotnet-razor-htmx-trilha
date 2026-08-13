// ============================================================================
// DEMO 3 — GroupBy, e os agregados que estouram com coleção vazia.
//
// O relatório por emitente da Semana 2 levou ~40 linhas: um Dictionary, um
// laço de acumulação, um segundo laço para ordenar. Aqui são 8 linhas.
//
// Mas o motivo de esta demo existir não é o encolhimento. É que TRÊS dos
// operadores mais usados jogam exceção com sequência vazia, e nenhum deles
// avisa em tempo de compilação. Em relatório fiscal, "nenhuma nota no
// período" não é caso raro — é o dia 1º de cada mês.
// ============================================================================

using Semana03.Dominio;

namespace Semana03.Demos;

// O DTO do relatório. `record` porque é dado de saída: nasce pronto,
// ninguém muda, e o ToString/igualdade de graça ajudam no teste.
public record LinhaRelatorio(
    string Cnpj,
    string RazaoSocial,
    int Quantidade,
    decimal Total,
    decimal Maior);

public static class Demo3Agrupamento
{
    public static void Executar()
    {
        var notas = Massa.Notas();

        Console.WriteLine("  Relatório: total AUTORIZADO por emitente, maior primeiro.");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: quantas linhas o relatório tem, e qual emitente");
        Console.WriteLine("  >>> aparece em primeiro?");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // O relatório inteiro. Leia como frase.
        // ---------------------------------------------------------------
        var relatorio = notas
            .Where(n => n.Situacao == SituacaoNota.Autorizada)
            .GroupBy(n => n.Cnpj)                       // chave -> grupo
            .Select(g => new LinhaRelatorio(
                Cnpj: g.Key,                            // g.Key é o valor agrupado
                RazaoSocial: g.First().RazaoSocial,     // todas do grupo têm a mesma
                Quantidade: g.Count(),
                Total: g.Sum(n => n.Valor),
                Maior: g.Max(n => n.Valor)))
            .OrderByDescending(l => l.Total)
            .ToList();

        Console.WriteLine($"  {"CNPJ",-16} {"Razão social",-24} {"Qtd",4} {"Total",12} {"Maior",12}");
        Console.WriteLine("  " + new string('-', 72));

        foreach (var l in relatorio)
            Console.WriteLine($"  {l.Cnpj,-16} {l.RazaoSocial,-24} {l.Quantidade,4} " +
                              $"{l.Total,12:N2} {l.Maior,12:N2}");

        Console.WriteLine("  " + new string('-', 72));
        Console.WriteLine($"  {"TOTAL GERAL",-46} {relatorio.Sum(l => l.Total),12:N2}");
        Console.WriteLine();

        Console.WriteLine("  Um GroupBy devolve uma sequência de GRUPOS, e cada grupo é ele");
        Console.WriteLine("  próprio uma sequência — com uma propriedade `Key` colada nele.");
        Console.WriteLine("  É por isso que dá para chamar g.Sum(), g.Count(), g.First():");
        Console.WriteLine("  o grupo É um IEnumerable<NotaFiscal>.");
        Console.WriteLine();
        Console.WriteLine("  Delphi: você faria com TDictionary<string, TList<TNota>> e dois");
        Console.WriteLine("  laços. É a mesma estrutura — o GroupBy monta esse dicionário");
        Console.WriteLine("  para você e devolve já percorrível.");
        Console.WriteLine();

        MostrarAgregadosPerigosos();
        MostrarFamiliaFirstSingle();
    }

    // ------------------------------------------------------------------------
    // O que estoura, o que devolve zero, e por que a diferença é proposital.
    // ------------------------------------------------------------------------
    private static void MostrarAgregadosPerigosos()
    {
        Console.WriteLine("  ────────────────────────────────────────────────────────────────");
        Console.WriteLine("  SEQUÊNCIA VAZIA — o mês que ainda não teve nota");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: das 4 chamadas abaixo, sobre uma lista VAZIA,");
        Console.WriteLine("  >>> quais devolvem valor e quais jogam exceção?");
        Console.WriteLine();

        var vazia = new List<NotaFiscal>();

        Tentar("Count()",            () => vazia.Count().ToString());
        Tentar("Sum(n => n.Valor)",  () => vazia.Sum(n => n.Valor).ToString("N2"));
        Tentar("Max(n => n.Valor)",  () => vazia.Max(n => n.Valor).ToString("N2"));
        Tentar("Average(n => ...)",  () => vazia.Average(n => n.Valor).ToString("N2"));

        Console.WriteLine();
        Console.WriteLine("  A lógica, e ela NÃO é arbitrária:");
        Console.WriteLine("      Sum   -> 0 é a resposta matematicamente correta para");
        Console.WriteLine("               'some nada'. Existe elemento neutro.");
        Console.WriteLine("      Max   -> 'o maior de nenhum' não tem resposta. Não existe");
        Console.WriteLine("               neutro. Devolver 0 seria MENTIR.");
        Console.WriteLine("      Average -> divisão por zero, mesma coisa.");
        Console.WriteLine();
        Console.WriteLine("  E é aqui que a escolha vira decisão fiscal, não técnica:");
        Console.WriteLine("      DefaultIfEmpty(0m).Max()  -> devolve 0 e some com o problema");
        Console.WriteLine("      MaxBy(...)                -> devolve null, e você DECIDE");
        Console.WriteLine("      if (!notas.Any()) return  -> a tela diz 'sem movimento'");
        Console.WriteLine();
        Console.WriteLine("  A primeira é a que a IA vai te sugerir, porque faz o erro sumir.");
        Console.WriteLine("  Num relatório de apuração, 'maior nota do mês: R$ 0,00' e");
        Console.WriteLine("  'não houve movimento' são fatos DIFERENTES. Recuse a primeira.");
        Console.WriteLine();
    }

    // ------------------------------------------------------------------------
    // First / Single / FirstOrDefault / SingleOrDefault — a matriz que
    // quase todo mundo usa errado, inclusive código gerado por IA.
    // ------------------------------------------------------------------------
    private static void MostrarFamiliaFirstSingle()
    {
        Console.WriteLine("  ────────────────────────────────────────────────────────────────");
        Console.WriteLine("  FIRST vs SINGLE — a escolha diz o que você ACREDITA sobre o dado");
        Console.WriteLine();

        var notas = Massa.Notas();

        // "SP tem 4 notas" — First pega uma; Single reclama.
        Tentar("Where(UF=SP).First()",
               () => $"NF {notas.Where(n => n.UfDestino == "SP").First().Numero}");

        Tentar("Where(UF=SP).Single()",
               () => $"NF {notas.Where(n => n.UfDestino == "SP").Single().Numero}");

        // "Nenhuma nota do AC" — os OrDefault devolvem null.
        Tentar("Where(UF=AC).First()",
               () => $"NF {notas.Where(n => n.UfDestino == "AC").First().Numero}");

        Tentar("Where(UF=AC).FirstOrDefault()",
               () => notas.Where(n => n.UfDestino == "AC").FirstOrDefault() is { } nf
                     ? $"NF {nf.Numero}" : "null");

        Console.WriteLine();
        Console.WriteLine("  A matriz inteira:");
        Console.WriteLine();
        Console.WriteLine("                       0 itens        1 item      2+ itens");
        Console.WriteLine("      First             ESTOURA        devolve     devolve o 1º");
        Console.WriteLine("      FirstOrDefault    null           devolve     devolve o 1º");
        Console.WriteLine("      Single            ESTOURA        devolve     ESTOURA");
        Console.WriteLine("      SingleOrDefault   null           devolve     ESTOURA");
        Console.WriteLine();
        Console.WriteLine("  Como escolher, e é uma pergunta sobre o DOMÍNIO:");
        Console.WriteLine();
        Console.WriteLine("      'busca por chave única (CNPJ, ID)'   -> Single/SingleOrDefault.");
        Console.WriteLine("       Se vier 2, o banco está corrompido e você QUER saber agora.");
        Console.WriteLine();
        Console.WriteLine("      'a primeira da lista ordenada'       -> First/FirstOrDefault.");
        Console.WriteLine("       Ter várias é normal e esperado.");
        Console.WriteLine();
        Console.WriteLine("  O erro caro é usar First numa busca por chave única: o dia em que");
        Console.WriteLine("  dois cadastros duplicarem o CNPJ, o sistema escolhe um em silêncio");
        Console.WriteLine("  e emite a nota no CNPJ errado. Single teria parado o processo.");
        Console.WriteLine();
        Console.WriteLine("  ISTO É PARA COBRAR DA IA. Ela usa `.First()` por padrão, porque");
        Console.WriteLine("  `.First()` nunca reclama de dado sujo. Você quer que reclame.");
    }

    // Executa e mostra o resultado OU o nome da exceção. Sem try/catch aqui
    // a demo morreria na terceira linha.
    private static void Tentar(string rotulo, Func<string> acao)
    {
        try
        {
            Console.WriteLine($"      {rotulo,-32} -> {acao()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"      {rotulo,-32} -> {ex.GetType().Name}: {ex.Message}");
        }
    }
}
