// ============================================================================
// DEMO 2 — EXECUÇÃO ADIADA. É o susto da semana.
//
// Em Delphi, quando você chama uma função que filtra uma lista, ela filtra.
// Ali, naquela linha. Você tem a lista filtrada na mão e acabou.
//
// LINQ não faz isso. `Where` não filtra nada — ele MONTA uma pergunta e
// devolve o objeto que sabe respondê-la. A resposta só é calculada quando
// alguém percorre. Toda vez que alguém percorrer.
//
// Consequências, e as três mordem em produção:
//   1. o filtro roda ZERO vezes se ninguém iterar
//   2. o filtro roda DE NOVO a cada iteração — duas vezes = trabalho dobrado
//   3. a consulta enxerga a fonte NO MOMENTO DA ITERAÇÃO, não no da escrita
//
// A (3) é a que estraga dado fiscal em silêncio.
//
// Para tornar isso visível eu instrumentei o filtro: cada vez que a lambda
// é avaliada, um contador sobe. O contador não mente.
// ============================================================================

using Semana03.Dominio;

namespace Semana03.Demos;

public static class Demo2ExecucaoAdiada
{
    private static int _avaliacoes;

    public static void Executar()
    {
        var notas = Massa.Notas();

        Console.WriteLine("  Toda vez que a lambda do Where for avaliada, um contador sobe.");
        Console.WriteLine("  A massa tem 12 notas.");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA cada contador ANTES de olhar. São 5 números.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // CENA 1 — a query é escrita. Quantas avaliações?
        // ---------------------------------------------------------------
        _avaliacoes = 0;

        var autorizadas = notas.Where(Contando);
        // ^ repare no tipo: NÃO é List<NotaFiscal>. É IEnumerable<NotaFiscal>,
        //   e por baixo é uma máquina de estados que ainda não rodou.

        Console.WriteLine($"  (1) query escrita, ninguém iterou   -> {_avaliacoes} avaliações");
        Console.WriteLine($"      tipo estático (o que o `var` virou): IEnumerable<NotaFiscal>");
        Console.WriteLine($"      tipo em execução (o objeto real)   : {autorizadas.GetType().Name}");
        Console.WriteLine("      Nome feio de propósito: é uma classe interna do .NET que");
        Console.WriteLine("      guarda a fonte e o filtro, e não roda até pedirem.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // CENA 2 — um foreach.
        // ---------------------------------------------------------------
        int quantas = 0;
        foreach (var _ in autorizadas)
            quantas++;

        Console.WriteLine($"  (2) depois de UM foreach           -> {_avaliacoes} avaliações  ({quantas} passaram)");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // CENA 3 — a MESMA variável, percorrida de novo.
        //
        // Isto é o padrão mais caro que existe em código de linha de negócio,
        // e ele parece inofensivo: `if (query.Any())` seguido de
        // `foreach (var x in query)` percorre DUAS vezes.
        // ---------------------------------------------------------------
        foreach (var _ in autorizadas)
            quantas++;

        Console.WriteLine($"  (3) depois do SEGUNDO foreach      -> {_avaliacoes} avaliações");
        Console.WriteLine("      A mesma variável. Nenhuma linha nova de filtro.");
        Console.WriteLine("      Se cada avaliação fosse uma ida ao banco, você dobrou a conta.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // CENA 4 — .ToList() materializa. Vira dado, deixa de ser pergunta.
        // ---------------------------------------------------------------
        _avaliacoes = 0;
        var materializada = notas.Where(Contando).ToList();
        int aposToList = _avaliacoes;

        foreach (var _ in materializada) { }
        foreach (var _ in materializada) { }

        Console.WriteLine($"  (4) .ToList() + DOIS foreach       -> {_avaliacoes} avaliações");
        Console.WriteLine($"      (o ToList sozinho já custou {aposToList}; os foreach custaram 0)");
        Console.WriteLine("      ToList EXECUTA e guarda. Depois disso é uma lista comum.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // CENA 5 — curto-circuito. Aqui LINQ faz MENOS trabalho que o laço.
        //
        // A execução adiada não é só custo. `First` para no primeiro que
        // serve — não filtra as 12 para depois pegar a primeira.
        // ---------------------------------------------------------------
        _avaliacoes = 0;
        var primeira = notas.Where(Contando).First();

        Console.WriteLine($"  (5) .Where(...).First()            -> {_avaliacoes} avaliações");
        Console.WriteLine($"      achou a NF {primeira.Numero} e PAROU. Não avaliou as 12.");
        Console.WriteLine("      Um laço `for` ingênuo teria filtrado tudo antes de pegar a 1ª.");
        Console.WriteLine();

        MostrarAFonteQueMudou();
        MostrarOsTresGrupos();
    }

    // A lambda instrumentada. Fora `_avaliacoes++`, é um filtro comum.
    private static bool Contando(NotaFiscal n)
    {
        _avaliacoes++;
        return n.Situacao == SituacaoNota.Autorizada;
    }

    // ------------------------------------------------------------------------
    // A CONSEQUÊNCIA QUE ESTRAGA DADO — a consulta vê a fonte de HOJE.
    // ------------------------------------------------------------------------
    private static void MostrarAFonteQueMudou()
    {
        Console.WriteLine("  ────────────────────────────────────────────────────────────────");
        Console.WriteLine("  A FONTE MUDOU DEPOIS DA CONSULTA. Quem ganha?");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: a consulta foi escrita ANTES de a nota 9999 entrar");
        Console.WriteLine("  >>> na lista. Ela aparece no resultado?");
        Console.WriteLine();

        var notas = Massa.Notas();

        // Consulta escrita AGORA, com 12 notas na fonte.
        var acimaDe10Mil = notas.Where(n => n.Valor > 10_000m);
        var snapshot = notas.Where(n => n.Valor > 10_000m).ToList();

        Console.WriteLine($"      no momento da escrita, a fonte tem {notas.Count} notas");

        // O sistema segue rodando. Alguém importa mais uma nota.
        notas.Add(new NotaFiscal(9999, "11222333000181", "Metalúrgica Aurora",
                                 "SP", new DateTime(2026, 07, 31), 99_000.00m,
                                 SituacaoNota.Autorizada));

        Console.WriteLine($"      alguém importou a NF 9999; agora tem {notas.Count}");
        Console.WriteLine();
        Console.WriteLine($"      consulta adiada (IEnumerable) -> {Massa.Numeros(acimaDe10Mil)}");
        Console.WriteLine($"      snapshot (.ToList() antes)    -> {Massa.Numeros(snapshot)}");
        Console.WriteLine();
        Console.WriteLine("      MESMO filtro, MESMA fonte, resultados diferentes.");
        Console.WriteLine("      A diferença é UMA chamada de .ToList().");
        Console.WriteLine();
        Console.WriteLine("      Em relatório fiscal isso é o total do rodapé não bater com");
        Console.WriteLine("      a soma das linhas — porque as linhas foram enumeradas num");
        Console.WriteLine("      instante e o total noutro. Ninguém vê erro; vê número errado.");
        Console.WriteLine();

        // E o caso extremo: a fonte esvaziou.
        var contagem = notas.Where(n => n.Situacao == SituacaoNota.Autorizada);
        notas.Clear();

        Console.WriteLine($"      lista limpa DEPOIS da consulta -> {contagem.Count()} resultados");
        Console.WriteLine("      A consulta não guardou nada. Ela só sabe PERGUNTAR à lista,");
        Console.WriteLine("      e a lista de agora está vazia.");
        Console.WriteLine();
    }

    // ------------------------------------------------------------------------
    // A régua que resolve 90% das decisões: o operador ADIA ou EXECUTA?
    // ------------------------------------------------------------------------
    private static void MostrarOsTresGrupos()
    {
        Console.WriteLine("  ────────────────────────────────────────────────────────────────");
        Console.WriteLine("  A RÉGUA — olhe o TIPO DE RETORNO e você sabe:");
        Console.WriteLine();
        Console.WriteLine("   ADIAM (devolvem IEnumerable<T>, não rodam nada):");
        Console.WriteLine("      Where · Select · OrderBy · OrderByDescending · GroupBy");
        Console.WriteLine("      Take · Skip · Distinct · Reverse · SelectMany");
        Console.WriteLine();
        Console.WriteLine("   EXECUTAM NA HORA (devolvem valor ou coleção concreta):");
        Console.WriteLine("      ToList · ToArray · ToDictionary · ToHashSet");
        Console.WriteLine("      Count · Sum · Min · Max · Average · Aggregate");
        Console.WriteLine("      First · Single · Last · Any · All · Contains · ElementAt");
        Console.WriteLine();
        Console.WriteLine("   A regra em uma frase: se o retorno AINDA É IEnumerable<T>,");
        Console.WriteLine("   nada rodou. Se é um número, um objeto ou uma List, rodou.");
        Console.WriteLine();
        Console.WriteLine("   E a decisão prática, que é o que você vai exigir da IA:");
        Console.WriteLine("      vai percorrer mais de uma vez?      -> .ToList() e acabou");
        Console.WriteLine("      vai devolver de um método público?  -> .ToList(), senão o");
        Console.WriteLine("                                             chamador executa sem saber");
        Console.WriteLine("      é um passo intermediário de cadeia? -> deixe adiado");
        Console.WriteLine();
        Console.WriteLine("   Na Semana 7 isto deixa de ser sobre memória e passa a ser sobre");
        Console.WriteLine("   SQL: com EF Core, o Where adiado vira WHERE no banco; depois de");
        Console.WriteLine("   um .ToList() prematuro, vira SELECT * e filtro em C#. Mesma");
        Console.WriteLine("   linha, mesma sintaxe, e a diferença entre 40 ms e 40 segundos.");
    }
}
