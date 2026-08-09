// ============================================================================
// DÍVIDA 3 — static é UM POR PROCESSO (Semana 2, Q8)
//
// Sua resposta: "Static tem apenas 1 instância, independente de quantos
// PROCESSOS estão sendo executados."
//
// Invertido. É UM POR PROCESSO. O que compartilha a mesma cópia são as
// THREADS e REQUISIÇÕES dentro do mesmo processo.
//
// Você já acertou a consequência mais importante — vazamento de dados entre
// usuários. Esta demo mostra o vazamento ACONTECENDO, em número, e depois
// mostra a segunda consequência, que muda com o número de processos.
// ============================================================================

namespace Quitacao.Demos;

// ----------------------------------------------------------------------------
// O ANTIPADRÃO. Isto é o que você NÃO deve escrever num PageModel.
//
// Parece inofensivo e resolve um problema real: "preciso saber qual empresa
// está logada em vários lugares do código sem passar parâmetro". Em Delphi
// desktop isso funciona — um usuário, uma thread principal.
// ----------------------------------------------------------------------------
public static class ContextoAtual
{
    public static string? EmpresaLogada { get; set; }   // BOMBA
}

// A forma correta: instância por requisição. Na Semana 4 isto vira
// AddScoped no contêiner de DI e você nem instancia à mão.
public class ContextoPorRequisicao
{
    public string? EmpresaLogada { get; set; }
}

public static class Demo3StaticEntreRequisicoes
{
    public static void Executar()
    {
        Console.WriteLine("DÍVIDA 3 — static entre requisições");
        Console.WriteLine(new string('-', 70));
        Console.WriteLine();
        Console.WriteLine("  Cenário: 6 requisições SIMULTÂNEAS, cada uma de uma empresa");
        Console.WriteLine("  diferente. Cada requisição grava a sua empresa no contexto,");
        Console.WriteLine("  faz um trabalho curto, e depois LÊ o contexto para montar o");
        Console.WriteLine("  relatório — exatamente o que um PageModel faria.");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA, antes de olhar:");
        Console.WriteLine("  >>> (a) com campo static, quantas das 6 leem a empresa ERRADA?");
        Console.WriteLine("  >>> (b) com instância por requisição, quantas leem errado?");
        Console.WriteLine();

        Console.WriteLine("  (a) COM CAMPO static  — ContextoAtual.EmpresaLogada");
        ExecutarComStatic();

        Console.WriteLine();
        Console.WriteLine("  (b) COM INSTÂNCIA POR REQUISIÇÃO — o certo");
        ExecutarComEscopo();

        Console.WriteLine();
        Console.WriteLine("  Em sistema fiscal, (a) é o operador da empresa X vendo as");
        Console.WriteLine("  notas da empresa Y. Vazamento entre clientes: incidente");
        Console.WriteLine("  reportável, não bug de conveniência.");
        Console.WriteLine();
        Console.WriteLine("  E repare: em desenvolvimento isso NÃO REPRODUZ. Um usuário,");
        Console.WriteLine("  uma requisição por vez, tudo funciona. Quebra em produção.");
        Console.WriteLine();

        MostrarSegundaConsequencia();
    }

    private static void ExecutarComStatic()
    {
        string[] empresas = ["Alfa", "Beta", "Gama", "Delta", "Epsilon", "Zeta"];
        var erros = 0;
        var linhas = new System.Collections.Concurrent.ConcurrentBag<string>();

        // Parallel.ForEach simula o servidor atendendo requisições em threads
        // diferentes do MESMO processo. É exatamente o que o Kestrel faz.
        Parallel.ForEach(empresas, empresa =>
        {
            // "Início da requisição": grava quem sou.
            ContextoAtual.EmpresaLogada = empresa;

            // Trabalho: consultar banco, calcular imposto, renderizar.
            // O Thread.Sleep é o que dá tempo de outra requisição passar por
            // cima. Num servidor real esse tempo é a consulta ao banco.
            Thread.Sleep(Random.Shared.Next(5, 40));

            // "Fim da requisição": leio o contexto para montar a resposta.
            string? lido = ContextoAtual.EmpresaLogada;

            bool ok = lido == empresa;
            if (!ok)
                Interlocked.Increment(ref erros);

            linhas.Add($"      req '{empresa,-8}' leu '{lido,-8}' {(ok ? "ok" : "<<< VAZOU")}");
        });

        foreach (var linha in linhas.OrderBy(l => l))
            Console.WriteLine(linha);

        Console.WriteLine($"      => {erros} de 6 requisições leram a empresa ERRADA");
    }

    private static void ExecutarComEscopo()
    {
        string[] empresas = ["Alfa", "Beta", "Gama", "Delta", "Epsilon", "Zeta"];
        var erros = 0;

        Parallel.ForEach(empresas, empresa =>
        {
            // A ÚNICA diferença: uma instância por requisição, não um static.
            var contexto = new ContextoPorRequisicao { EmpresaLogada = empresa };

            Thread.Sleep(Random.Shared.Next(5, 40));

            if (contexto.EmpresaLogada != empresa)
                Interlocked.Increment(ref erros);
        });

        Console.WriteLine($"      => {erros} de 6 requisições leram a empresa errada");
        Console.WriteLine("      (o objeto não é compartilhado, então não há o que corromper)");
    }

    private static void MostrarSegundaConsequencia()
    {
        Console.WriteLine("  A SEGUNDA CONSEQUÊNCIA — e é aqui que 'por processo' importa:");
        Console.WriteLine();
        Console.WriteLine($"      Este processo é o PID {Environment.ProcessId}.");
        Console.WriteLine("      ContextoAtual.EmpresaLogada existe UMA vez neste PID.");
        Console.WriteLine("      Suba uma segunda instância da aplicação: outro PID,");
        Console.WriteLine("      OUTRA cópia do campo. As duas não se conhecem.");
        Console.WriteLine();
        Console.WriteLine("      Se static fosse global entre processos, um cache static");
        Console.WriteLine("      funcionaria com o sistema escalado. Sendo POR processo,");
        Console.WriteLine("      cada instância tem o SEU cache e eles DIVERGEM:");
        Console.WriteLine();
        Console.WriteLine("        usuário salva     -> instância 1 atualiza o cache dela");
        Console.WriteLine("        load balancer     -> manda a próxima req. para a inst. 2");
        Console.WriteLine("        instância 2 lê    -> cache dela nunca soube da mudança");
        Console.WriteLine("        usuário vê        -> dado velho, sem nenhum erro no log");
        Console.WriteLine();
        Console.WriteLine("      É a mesma família do problema da Semana 10: chave de");
        Console.WriteLine("      Data Protection em memória local, cookie emitido pela");
        Console.WriteLine("      instância 1 que a instância 2 não consegue decifrar.");
        Console.WriteLine();
        Console.WriteLine("  A ESCADA PARA GUARDAR:");
        Console.WriteLine("      const / static readonly imutável -> seguro sempre");
        Console.WriteLine("      static MUTÁVEL                   -> corrompe entre REQUISIÇÕES");
        Console.WriteLine("      static como CACHE                -> DIVERGE entre INSTÂNCIAS");
        Console.WriteLine();
        Console.WriteLine("  Os arrays de pesos do ValidadorCnpj e o HashSet de UFs da");
        Console.WriteLine("  CalculadoraIcms são static readonly imutáveis — linha 1, seguros.");
    }
}
