// ============================================================================
// DEMO 5 — async é sobre VAZÃO, não sobre velocidade.
//
// A frase que você vai ouvir e precisa recusar: "async deixa mais rápido".
// Não deixa. Uma consulta que leva 800 ms leva 800 ms com ou sem async.
//
// O que async faz é DEVOLVER A THREAD enquanto espera. Ninguém fica parado
// segurando um recurso caro só para olhar a rede.
//
// Em desktop Delphi isso quase não importa: uma thread por usuário, e o
// usuário é um. Num servidor web, o pool de threads é o recurso finito que
// separa "atende 500 usuários" de "atende 50" — no MESMO hardware, com o
// MESMO banco, com a MESMA latência.
//
// Esta demo mede as duas coisas separadamente:
//   parte A — latência: sequencial vs paralelo (aqui async parece "rápido")
//   parte B — vazão: bloqueante vs assíncrono (aqui está o ponto de verdade)
// ============================================================================

using System.Diagnostics;

namespace Semana03.Demos;

public static class Demo5AsyncThroughput
{
    public static void Executar() => ExecutarAsync().GetAwaiter().GetResult();
    // ^ `.GetAwaiter().GetResult()` e não `.Result`: os dois bloqueiam, mas
    //   este propaga a exceção original em vez de embrulhá-la numa
    //   AggregateException. A demo 6 mostra a diferença. Aqui é aceitável
    //   porque estamos na raiz de um console — num PageModel, nunca.

    private static async Task ExecutarAsync()
    {
        Console.WriteLine("  Três consultas independentes ao SEFAZ, 800 ms cada.");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: quanto tempo cada abordagem leva?");
        Console.WriteLine("  >>> (a) três await em sequência   (b) Task.WhenAll");
        Console.WriteLine();

        await ParteALatencia();
        await ParteBVazao();

        MostrarConclusao();
    }

    // ========================================================================
    // PARTE A — latência. Três esperas que não dependem uma da outra.
    // ========================================================================
    private static async Task ParteALatencia()
    {
        // (a) SEQUENCIAL. Cada await espera o anterior TERMINAR.
        var relogio = Stopwatch.StartNew();

        var sp = await ConsultarSefazAsync("SP");
        var mg = await ConsultarSefazAsync("MG");
        var rj = await ConsultarSefazAsync("RJ");

        relogio.Stop();
        Console.WriteLine($"  (a) três await em sequência -> {relogio.ElapsedMilliseconds,5} ms   " +
                          $"[{sp}, {mg}, {rj}]");

        // (b) PARALELO. Dispara as três, DEPOIS espera as três.
        //
        // A diferença é uma linha: as Tasks são criadas antes do primeiro
        // await. Task já está rodando no instante em que nasce — não existe
        // "Start" a chamar.
        relogio.Restart();

        var tarefaSp = ConsultarSefazAsync("SP");
        var tarefaMg = ConsultarSefazAsync("MG");
        var tarefaRj = ConsultarSefazAsync("RJ");

        string[] resultados = await Task.WhenAll(tarefaSp, tarefaMg, tarefaRj);

        relogio.Stop();
        Console.WriteLine($"  (b) Task.WhenAll            -> {relogio.ElapsedMilliseconds,5} ms   " +
                          $"[{string.Join(", ", resultados)}]");
        Console.WriteLine();
        Console.WriteLine("      Nada ficou mais rápido: cada consulta ainda leva 800 ms.");
        Console.WriteLine("      Elas passaram a esperar AO MESMO TEMPO.");
        Console.WriteLine();
        Console.WriteLine("      E o erro clássico, que o compilador não pega:");
        Console.WriteLine("          var a = await F();  var b = await G();   <- sequencial");
        Console.WriteLine("          var a = F();        var b = G();         <- paralelo");
        Console.WriteLine("          await Task.WhenAll(a, b);");
        Console.WriteLine("      Um `await` no lugar errado custa 1,6 s por requisição, e o");
        Console.WriteLine("      código continua correto. Só fica lento.");
        Console.WriteLine();
        Console.WriteLine("      Quando NÃO paralelizar: se `b` precisa do resultado de `a`,");
        Console.WriteLine("      sequencial é o certo. Paralelo só para trabalho independente.");
        Console.WriteLine();
    }

    // ========================================================================
    // PARTE B — vazão. É este o motivo de async existir no servidor.
    // ========================================================================
    private static async Task ParteBVazao()
    {
        const int requisicoes = 64;
        const int latenciaMs = 250;

        Console.WriteLine("  ────────────────────────────────────────────────────────────────");
        Console.WriteLine($"  VAZÃO — {requisicoes} requisições simultâneas, {latenciaMs} ms de I/O cada");
        Console.WriteLine();
        Console.WriteLine("  Estrangulei o pool de threads para 2 mínimas, para simular em");
        Console.WriteLine("  segundos o que num servidor real acontece sob carga.");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA duas coisas por versão:");
        Console.WriteLine("  >>> quanto tempo leva, e quantas das 64 conseguem esperar AO");
        Console.WriteLine("  >>> MESMO TEMPO (o pico de esperas simultâneas).");
        Console.WriteLine();

        ThreadPool.GetMinThreads(out int workerOriginal, out int ioOriginal);
        ThreadPool.SetMinThreads(2, 2);

        try
        {
            // (a) BLOQUEANTE — é o código Delphi traduzido literalmente.
            //     Thread.Sleep segura a thread. Só consegue esperar quem tem
            //     thread, e o pool cria threads devagar de propósito: criar
            //     thread é caro, então ele injeta poucas por segundo.
            var relogio = Stopwatch.StartNew();
            _emEspera = 0;
            _picoBloqueante = 0;

            var bloqueantes = Enumerable.Range(0, requisicoes)
                .Select(_ => Task.Run(() =>
                {
                    RegistrarPico(ref _picoBloqueante);
                    Thread.Sleep(latenciaMs);   // <- a thread fica PARADA, segurando o lugar
                    Interlocked.Decrement(ref _emEspera);
                }))
                .ToArray();

            await Task.WhenAll(bloqueantes);
            relogio.Stop();
            long msBloqueante = relogio.ElapsedMilliseconds;

            // (b) ASSÍNCRONO — Task.Delay não segura thread nenhuma. Um timer
            //     do sistema operacional avisa quando o prazo vence.
            relogio.Restart();
            _emEspera = 0;
            _picoAssincrono = 0;

            var assincronas = Enumerable.Range(0, requisicoes)
                .Select(async _ =>
                {
                    RegistrarPico(ref _picoAssincrono);
                    await Task.Delay(latenciaMs);   // <- devolve a thread ao pool
                    Interlocked.Decrement(ref _emEspera);
                })
                .ToArray();

            await Task.WhenAll(assincronas);
            relogio.Stop();

            Console.WriteLine($"      (a) Thread.Sleep (bloqueante) -> {msBloqueante,6} ms, " +
                              $"pico de {_picoBloqueante,2} esperas simultâneas");
            Console.WriteLine($"      (b) await Task.Delay          -> {relogio.ElapsedMilliseconds,6} ms, " +
                              $"pico de {_picoAssincrono,2} esperas simultâneas");
            Console.WriteLine();
            Console.WriteLine($"      O trabalho de CPU é o mesmo: zero. As {requisicoes} esperas são");
            Console.WriteLine("      idênticas, de 250 ms cada. A diferença inteira é quem");
            Console.WriteLine("      segura a thread durante a espera.");
            Console.WriteLine();
            Console.WriteLine("      Em (a), o número de esperas simultâneas é limitado pelo");
            Console.WriteLine("      número de THREADS — porque esperar, ali, consome uma. As");
            Console.WriteLine("      demais ficam na fila. Em (b) as 64 esperam juntas, e as");
            Console.WriteLine("      mesmas poucas threads do pool seguem livres para trabalhar.");
            Console.WriteLine();
            Console.WriteLine($"      Traduzindo para requisição HTTP: (a) é o servidor que");
            Console.WriteLine($"      atende {_picoBloqueante} usuários por vez porque tem {_picoBloqueante} threads.");
            Console.WriteLine($"      (b) atende {_picoAssincrono} com as mesmas threads. É esta a palavra VAZÃO.");
            Console.WriteLine();
            Console.WriteLine("      Num Kestrel sob carga, (a) é a requisição que entra na fila e");
            Console.WriteLine("      espera thread livre. O gráfico de latência sobe, a CPU fica em");
            Console.WriteLine("      3%, e o diagnóstico 'o servidor está ocioso, deve ser o banco'");
            Console.WriteLine("      manda todo mundo procurar no lugar errado.");
            Console.WriteLine();
        }
        finally
        {
            // Devolver a configuração do pool: ela é do PROCESSO inteiro.
            // Mexer nela e não restaurar é exatamente o `static` mutável da
            // dívida 3, com outro nome.
            ThreadPool.SetMinThreads(workerOriginal, ioOriginal);
        }
    }

    private static void MostrarConclusao()
    {
        Console.WriteLine("  ────────────────────────────────────────────────────────────────");
        Console.WriteLine("  AS QUATRO FRASES QUE PRECISAM FICAR CERTAS");
        Console.WriteLine();
        Console.WriteLine("   1. `async` NÃO cria thread. Nenhuma. Nunca.");
        Console.WriteLine("      Ele reescreve o método numa máquina de estados que sabe");
        Console.WriteLine("      pausar e continuar. Quem continua é uma thread do pool que");
        Console.WriteLine("      já existia — e pode não ser a mesma que começou.");
        Console.WriteLine();
        Console.WriteLine("   2. `await` NÃO bloqueia. Ele DEVOLVE a thread e registra");
        Console.WriteLine("      'quando terminar, continue daqui'. Bloquear é `.Result`,");
        Console.WriteLine("      `.Wait()` e `Thread.Sleep`.");
        Console.WriteLine();
        Console.WriteLine("   3. Uma `Task` já está rodando quando nasce. Não existe `Start`.");
        Console.WriteLine("      É por isso que dá para disparar três e esperar depois.");
        Console.WriteLine();
        Console.WriteLine("   4. async serve para I/O — rede, disco, banco. Para cálculo puro");
        Console.WriteLine("      não adianta nada: não há espera a devolver. Aí é Task.Run,");
        Console.WriteLine("      que é outra conversa (paralelismo, não assincronia).");
        Console.WriteLine();
        Console.WriteLine("  PARALELO DELPHI: você conhece `TThread` e `Synchronize`, que é");
        Console.WriteLine("  'rode noutra thread e volte para a principal para tocar na UI'.");
        Console.WriteLine("  `await` faz a volta sozinho, e sem thread nova. O modelo que não");
        Console.WriteLine("  tem equivalente no Delphi é este: esperar sem ocupar ninguém.");
    }

    // ------------------------------------------------------------------------
    // Contagem de esperas simultâneas. Campos, não locais, porque são lidos e
    // escritos por várias threads e precisam de Interlocked.
    // ------------------------------------------------------------------------
    private static int _emEspera;
    private static int _picoBloqueante;
    private static int _picoAssincrono;

    private static void RegistrarPico(ref int pico)
    {
        int agora = Interlocked.Increment(ref _emEspera);

        // Sobe o pico sem lock: se outra thread mexeu no meio, lê de novo e
        // tenta outra vez. É o mesmo padrão de compare-and-swap que você
        // faria com InterlockedCompareExchange em Delphi.
        int visto;
        while (agora > (visto = Volatile.Read(ref pico)))
            Interlocked.CompareExchange(ref pico, agora, visto);
    }

    // ------------------------------------------------------------------------
    // A consulta falsa. Task.Delay é o substituto honesto de I/O: espera sem
    // consumir CPU e sem segurar thread — igual a uma chamada de rede.
    //
    // Thread.Sleep NÃO serviria aqui: ele segura a thread, que é justamente
    // o que a demo quer contrastar.
    // ------------------------------------------------------------------------
    private static async Task<string> ConsultarSefazAsync(string uf)
    {
        await Task.Delay(800);
        return $"{uf}:ok";
    }
}
