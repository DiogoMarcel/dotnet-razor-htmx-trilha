// ============================================================================
// Bloco de quitação de dívidas — Semanas 1 e 2
//
// REGRA DE USO, e ela é o exercício inteiro:
//
//   1. Abra PREVISOES.md
//   2. Escreva a previsão de CADA demo
//   3. SÓ DEPOIS rode  dotnet run
//   4. Compare
//
// Se você rodar antes de prever, o programa imprime as respostas e você
// concorda com todas. Não é preguiça — é como o cérebro funciona: reconhecer
// a resposta certa é fácil, produzi-la é que mede. A previsão escrita é o
// único jeito de saber a diferença.
//
// Para rodar uma demo específica:  dotnet run -- 3
// ============================================================================

using Quitacao.Demos;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var demos = new (int Numero, string Titulo, Action Executar)[]
{
    (1, "Inversão de controle  (Semana 2, Q11)", Demo1InversaoDeControle.Executar),
    (2, "IReadOnlyList         (Semana 2, Q6)",  Demo2ReadOnlyList.Executar),
    (3, "static entre requisições (Semana 2, Q8)", Demo3StaticEntreRequisicoes.Executar),
    (4, "quem está segurando?  (Semana 2, Q12c)", Demo4QuemEstaSegurando.Executar),
};

// args vem do dotnet run -- N. Se vier vazio ou inválido, roda todas.
var selecionada = args.Length > 0 && int.TryParse(args[0], out int n) ? n : 0;

Console.WriteLine();
Console.WriteLine("QUITAÇÃO DE DÍVIDAS — Semanas 1 e 2");
Console.WriteLine();
Console.WriteLine("  Já escreveu as previsões em PREVISOES.md?");
Console.WriteLine("  Se não, feche isto e escreva. Sem previsão, a demo não ensina.");
Console.WriteLine();

foreach (var demo in demos)
{
    if (selecionada != 0 && demo.Numero != selecionada)
        continue;

    Console.WriteLine();
    Console.WriteLine(new string('=', 70));
    Console.WriteLine($"DEMO {demo.Numero} — {demo.Titulo}");
    Console.WriteLine(new string('=', 70));
    Console.WriteLine();

    demo.Executar();
    Console.WriteLine();
}

Console.WriteLine(new string('=', 70));
Console.WriteLine("FIM. A 5a dívida (acessibilidade, Semana 1) não é código —");
Console.WriteLine("está em 05-semana-01-acessibilidade.md.");
Console.WriteLine(new string('=', 70));
