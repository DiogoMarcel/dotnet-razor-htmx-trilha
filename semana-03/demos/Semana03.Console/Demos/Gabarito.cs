// ============================================================================
// GABARITO — a ponte entre a saída das demos e o PREVISOES.md.
//
// POR QUE ESTE ARQUIVO EXISTE, e é uma correção de desenho pedida por ele em
// 13/08: as demos já mostravam tudo, mas nada na saída dizia QUAL questão cada
// trecho respondia. Conferir virava trabalho de mapear na mão, e item
// conceitual (1.4, 3.4, 5.2) ficava diluído na narração.
//
// Agora cada demo termina com o seu bloco numerado, na mesma numeração do
// PREVISOES.md. Você lê de cima para baixo e marca certo/errado sem procurar.
//
// REGRA DESTE ARQUIVO: nada de número copiado à mão. Tudo que sai da massa é
// RECALCULADO aqui, com a mesma consulta da demo. Se alguém mexer na massa ou
// num filtro, o gabarito acompanha — gabarito que mente é pior que nenhum.
//
// Os itens conceituais são resposta de REFERÊNCIA, curta de propósito. Elas
// dizem o que é certo; o porquê está na narração da demo, logo acima.
// ============================================================================

using Semana03.Dominio;

namespace Semana03.Demos;

public static class Gabarito
{
    public static void Imprimir(int demo)
    {
        Console.WriteLine();
        Console.WriteLine("  " + new string('~', 68));
        Console.WriteLine($"  GABARITO DA DEMO {demo} — confira item a item no PREVISOES.md");
        Console.WriteLine("  " + new string('~', 68));

        switch (demo)
        {
            case 1: Demo1(); break;
            case 2: Demo2(); break;
            case 3: Demo3(); break;
            case 4: Demo4(); break;
            case 5: Demo5(); break;
            case 6: Demo6(); break;
        }

        Console.WriteLine();
        Console.WriteLine("    Na tabela \"Depois de rodar\", registre SÓ o que não bateu.");
    }

    // ------------------------------------------------------------------------
    private static void Demo1()
    {
        // Recalculado: é a mesma consulta que a demo executou.
        var julho = Massa.Notas()
            .Where(n => n.Situacao == SituacaoNota.Autorizada)
            .Where(n => n.Emissao.Year == 2026 && n.Emissao.Month == 7)
            .OrderByDescending(n => n.Valor)
            .ToList();

        Item("1.1", Massa.Numeros(julho));
        Cont($"são {julho.Count}: fora 1003 e 1011 (canceladas), 1008 (em digitação),");
        Cont("e 1006 (emitida em agosto).");

        Item("1.2", $"{julho.Sum(n => n.Valor):N2}");

        Item("1.3", "Func<NotaFiscal, decimal> recebe uma NotaFiscal e devolve um decimal.");
        Cont("Action<NotaFiscal> recebe uma NotaFiscal e NÃO devolve nada.");
        Cont("No Func<>, o ÚLTIMO parâmetro genérico é sempre o retorno.");

        Item("1.4", "Where implementa o que não varia — percorrer e deixar passar — e");
        Cont("recebe de fora, como valor, o critério do que é \"passar\". Ordenar");
        Cont("implementava percorrer e trocar, e recebia de fora o critério do");
        Cont("que é \"maior\". Mesmo mecanismo: inversão de controle.");
    }

    // ------------------------------------------------------------------------
    private static void Demo2()
    {
        Item("2.1", "0");
        Cont("Where não filtrou nada. Montou a pergunta e devolveu.");

        Item("2.2", "12");
        Cont("o foreach executou o filtro sobre as 12 da massa.");

        Item("2.3", "24");
        Cont("a MESMA variável foi percorrida de novo, e o filtro rodou de novo.");
        Cont("Nenhuma linha nova. É o custo escondido de \"Any() + foreach\".");

        Item("2.4", "12");
        Cont("o ToList executou uma vez e guardou. Os dois foreach custaram 0.");

        Item("2.5", "1");
        Cont("First para no primeiro que passa, e a NF 1001 — primeira da massa —");
        Cont("é autorizada. Não avaliou as outras 11. Aqui LINQ faz MENOS trabalho");
        Cont("que um laço ingênuo, que filtraria tudo antes de pegar a primeira.");

        Item("2.6", "Adiada: SIM, a 9999 aparece.");
        Cont("A consulta lê a fonte no instante da ITERAÇÃO, não no da escrita.");
        Cont("Com .ToList() no fim: NÃO. O snapshot foi tirado antes do Add.");

        Item("2.7", "Pelo TIPO DE RETORNO.");
        Cont("Ainda é IEnumerable<T>? nada rodou.");
        Cont("É número, objeto ou List<T>? já rodou.");
    }

    // ------------------------------------------------------------------------
    private static void Demo3()
    {
        // Recalculado, igual à demo — só as duas colunas que as questões cobram.
        var relatorio = Massa.Notas()
            .Where(n => n.Situacao == SituacaoNota.Autorizada)
            .GroupBy(n => n.Cnpj)
            .Select(g => new { Razao = g.First().RazaoSocial, Total = g.Sum(n => n.Valor) })
            .OrderByDescending(l => l.Total)
            .ToList();

        Item("3.1", $"{relatorio.Count} linhas");
        Cont($"são {relatorio.Count} CNPJs distintos entre as notas autorizadas.");

        Item("3.2", $"{relatorio[0].Razao} — {relatorio[0].Total:N2}");

        Item("3.3", "Count() -> 0   ·   Sum -> 0");
        Cont("Max -> InvalidOperationException");
        Cont("Average -> InvalidOperationException");

        Item("3.4", "Elemento neutro.");
        Cont("Somar nada dá zero, e zero é a resposta matematicamente certa.");
        Cont("\"O maior de nenhum\" não tem resposta — devolver 0 seria MENTIR.");

        Item("3.5", "SP First  -> NF 1001");
        Cont("SP Single -> InvalidOperationException (são 4 itens)");
        Cont("AC First  -> InvalidOperationException (são 0 itens)");
        Cont("AC FirstOrDefault -> null");

        Item("3.6", "Single ou SingleOrDefault.");
        Cont("CNPJ identifica estabelecimento e é único por definição. Se vierem");
        Cont("dois, o cadastro está corrompido e você QUER que pare agora. First");
        Cont("escolheria um em silêncio e emitiria a nota no CNPJ errado.");
    }

    // ------------------------------------------------------------------------
    private static void Demo4()
    {
        Item("4.1", "Arredonda o TOTAL em vez de arredondar cada item.");
        Cont("Consequência: R$ 1,00 de diferença em 200 itens — meio centavo por");
        Cont("item, todos para o mesmo lado. O XML sai com o totalizador");
        Cont("divergente da soma dos itens e a SEFAZ rejeita o lote, com uma");
        Cont("mensagem que não fala nada sobre arredondamento.");

        Item("4.2", "Agrupa por RazaoSocial (nome) em vez de Cnpj (identificador).");
        Cont("Consequência: matriz e filial viram um contribuinte só — 4 linhas");
        Cont("onde há 5 estabelecimentos. Apuração de ICMS é POR estabelecimento.");

        Item("4.3", "Take(3) ANTES do Where.");
        Cont("Corta as 3 maiores de TODAS e só então descarta as não autorizadas.");
        Cont("Consequência: devolve 2 quando pediram 3, sem erro e sem aviso. Só");
        Cont("aparece no dia em que uma nota grande é cancelada.");

        Item("4.4", "Acumula o total em double no meio de um cálculo de dinheiro.");
        Cont("Consequência: bate em 2 casas e suja na 13ª. Invisível em qualquer");
        Cont("relatório, até o valor cair sobre um limite de arredondamento e um");
        Cont("centavo aparecer do nada, sem ninguém saber explicar de onde veio.");

        Item("4.5", "Math.Round(x, 2) usa MidpointRounding.ToEven — bancário.");
        Cont("Empate vai para o dígito par, não para cima. Fiscal espera");
        Cont("AwayFromZero: falta o terceiro argumento, EXPLÍCITO.");
        Cont("Delphi: é a diferença entre RoundTo e SimpleRoundTo.");

        Item("4.6", "Compilador: ZERO dos cinco.");
        Cont("Nenhum é erro de tipo ou de sintaxe — todos compilam com 0 avisos.");
        Cont("Teste escrito pela mesma IA: zero também. Ela escreveria o teste");
        Cont("contra a expectativa dela, que é exatamente a que gerou o bug.");
        Cont("Os cinco são erro de DOMÍNIO expresso em LINQ. Só pega quem sabe");
        Cont("fiscal — e é por isso que este é o exercício mais parecido com o");
        Cont("seu trabalho real.");
    }

    // ------------------------------------------------------------------------
    private static void Demo5()
    {
        Item("5.1", "(a) ~2400 ms   (b) ~800 ms");
        Cont("em (a) cada await espera o anterior TERMINAR; em (b) as três");
        Cont("esperas acontecem ao mesmo tempo.");

        Item("5.2", "NADA ficou mais rápido.");
        Cont("Cada consulta continua levando os mesmos 800 ms. O que mudou é que");
        Cont("as esperas passaram a ser SIMULTÂNEAS. A palavra certa é VAZÃO.");
        Cont("Dizer \"mais rápido\" faz quem lê usar async em cálculo, onde não");
        Cont("adianta nada — é o erro mais comum sobre o assunto.");

        Item("5.3", "Bloqueante: segundos, e o pico de esperas simultâneas fica preso");
        Cont("ao número de threads do pool (~8 nesta máquina).");
        Cont("Async: ~250 ms, pico de 64 — todas as 64 esperam juntas.");
        Cont("PORQUÊ: com Thread.Sleep, ESPERAR CONSOME UMA THREAD, então só");
        Cont("espera quem tem thread e o resto fica na fila. Com await a thread é");
        Cont("devolvida durante a espera, e as mesmas poucas atendem as 64.");

        Item("5.4", "Não. Nenhuma. Nunca.");
        Cont("async reescreve o método numa máquina de estados que sabe pausar e");
        Cont("continuar. Quem continua é uma thread do pool que JÁ EXISTIA — e");
        Cont("pode não ser a mesma que começou.");

        Item("5.5", "Não ajuda.");
        Cont("Não há espera de I/O a devolver: é CPU do começo ao fim. async só");
        Cont("rende onde existe espera ociosa. Para distribuir cálculo é Task.Run");
        Cont("ou Parallel — paralelismo, que é outra conversa.");
    }

    // ------------------------------------------------------------------------
    private static void Demo6()
    {
        Item("6.1", "Não pega.");
        Cont("async void não devolve Task, então não há onde guardar a falha. O");
        Cont("runtime joga a exceção no SynchronizationContext capturado no");
        Cont("início do método. Em ASP.NET Core não existe contexto: vai direto");
        Cont("ao thread pool e DERRUBA O PROCESSO — não é erro 500 numa");
        Cont("requisição, é a aplicação inteira caindo.");

        Item("6.2", "await                     -> InvalidOperationException");
        Cont(".Result                   -> AggregateException");
        Cont(".GetAwaiter().GetResult() -> InvalidOperationException");
        Cont("Os dois últimos bloqueiam igual; só um embrulha. Com .Result, o");
        Cont("seu catch (InvalidOperationException) NÃO pega.");

        Item("6.3", "Depois dos 400 ms.");
        Cont("Sem nenhum await, o método roda inteiro e SÍNCRONO na thread do");
        Cont("chamador, e devolve uma Task já concluída. O nome \"Async\" mente.");

        Item("6.4", "Logo após o laço: 0.   Depois de 300 ms: 5.");
        Cont("Aqui completaram porque o processo continuou vivo. Num servidor o");
        Cont("processo também continua, mas o ESCOPO da requisição não.");

        Item("6.5", "O que está errado é o MECANISMO, não a conclusão.");
        Cont("ASP.NET Core não tem SynchronizationContext, então o deadlock");
        Cont("clássico NÃO acontece. O que acontece é inanição de threads: cada");
        Cont(".Result prende uma thread do pool e a aplicação fica lenta até");
        Cont("cair, sem travar e sem erro no log.");
        Cont("IMPORTA porque quem acredita em deadlock procura contenção de lock,");
        Cont("não acha nada, e conclui que o problema é o banco. Quem sabe que é");
        Cont("inanição olha a contagem de threads do pool e acha em 5 minutos.");
    }

    // ------------------------------------------------------------------------
    private static void Item(string numero, string texto)
    {
        Console.WriteLine();
        Console.WriteLine($"    {numero}  {texto}");
    }

    private static void Cont(string texto) => Console.WriteLine($"          {texto}");
}
