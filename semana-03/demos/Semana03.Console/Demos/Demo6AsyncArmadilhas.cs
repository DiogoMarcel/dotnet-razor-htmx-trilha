// ============================================================================
// DEMO 6 — as quatro armadilhas de async, e a mentira que a IA vai te contar.
//
// Estas quatro aparecem em código gerado por IA com frequência alta, porque
// o corpus de treino dela está cheio de ASP.NET **Framework** (2012-2018),
// onde as regras eram outras. Elas compilam, e três das quatro não dão nem
// aviso.
//
// A mentira específica a recusar está na armadilha 2, e é uma das poucas
// coisas em que você vai ter que corrigir a ferramenta com fonte na mão.
// ============================================================================

using System.Diagnostics;

namespace Semana03.Demos;

public static class Demo6AsyncArmadilhas
{
    public static void Executar() => ExecutarAsync().GetAwaiter().GetResult();

    private static async Task ExecutarAsync()
    {
        Console.WriteLine("  >>> PREVEJA cada uma antes de olhar. São 4.");
        Console.WriteLine();

        Armadilha1AsyncVoid();
        await Armadilha2ResultEDeadlock();
        await Armadilha3AsyncSemAwait();
        await Armadilha4FireAndForget();

        MostrarChecklist();
    }

    // ========================================================================
    // ARMADILHA 1 — `async void`: a exceção não vai para o seu catch.
    // ========================================================================
    private static void Armadilha1AsyncVoid()
    {
        Console.WriteLine("  ARMADILHA 1 — async void engole a exceção");
        Console.WriteLine("  " + new string('-', 68));
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: o `catch` em volta da chamada pega a exceção?");
        Console.WriteLine();

        // Instalo um SynchronizationContext meu só para PROVAR para onde a
        // exceção vai. Num console e num ASP.NET Core não existe contexto
        // nenhum — e é exatamente por isso que lá ela derruba o processo.
        var contextoAnterior = SynchronizationContext.Current;
        var espiao = new ContextoEspiao();
        SynchronizationContext.SetSynchronizationContext(espiao);

        bool catchDoChamadorEntrou = false;

        try
        {
            GravarAuditoriaAsyncVoid();   // <- async void: não dá para await
        }
        catch (Exception)
        {
            catchDoChamadorEntrou = true;
        }

        // Dar uma volta ao laço de mensagens do espião.
        espiao.Drenar();
        SynchronizationContext.SetSynchronizationContext(contextoAnterior);

        Console.WriteLine($"      catch do chamador entrou?      {catchDoChamadorEntrou}");
        Console.WriteLine($"      exceção capturada pelo contexto: " +
                          $"{espiao.Capturada?.GetType().Name ?? "nenhuma"}");
        Console.WriteLine($"      mensagem                       : {espiao.Capturada?.Message ?? "-"}");
        Console.WriteLine();
        Console.WriteLine("      O MECANISMO, e é o que precisa ficar:");
        Console.WriteLine("      um método `async Task` guarda a exceção DENTRO da Task, e ela");
        Console.WriteLine("      salta quando você faz await. Um `async void` não tem Task —");
        Console.WriteLine("      não há onde guardar. O runtime então joga a exceção no");
        Console.WriteLine("      SynchronizationContext capturado no início do método.");
        Console.WriteLine();
        Console.WriteLine("      Aqui eu instalei um contexto que a captura, só para mostrá-la.");
        Console.WriteLine("      Em ASP.NET Core NÃO EXISTE SynchronizationContext: a exceção");
        Console.WriteLine("      vai direto para o thread pool e DERRUBA O PROCESSO. Não é");
        Console.WriteLine("      erro 500 numa requisição — é a aplicação inteira caindo.");
        Console.WriteLine();
        Console.WriteLine("      Regra sem exceção prática: `async void` só em event handler");
        Console.WriteLine("      de UI. Em servidor, nunca. Assinatura certa é `async Task`.");
        Console.WriteLine();
    }

    // ========================================================================
    // ARMADILHA 2 — `.Result` / `.Wait()`, e a mentira sobre deadlock.
    // ========================================================================
    private static async Task Armadilha2ResultEDeadlock()
    {
        Console.WriteLine("  ARMADILHA 2 — .Result: o tipo da exceção muda");
        Console.WriteLine("  " + new string('-', 68));
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: as três linhas chamam o MESMO método, que lança");
        Console.WriteLine("  >>> InvalidOperationException. Que exceção cada catch recebe?");
        Console.WriteLine();

        // (a) await — a exceção original, intacta.
        try { await FalharAsync(); }
        catch (Exception ex) { Console.WriteLine($"      await                      -> {ex.GetType().Name}"); }

        // (b) .Result — embrulhada.
        try { _ = FalharAsync().Result; }
        catch (Exception ex) { Console.WriteLine($"      .Result                    -> {ex.GetType().Name}"); }

        // (c) .GetAwaiter().GetResult() — bloqueia igual, mas não embrulha.
        try { _ = FalharAsync().GetAwaiter().GetResult(); }
        catch (Exception ex) { Console.WriteLine($"      .GetAwaiter().GetResult()  -> {ex.GetType().Name}"); }

        Console.WriteLine();
        Console.WriteLine("      `.Result` embrulha em AggregateException porque uma Task pode");
        Console.WriteLine("      ter falhado por vários motivos (Task.WhenAll). Consequência");
        Console.WriteLine("      prática: seu `catch (InvalidOperationException)` NÃO PEGA, e");
        Console.WriteLine("      o erro sobe como falha genérica com a causa escondida em");
        Console.WriteLine("      `.InnerException`.");
        Console.WriteLine();
        Console.WriteLine("      ────── A MENTIRA A RECUSAR ──────");
        Console.WriteLine();
        Console.WriteLine("      A IA vai te dizer: \"nunca use .Result em ASP.NET Core, causa");
        Console.WriteLine("      deadlock\". A conclusão está certa; o motivo está errado, e o");
        Console.WriteLine("      motivo errado te faz procurar o problema no lugar errado.");
        Console.WriteLine();
        Console.WriteLine("      O deadlock clássico exigia um SynchronizationContext que só");
        Console.WriteLine("      admite uma thread por vez: ASP.NET Framework e WinForms/WPF.");
        Console.WriteLine("      A thread bloqueava no .Result esperando a continuação, e a");
        Console.WriteLine("      continuação esperava a thread. Travava de vez.");
        Console.WriteLine();
        Console.WriteLine("      ASP.NET Core NÃO TEM SynchronizationContext. Esse deadlock");
        Console.WriteLine("      não acontece. O que acontece é pior de diagnosticar: cada");
        Console.WriteLine("      `.Result` prende uma thread do pool, e sob carga a aplicação");
        Console.WriteLine("      morre de INANIÇÃO DE THREADS — foi o que a demo 5 mediu.");
        Console.WriteLine("      Sem travar, sem erro, só ficando lenta até cair.");
        Console.WriteLine();
        Console.WriteLine("      Por que isso importa para você: quem acredita em \"deadlock\"");
        Console.WriteLine("      procura lock e contenção, não vê nada, e conclui que é o");
        Console.WriteLine("      banco. Quem sabe que é inanição olha a contagem de threads");
        Console.WriteLine("      do pool e acha em cinco minutos.");
        Console.WriteLine();
    }

    // ========================================================================
    // ARMADILHA 3 — `async` sem `await` roda síncrono. E `Task` não é thread.
    // ========================================================================
    private static async Task Armadilha3AsyncSemAwait()
    {
        Console.WriteLine("  ARMADILHA 3 — async sem await não assincroniza nada");
        Console.WriteLine("  " + new string('-', 68));
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: o método abaixo é `async Task` e leva 400 ms de CPU.");
        Console.WriteLine("  >>> A linha seguinte à chamada roda antes ou depois dos 400 ms?");
        Console.WriteLine();

        int threadAntes = Environment.CurrentManagedThreadId;
        var relogio = Stopwatch.StartNew();

        Task tarefa = ApurarAsync();      // sem await — só chamou

        long msAteRetornar = relogio.ElapsedMilliseconds;
        Console.WriteLine($"      a chamada retornou depois de {msAteRetornar} ms");
        Console.WriteLine($"      a tarefa já terminou? {tarefa.IsCompleted}");

        await tarefa;
        relogio.Stop();

        Console.WriteLine($"      total                        {relogio.ElapsedMilliseconds} ms");
        Console.WriteLine($"      thread antes: {threadAntes} · depois: {Environment.CurrentManagedThreadId}");
        Console.WriteLine();
        Console.WriteLine("      `async` não é promessa de rodar em outro lugar. Um método");
        Console.WriteLine("      `async` roda SÍNCRONO na thread do chamador até encontrar o");
        Console.WriteLine("      primeiro `await` que realmente precise esperar. Sem `await`");
        Console.WriteLine("      nenhum, roda inteiro, síncrono, e devolve uma Task já pronta.");
        Console.WriteLine();
        Console.WriteLine("      Num PageModel isso é o handler que trava a requisição por");
        Console.WriteLine("      400 ms de CPU e ainda assim se chama `OnGetAsync`. O nome");
        Console.WriteLine("      mente; o perfil de carga não.");
        Console.WriteLine();
        Console.WriteLine("      O compilador AVISA (CS1998) — é o único dos quatro que avisa.");
        Console.WriteLine("      Eu precisei silenciar o aviso para esta demo compilar limpa.");
        Console.WriteLine("      Se a IA te entregar código com CS1998 suprimido, pergunte por quê.");
        Console.WriteLine();
    }

    // ========================================================================
    // ARMADILHA 4 — fire-and-forget: o trabalho que nunca acontece.
    // ========================================================================
    private static async Task Armadilha4FireAndForget()
    {
        Console.WriteLine("  ARMADILHA 4 — a Task que ninguém esperou");
        Console.WriteLine("  " + new string('-', 68));
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: o contador de auditoria vale quanto ao final?");
        Console.WriteLine();

        _auditorias = 0;

        // O padrão que a IA escreve para "não deixar o usuário esperando":
        // dispara e segue. A Task existe, roda, e ninguém guarda referência.
        for (int i = 0; i < 5; i++)
            _ = RegistrarAuditoriaAsync(i);   // `_ =` cala o aviso CS4014

        Console.WriteLine($"      logo após o laço            -> {_auditorias} auditorias gravadas");

        // Num PageModel, aqui a resposta HTTP já foi enviada e a requisição
        // encerrou. Quem estava no meio de um await pode nunca continuar.
        await Task.Delay(300);

        Console.WriteLine($"      300 ms depois               -> {_auditorias} auditorias gravadas");
        Console.WriteLine();
        Console.WriteLine("      Aqui elas completaram porque o processo continuou vivo. Num");
        Console.WriteLine("      servidor o processo também continua — mas o ESCOPO da");
        Console.WriteLine("      requisição não. O DbContext que a Task ia usar já foi");
        Console.WriteLine("      descartado, e você recebe ObjectDisposedException num log");
        Console.WriteLine("      sem stack trace de requisição nenhuma. Semana 7.");
        Console.WriteLine();
        Console.WriteLine("      `_ =` na frente de uma chamada async é bandeira vermelha de");
        Console.WriteLine("      revisão: alguém calou um aviso do compilador em vez de");
        Console.WriteLine("      responder à pergunta que ele fazia.");
        Console.WriteLine();
    }

    private static void MostrarChecklist()
    {
        Console.WriteLine("  ────────────────────────────────────────────────────────────────");
        Console.WriteLine("  CHECKLIST DE REVISÃO — o que procurar em código async de IA");
        Console.WriteLine();
        Console.WriteLine("      async void            -> exigir async Task (exceto handler de UI)");
        Console.WriteLine("      .Result / .Wait()     -> exigir await até a raiz");
        Console.WriteLine("      _ = MetodoAsync()     -> perguntar quem observa a falha");
        Console.WriteLine("      async sem await       -> tirar o async, ou justificar");
        Console.WriteLine("      awaits em fila        -> se independentes, Task.WhenAll");
        Console.WriteLine("      sem CancellationToken -> exigir; requisição abortada deve parar");
        Console.WriteLine("                               o trabalho, não terminá-lo à toa");
        Console.WriteLine();
        Console.WriteLine("  E a regra que resume: async é contagioso de baixo para cima.");
        Console.WriteLine("  Se o repositório é async, o serviço é async e o handler é async.");
        Console.WriteLine("  O lugar onde alguém bloqueia é exatamente o lugar onde a cadeia");
        Console.WriteLine("  foi quebrada por preguiça.");
    }

    // ------------------------------------------------------------------------
    // Apoio
    // ------------------------------------------------------------------------

    private static int _auditorias;

    // Um contexto que captura o que for postado nele. Serve só para provar
    // PARA ONDE a exceção do async void vai.
    private sealed class ContextoEspiao : SynchronizationContext
    {
        private readonly Queue<(SendOrPostCallback Callback, object? Estado)> _fila = new();

        public Exception? Capturada { get; private set; }

        public override void Post(SendOrPostCallback d, object? state)
        {
            lock (_fila)
                _fila.Enqueue((d, state));
        }

        public void Drenar()
        {
            while (true)
            {
                (SendOrPostCallback Callback, object? Estado) item;

                lock (_fila)
                {
                    if (_fila.Count == 0)
                        return;

                    item = _fila.Dequeue();
                }

                try
                {
                    item.Callback(item.Estado);
                }
                catch (Exception ex)
                {
                    Capturada = ex;
                }
            }
        }
    }

    // O antipadrão. Assinatura `async void` — não devolve Task, logo não é
    // aguardável e não tem onde guardar a falha.
    private static async void GravarAuditoriaAsyncVoid()
    {
        await Task.Yield();
        throw new InvalidOperationException("falha ao gravar auditoria");
    }

    // Devolve Task<string> — e não Task — só porque `.Result` não existe em
    // Task não-genérica. O equivalente lá é `.Wait()`, com o mesmo defeito.
    private static async Task<string> FalharAsync()
    {
        await Task.Yield();
        throw new InvalidOperationException("SEFAZ recusou o lote");
    }

    // CS1998: método async sem await. O aviso está certo — é o ponto da
    // armadilha 3 — e por isso está silenciado aqui, e SÓ aqui.
#pragma warning disable CS1998
    private static async Task ApurarAsync()
    {
        var fim = Stopwatch.StartNew();

        // Trabalho de CPU de verdade, não Sleep: Sleep também não seria
        // assíncrono, mas confundiria o ponto com a demo 5.
        while (fim.ElapsedMilliseconds < 400)
        {
            _ = Enumerable.Range(0, 5_000).Sum();
        }
    }
#pragma warning restore CS1998

    private static async Task RegistrarAuditoriaAsync(int indice)
    {
        await Task.Delay(50);
        Interlocked.Increment(ref _auditorias);
    }
}
