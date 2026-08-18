// ============================================================================
// Semana 3 — LINQ e async
//
// REGRA DE USO, e é o exercício inteiro:
//
//   1. Abra ../../Exercícios/PREVISOES.md
//   2. Escreva a previsão de CADA demo
//   3. SÓ DEPOIS rode  dotnet run
//   4. Compare, e registre só o que NÃO bateu
//
// Rodar antes de prever transforma a demo numa leitura: você concorda com
// tudo e sai sem saber o que não sabia. Foi o que a correção da Semana 2
// mostrou, e o que o bloco de quitação confirmou — os conceitos que
// assentaram foram aqueles em que você voltou com o mecanismo na mão.
//
// Uma demo específica:  dotnet run -- 4
// Só o bloco de LINQ:   dotnet run -- linq      (demos 1 a 4)
// Só o bloco de async:  dotnet run -- async     (demos 5 e 6)
// ============================================================================

using Semana03.Demos;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var demos = new (int Numero, string Bloco, string Titulo, Action Executar)[]
{
    (1, "linq",  "do laço ao LINQ — nada novo aconteceu",   Demo1DoLacoAoLinq.Executar),
    (2, "linq",  "execução adiada — o susto da semana",     Demo2ExecucaoAdiada.Executar),
    (3, "linq",  "GroupBy e os agregados que estouram",     Demo3Agrupamento.Executar),
    (4, "linq",  "5 bugs plantados — ache antes de rodar",  Demo4BugsPlantados.Executar),
    (5, "async", "async é vazão, não velocidade",           Demo5AsyncThroughput.Executar),
    (6, "async", "as 4 armadilhas de async",                Demo6AsyncArmadilhas.Executar),
};

string filtro = args.Length > 0 ? args[0].ToLowerInvariant() : "";

Console.WriteLine();
Console.WriteLine("SEMANA 3 — LINQ E ASYNC");
Console.WriteLine();
Console.WriteLine("  Já escreveu as previsões em Exercícios/PREVISOES.md?");
Console.WriteLine("  Se não, feche isto e escreva. Sem previsão, a demo vira leitura.");
Console.WriteLine();

foreach (var demo in demos)
{
    bool rodar = filtro switch
    {
        "" => true,
        "linq" or "async" => demo.Bloco == filtro,
        _ => int.TryParse(filtro, out int n) && n == demo.Numero,
    };

    if (!rodar)
        continue;

    Console.WriteLine();
    Console.WriteLine(new string('=', 72));
    Console.WriteLine($"DEMO {demo.Numero} [{demo.Bloco}] — {demo.Titulo}");
    Console.WriteLine(new string('=', 72));
    Console.WriteLine();

    demo.Executar();
    Gabarito.Imprimir(demo.Numero);
    Console.WriteLine();
}

Console.WriteLine(new string('=', 72));
Console.WriteLine("FIM. Registre no PREVISOES.md só o que NÃO bateu.");
Console.WriteLine("Depois: Exercícios/exigir-ou-recusar.md e a prova.");
Console.WriteLine(new string('=', 72));
