// ============================================================================
// DEMO 4 — CINCO BUGS PLANTADOS. Todos compilam. Todos rodam. Todos mentem.
//
// Este é o treino literal do seu trabalho: revisar código que a IA escreveu.
// Nenhum destes cinco dá erro, aviso, exceção ou log. Todos devolvem um
// número plausível — e errado.
//
// COMO USAR, e a ordem não é sugestão:
//   1. Leia cada bloco `// ERRADO` e escreva no PREVISOES.md o que está errado
//      e qual é a consequência em reais.
//   2. Só então rode. A demo imprime os dois resultados e a diferença.
//
// Se você rodar primeiro, vai concordar com a explicação e não vai saber se
// teria pego sozinho. E pegar sozinho é o trabalho.
// ============================================================================

using Semana03.Dominio;

namespace Semana03.Demos;

public static class Demo4BugsPlantados
{
    // Massa desta demo = a massa comum + duas notas que expõem os bugs.
    private static List<NotaFiscal> MassaAmpliada()
    {
        var notas = Massa.Notas();

        // FILIAL: mesma razão social do 11222333000181, CNPJ diferente.
        // Estabelecimento distinto — apuração de ICMS é POR ESTABELECIMENTO.
        notas.Add(new NotaFiscal(2001, "11222333000272", "Metalúrgica Aurora",
                                 "SP", new DateTime(2026, 07, 07), 20_000.00m,
                                 SituacaoNota.Autorizada));

        // Uma cancelada de valor alto. Existe para o bug 3.
        notas.Add(new NotaFiscal(2002, "45612378000105", "Distribuidora Boa Vista",
                                 "MG", new DateTime(2026, 07, 14), 30_000.00m,
                                 SituacaoNota.Cancelada));

        return notas;
    }

    public static void Executar()
    {
        Console.WriteLine("  5 trechos. Todos compilam com 0 avisos. Todos estão errados.");
        Console.WriteLine("  >>> Ache cada um ANTES de rodar. Escreva no PREVISOES.md.");
        Console.WriteLine();

        Bug1Arredondamento();
        Bug2ChaveDeAgrupamento();
        Bug3OrdemDosOperadores();
        Bug4DoubleNoMeioDoDecimal();
        Bug5ArredondamentoBancario();

        Console.WriteLine();
        Console.WriteLine("  ────────────────────────────────────────────────────────────────");
        Console.WriteLine("  O QUE OS CINCO TÊM EM COMUM");
        Console.WriteLine();
        Console.WriteLine("  Nenhum é erro de LINQ. Todos são erro de DOMÍNIO expresso em");
        Console.WriteLine("  LINQ — e é por isso que nenhuma ferramenta pega:");
        Console.WriteLine();
        Console.WriteLine("      o compilador  vê tipos certos");
        Console.WriteLine("      o analisador  vê sintaxe idiomática");
        Console.WriteLine("      o teste       passa, se o teste foi escrito pela mesma IA");
        Console.WriteLine("      o revisor     vê código bonito");
        Console.WriteLine();
        Console.WriteLine("  Quem pega é quem sabe que apuração é por estabelecimento, que");
        Console.WriteLine("  imposto arredonda por linha, e que nota cancelada não compõe");
        Console.WriteLine("  base de cálculo. Ou seja: você, não a ferramenta.");
        Console.WriteLine();
        Console.WriteLine("  É esta a vantagem que você tem sobre a IA no seu escritório, e");
        Console.WriteLine("  ela é a única que não vai encolher com a próxima versão do modelo.");
    }

    // ========================================================================
    // BUG 1 — arredondar o TOTAL em vez de arredondar CADA LINHA.
    // ========================================================================
    private static void Bug1Arredondamento()
    {
        Cabecalho(1, "ICMS de 18% de uma nota com 200 itens");

        // Uma nota de peças: 120 itens a 27,75 e 80 a 15,25.
        // Não é massa escolhida a esmo — os dois preços caem EXATAMENTE no
        // meio centavo quando multiplicados por 18%:
        //      27,75 × 0,18 = 4,995      15,25 × 0,18 = 2,745
        // Cada item arredonda meio centavo para cima. 200 itens, 200 meios
        // centavos. Isso não é caso de laboratório: preço de peça terminado
        // em ,75 e ,25 é o que mais existe em nota de indústria.
        decimal[] itens =
        [
            .. Enumerable.Repeat(27.75m, 120),
            .. Enumerable.Repeat(15.25m, 80),
        ];

        const decimal aliquota = 0.18m;

        // ERRADO: soma tudo com as casas cheias e arredonda uma vez, no fim.
        decimal errado = Math.Round(itens.Sum(v => v * aliquota), 2,
                                    MidpointRounding.AwayFromZero);

        // CERTO: cada item gera o seu próprio valor de imposto, arredondado
        // nele. É esse valor por item que vai para o XML da NF-e e para o
        // livro fiscal — o total é consequência, não origem.
        decimal certo = itens.Sum(v => Math.Round(v * aliquota, 2,
                                                  MidpointRounding.AwayFromZero));

        Resultado(errado, certo);
        Console.WriteLine($"      São {itens.Length} itens e a diferença é {certo - errado:N2} —");
        Console.WriteLine("      exatamente meio centavo por item, todos para o mesmo lado.");
        Console.WriteLine();
        Console.WriteLine("      O XML da nota vai com o total ERRADO e a soma dos itens");
        Console.WriteLine("      CERTA. A SEFAZ rejeita o lote por divergência de totalizador,");
        Console.WriteLine("      e a mensagem de rejeição não diz nada sobre arredondamento.");
        Console.WriteLine();
        Console.WriteLine("      Repare que não há linha errada. Há uma ORDEM errada: somar");
        Console.WriteLine("      antes de arredondar, em vez de arredondar antes de somar.");
        Console.WriteLine("      É o tipo de defeito que sobrevive a qualquer revisão que");
        Console.WriteLine("      esteja procurando 'erro no código'.");
    }

    // ========================================================================
    // BUG 2 — agrupar por razão social em vez de CNPJ.
    // ========================================================================
    private static void Bug2ChaveDeAgrupamento()
    {
        Cabecalho(2, "quantos contribuintes emitiram nota autorizada?");

        var notas = MassaAmpliada().Where(n => n.Situacao == SituacaoNota.Autorizada).ToList();

        // ERRADO: razão social é NOME, não identificador. Matriz e filial
        // compartilham. Duas empresas do mesmo grupo também podem.
        var porNome = notas.GroupBy(n => n.RazaoSocial).ToList();

        // CERTO: CNPJ identifica o ESTABELECIMENTO, e apuração é por
        // estabelecimento.
        var porCnpj = notas.GroupBy(n => n.Cnpj).ToList();

        Console.WriteLine($"      ERRADO  GroupBy(RazaoSocial) -> {porNome.Count} contribuintes");
        Console.WriteLine($"      CERTO   GroupBy(Cnpj)        -> {porCnpj.Count} contribuintes");
        Console.WriteLine();

        // O grupo por nome que esconde mais de um CNPJ dentro.
        var fundido = porNome.First(g => g.Select(n => n.Cnpj).Distinct().Count() > 1);

        Console.WriteLine($"      '{fundido.Key}' virou UMA linha de " +
                          $"{fundido.Sum(n => n.Valor):N2}, somando dois CNPJs:");

        foreach (var g in fundido.GroupBy(n => n.Cnpj))
            Console.WriteLine($"          {g.Key}  {g.Sum(n => n.Valor),12:N2}");

        Console.WriteLine();
        Console.WriteLine("      A IA escolhe RazaoSocial porque o relatório fica mais legível.");
        Console.WriteLine("      Chave de agrupamento é decisão FISCAL, não de apresentação.");
    }

    // ========================================================================
    // BUG 3 — Take antes do Where. A ordem dos operadores é semântica.
    // ========================================================================
    private static void Bug3OrdemDosOperadores()
    {
        Cabecalho(3, "as 3 maiores notas AUTORIZADAS");

        var notas = MassaAmpliada();

        // ERRADO: pega as 3 maiores de TODAS, e só então descarta as que não
        // estão autorizadas. Sobram menos de 3, sem nenhum aviso.
        var errado = notas
            .OrderByDescending(n => n.Valor)
            .Take(3)
            .Where(n => n.Situacao == SituacaoNota.Autorizada)
            .ToList();

        // CERTO: filtra primeiro, ordena o que sobrou, aí corta.
        var certo = notas
            .Where(n => n.Situacao == SituacaoNota.Autorizada)
            .OrderByDescending(n => n.Valor)
            .Take(3)
            .ToList();

        Console.WriteLine($"      ERRADO -> {errado.Count} notas: {Massa.Numeros(errado)}");
        Console.WriteLine($"      CERTO  -> {certo.Count} notas: {Massa.Numeros(certo)}");
        Console.WriteLine();
        Console.WriteLine("      Pediram 3, a tela mostra 2, e ninguém repara — porque a tela");
        Console.WriteLine("      não sabe quantas deveria ter. O bug só aparece no dia em que");
        Console.WriteLine("      uma nota grande é cancelada.");
        Console.WriteLine();
        Console.WriteLine("      REGRA: filtre (Where) antes de cortar (Take/Skip/First).");
        Console.WriteLine("      Na Semana 7 essa mesma ordem vira o TOP/OFFSET do SQL, e o");
        Console.WriteLine("      erro passa a ser do banco.");
    }

    // ========================================================================
    // BUG 4 — um double no meio do caminho.
    // ========================================================================
    private static void Bug4DoubleNoMeioDoDecimal()
    {
        Cabecalho(4, "o acumulador que virou double");

        // A alíquota chegou como double porque o desserializador de JSON
        // devolve double por padrão para número sem sufixo. Ninguém reparou,
        // e o acumulador acompanhou o tipo.
        double aliquotaDouble = 0.18;
        const decimal aliquotaDecimal = 0.18m;

        decimal[] itens = [.. Enumerable.Repeat(27.75m, 120), .. Enumerable.Repeat(15.25m, 80)];

        // ERRADO: o total é acumulado em double. Cada parcela é aproximada,
        // e os erros se somam.
        double totalDouble = 0;
        foreach (var v in itens)
            totalDouble += (double)v * aliquotaDouble;

        // CERTO: decimal do começo ao fim. A alíquota vira decimal na
        // FRONTEIRA do sistema (na desserialização), nunca no cálculo.
        decimal totalDecimal = itens.Sum(v => v * aliquotaDecimal);

        Console.WriteLine($"      ERRADO  double  -> {totalDouble:G17}");
        Console.WriteLine($"      CERTO   decimal -> {totalDecimal}");
        Console.WriteLine();
        Console.WriteLine($"      os dois batem em 2 casas?  {Math.Round((decimal)totalDouble, 2) == Math.Round(totalDecimal, 2)}");
        Console.WriteLine($"      os dois são iguais?        {(decimal)totalDouble == totalDecimal}");
        Console.WriteLine();
        Console.WriteLine("      É ESTE o formato do defeito, e é por isso que ele passa:");
        Console.WriteLine("      em 2 casas decimais o valor BATE. A sujeira mora na 13ª casa.");
        Console.WriteLine("      Nenhum relatório mostra a 13ª casa, então ninguém vê nada —");
        Console.WriteLine("      até o dia em que o valor cai exatamente sobre um limite de");
        Console.WriteLine("      arredondamento e um centavo aparece do nada.");
        Console.WriteLine();

        // A demonstração canônica, e ela é exata em decimal.
        Console.WriteLine($"      double  : 0.1 + 0.2 == 0.3  ->  {0.1 + 0.2 == 0.3}");
        Console.WriteLine($"      decimal : 0.1m + 0.2m == 0.3m -> {0.1m + 0.2m == 0.3m}");
        Console.WriteLine($"      double  : (0.1 + 0.2) = {0.1 + 0.2:G17}");
        Console.WriteLine();
        Console.WriteLine("      `double` é base 2. Um décimo não existe em base 2, do mesmo");
        Console.WriteLine("      jeito que um terço não existe em base 10. `decimal` é base 10");
        Console.WriteLine("      e representa centavo exato — é para isso que ele foi feito.");
        Console.WriteLine();
        Console.WriteLine("      REGRA DE REVISÃO: `(double)` sobre dinheiro se recusa SEMPRE,");
        Console.WriteLine("      inclusive quando a diferença desta execução der zero. A");
        Console.WriteLine("      diferença é função dos valores, e amanhã os valores são outros.");
        Console.WriteLine();
        Console.WriteLine("      Cast que o compilador exige é PERGUNTA, não permissão. Quem");
        Console.WriteLine("      escreveu o cast respondeu 'cala a boca' a um aviso legítimo.");
    }

    // ========================================================================
    // BUG 5 — Math.Round arredonda "para o par" por padrão.
    // ========================================================================
    private static void Bug5ArredondamentoBancario()
    {
        Cabecalho(5, "Math.Round(valor, 2) — o padrão do .NET não é o padrão fiscal");

        decimal[] casos = [2.345m, 2.355m, 0.125m, 1.005m];

        Console.WriteLine($"      {"valor",8}  {"Round(x,2)",12}  {"AwayFromZero",14}");

        foreach (var v in casos)
            Console.WriteLine($"      {v,8}  {Math.Round(v, 2),12}  " +
                              $"{Math.Round(v, 2, MidpointRounding.AwayFromZero),14}");

        Console.WriteLine();
        Console.WriteLine("      `Math.Round(x, 2)` usa MidpointRounding.ToEven — arredondamento");
        Console.WriteLine("      bancário. Empate vai para o dígito PAR, não para cima.");
        Console.WriteLine("      Existe para não enviesar médias estatísticas.");
        Console.WriteLine();
        Console.WriteLine("      Delphi: `RoundTo` faz a MESMA coisa (banker's), e `SimpleRoundTo`");
        Console.WriteLine("      é que arredonda meio para cima. Se você já apanhou disso lá,");
        Console.WriteLine("      é a mesma armadilha com outro nome.");
        Console.WriteLine();
        Console.WriteLine("      Em nota fiscal, o esperado é meio-para-cima. Toda chamada de");
        Console.WriteLine("      Math.Round sobre dinheiro precisa do terceiro argumento");
        Console.WriteLine("      EXPLÍCITO — e código gerado por IA quase nunca o traz.");
    }

    // ------------------------------------------------------------------------
    private static void Cabecalho(int n, string titulo)
    {
        Console.WriteLine($"  BUG {n} — {titulo}");
        Console.WriteLine("  " + new string('-', 68));
    }

    private static void Resultado(decimal errado, decimal certo)
    {
        Console.WriteLine($"      ERRADO -> {errado,14:N2}");
        Console.WriteLine($"      CERTO  -> {certo,14:N2}");
        Console.WriteLine($"      delta  -> {errado - certo,14:N2}");
        Console.WriteLine();
    }
}
