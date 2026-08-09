// ============================================================================
// DÍVIDA 4 — "quem ainda está SEGURANDO?" (Semana 2, Q12c)
//
// Você acertou que o GC troca a pergunta, e pegou a primeira pergunta nova:
// "este objeto tem recurso do SO que precisa de liberação?"
//
// Falta a SEGUNDA, e ela é mais perigosa porque nada te avisa:
//     "alguma coisa de vida longa está SEGURANDO uma referência a este objeto?"
//
// Inversão de vocabulário que vale a semana inteira:
//     Delphi: vazamento = esqueci de LIBERAR  -> FALTA código num lugar
//     C#:     vazamento = esqueci de SOLTAR   -> SOBRA referência num lugar
//
// Achar código que falta é fácil: você procura o Free que não existe.
// Achar referência que sobra é difícil: não há linha errada, há uma linha
// CERTA segurando algo por tempo demais.
// ============================================================================

namespace Quitacao.Demos;

// Objeto pesado o suficiente para a diferença aparecer no medidor.
public class RelatorioPesado
{
    private readonly byte[] _dados = new byte[2_000_000];   // ~2 MB

    public RelatorioPesado(string nome)
    {
        Nome = nome;
        _dados[0] = 1;   // impede o compilador de otimizar o array embora
    }

    public string Nome { get; }
    public int Tamanho => _dados.Length;
}

// O ANTIPADRÃO: cache static sem política de expiração.
// Parece esperto. É o vazamento de memória mais comum em .NET.
public static class CacheDeRelatorios
{
    private static readonly List<RelatorioPesado> _cache = [];

    public static void Guardar(RelatorioPesado r) => _cache.Add(r);
    public static int Quantidade => _cache.Count;
    public static void Limpar() => _cache.Clear();
}

public static class Demo4QuemEstaSegurando
{
    public static void Executar()
    {
        Console.WriteLine("DÍVIDA 4 — quem ainda está segurando?");
        Console.WriteLine(new string('-', 70));
        Console.WriteLine();
        Console.WriteLine("  Cada RelatorioPesado ocupa ~2 MB. Vou criar 20 deles em dois");
        Console.WriteLine("  cenários, deixar as variáveis locais saírem de escopo, forçar");
        Console.WriteLine("  o GC, e medir a memória.");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA, antes de olhar:");
        Console.WriteLine("  >>> (a) SEM cache: depois do GC, a memória volta ao inicial?");
        Console.WriteLine("  >>> (b) COM cache static: volta? Se não, quanto sobra?");
        Console.WriteLine("  >>> (c) Nenhum dos dois cenários chama Dispose nem Free.");
        Console.WriteLine("  >>>     Por que só um vaza?");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // (a) Sem ninguém segurando. O GC recolhe.
        // ---------------------------------------------------------------
        long antes = MemoriaEstavel();
        Console.WriteLine($"  Linha de base            : {Mb(antes)}");

        CriarSemGuardar(20);

        long depoisA = MemoriaEstavel();
        Console.WriteLine($"  (a) sem cache, após GC   : {Mb(depoisA)}   " +
                          $"(delta {Mb(depoisA - antes)})");
        Console.WriteLine("      Nenhum Free, nenhum Dispose. O GC recolheu tudo.");
        Console.WriteLine("      É o ganho real do GC, e ele é grande.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // (b) O cache static segura. O GC não pode fazer nada.
        // ---------------------------------------------------------------
        CriarEGuardarNoCache(20);

        long depoisB = MemoriaEstavel();
        Console.WriteLine($"  (b) com cache, após GC   : {Mb(depoisB)}   " +
                          $"(delta {Mb(depoisB - antes)})");
        Console.WriteLine($"      Cache tem {CacheDeRelatorios.Quantidade} relatórios presos.");
        Console.WriteLine("      MESMO código de criação. MESMO GC forçado. Zero Dispose");
        Console.WriteLine("      nos dois casos. A única diferença é uma linha de _cache.Add.");
        Console.WriteLine();
        Console.WriteLine("      O GC não 'falhou'. Ele fez exatamente o trabalho dele:");
        Console.WriteLine("      não coletar o que ainda é ALCANÇÁVEL. E é alcançável");
        Console.WriteLine("      porque um campo static aponta para lá.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // Solta a referência. Agora o GC pode.
        // ---------------------------------------------------------------
        CacheDeRelatorios.Limpar();
        long depoisLimpar = MemoriaEstavel();

        Console.WriteLine($"  após Cache.Limpar()      : {Mb(depoisLimpar)}   " +
                          $"(delta {Mb(depoisLimpar - antes)})");
        Console.WriteLine("      A correção não foi 'liberar o objeto'. Foi SOLTAR a");
        Console.WriteLine("      referência. Objeto nenhum foi destruído por você.");
        Console.WriteLine();

        MostrarWeakReference();
        MostrarAsTresPerguntas();
    }

    private static void CriarSemGuardar(int quantos)
    {
        for (int i = 0; i < quantos; i++)
        {
            var r = new RelatorioPesado($"rel-{i}");
            _ = r.Tamanho;
            // r sai de escopo aqui. Ninguém mais o alcança.
        }
    }

    private static void CriarEGuardarNoCache(int quantos)
    {
        for (int i = 0; i < quantos; i++)
        {
            var r = new RelatorioPesado($"rel-{i}");
            CacheDeRelatorios.Guardar(r);   // <- a única linha que muda tudo
        }
    }

    // ------------------------------------------------------------------------
    // WeakReference: prova direta de "alcançável" vs "coletado", sem medir MB.
    // ------------------------------------------------------------------------
    private static void MostrarWeakReference()
    {
        Console.WriteLine("  PROVA DIRETA — WeakReference diz se o objeto ainda existe:");
        Console.WriteLine();

        var (fraca1, fraca2) = CriarDuasReferenciasFracas();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine($"      objeto SOLTO   ainda vivo? {fraca1.IsAlive,-5}  <- coletado");
        Console.WriteLine($"      objeto no cache ainda vivo? {fraca2.IsAlive,-5}  <- preso");
        Console.WriteLine();
        Console.WriteLine("      Mesmo tipo, mesmo momento de criação, mesmo GC.");
        Console.WriteLine("      Um sumiu, o outro não. A diferença é quem aponta para ele.");
        Console.WriteLine();

        CacheDeRelatorios.Limpar();
    }

    // Em método separado para as variáveis locais saírem de escopo de verdade.
    private static (WeakReference Solto, WeakReference Preso) CriarDuasReferenciasFracas()
    {
        var solto = new RelatorioPesado("solto");
        var preso = new RelatorioPesado("preso");

        CacheDeRelatorios.Guardar(preso);

        return (new WeakReference(solto), new WeakReference(preso));
    }

    private static void MostrarAsTresPerguntas()
    {
        Console.WriteLine("  AS TRÊS PERGUNTAS QUE SUBSTITUEM 'QUEM CHAMA Free?'");
        Console.WriteLine();
        Console.WriteLine("   1. Este objeto segura recurso NÃO-GERENCIADO?");
        Console.WriteLine("      (arquivo, socket, conexão de banco, handle do SO)");
        Console.WriteLine("      -> IDisposable + using. Esta você já tinha na Q12a/b.");
        Console.WriteLine("      -> E lembre: Dispose é sobre TEMPO, não sobre memória.");
        Console.WriteLine("         O GC recolhe a memória do StreamReader sozinho; o que");
        Console.WriteLine("         ele não faz é liberar o HANDLE na hora certa.");
        Console.WriteLine();
        Console.WriteLine("   2. Alguma coisa de VIDA LONGA está segurando referência?");
        Console.WriteLine("      (campo static, event handler não desinscrito, cache sem");
        Console.WriteLine("       expiração, singleton guardando dado de requisição)");
        Console.WriteLine("      -> ESTA É A QUE FALTAVA. É o que esta demo mostrou.");
        Console.WriteLine("      -> Não tem linha errada para achar. Precisa de profiler.");
        Console.WriteLine();
        Console.WriteLine("   3. Quanto tempo este objeto DEVE viver?");
        Console.WriteLine("      -> Na Semana 4 deixa de ser filosofia e vira uma linha:");
        Console.WriteLine("         AddScoped    = uma instância por requisição");
        Console.WriteLine("         AddSingleton = uma por aplicação (mesmos riscos do static)");
        Console.WriteLine("         AddTransient = uma por injeção");
        Console.WriteLine("      -> Errar aqui é o bug da Semana 7: DbContext como singleton,");
        Console.WriteLine("         que funciona na 1a requisição e quebra na 2a.");
        Console.WriteLine();
        Console.WriteLine("  Note que a pergunta 2 e a Dívida 3 (static) são o MESMO");
        Console.WriteLine("  problema visto de dois ângulos: static corrompe dado entre");
        Console.WriteLine("  requisições E impede o GC de coletar. Um campo, dois estragos.");
    }

    // ------------------------------------------------------------------------
    // GC.Collect() em código de produção é quase sempre erro — ele pausa a
    // aplicação e o runtime decide melhor que você. Aqui é legítimo: o objetivo
    // é medir, e medir exige um ponto determinístico.
    // ------------------------------------------------------------------------
    private static long MemoriaEstavel()
    {
        for (int i = 0; i < 3; i++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        return GC.GetTotalMemory(forceFullCollection: true);
    }

    private static string Mb(long bytes) => $"{bytes / 1024.0 / 1024.0,8:N2} MB";
}
