// ============================================================================
// FiscalLab — Semana 2 — roteiro executável dos 6 exercícios.
//
// Top-level statements: não tem class Program nem static void Main visível.
// O compilador gera os dois. É açúcar sintático de arquivo único — na Semana
// 4 o Program.cs da aplicação web usa a mesma forma.
// ============================================================================

using System.Globalization;
using FiscalLab.Domain;
using FiscalLab.Servicos;

// ----------------------------------------------------------------------------
// Cultura de EXIBIÇÃO definida explicitamente.
//
// Duas culturas diferentes convivem neste programa, de propósito:
//   InvariantCulture -> para LER o arquivo (ponto decimal, formato fixo)
//   pt-BR            -> para MOSTRAR ao usuário (vírgula decimal, R$)
//
// Depender da cultura da máquina é bug que só aparece no servidor, onde o
// locale é en-US e ninguém testou.
// ----------------------------------------------------------------------------
var ptBr = CultureInfo.GetCultureInfo("pt-BR");
CultureInfo.DefaultThreadCurrentCulture = ptBr;
Console.OutputEncoding = System.Text.Encoding.UTF8;

Titulo("EXERCÍCIO 1 — domínio: objeto que se recusa a nascer inválido");
Exercicio1();

Titulo("EXERCÍCIO 2 — validador de CNPJ");
Exercicio2();

Titulo("EXERCÍCIO 3 — calculadora de ICMS");
Exercicio3();

Titulo("EXERCÍCIO 4 — leitura do CSV");
var notasValidas = Exercicio4();

Titulo("EXERCÍCIOS 5 e 6 — relatório por emitente, ordenado");
Exercicio56(notasValidas);


// ============================================================================
// EXERCÍCIO 1
// ============================================================================
void Exercicio1()
{
    // 11222333000181 é um CNPJ que FECHA no dígito verificador. Os CNPJs do
    // notas.csv não fecham — ver o relatório do exercício 5.
    var emitente = new Empresa("Metalúrgica Alfa Ltda", "11.222.333/0001-81",
                               RegimeTributario.LucroReal)
    {
        NomeFantasia = "Alfa Metais",
        Endereco = new Endereco("Rua das Indústrias", "480", "Joinville", "SC", "89210-100")
    };

    Console.WriteLine($"Emitente : {emitente}");
    Console.WriteLine($"Endereço : {emitente.Endereco.Resumo}");
    Console.WriteLine($"Regime   : {emitente.Regime}");
    Console.WriteLine();

    var nota = new NotaFiscal(128, emitente, new DateTime(2026, 7, 5));
    nota.AdicionarItem(new ItemNota("Chapa de aço 2mm", 12m, 87.50m));
    nota.AdicionarItem(new ItemNota("Tubo redondo 1\"", 40m, 15.30m));
    nota.AdicionarItem(new ItemNota("Eletrodo E6013 kg", 5.5m, 42.90m));

    foreach (var item in nota.Itens)
        Console.WriteLine($"  {item}");

    nota.Autorizar();
    Console.WriteLine();
    Console.WriteLine($"  => {nota}");
    Console.WriteLine();

    // ------------------------------------------------------------------------
    // Quebrando de propósito. O objetivo é LER as mensagens: exceção com
    // mensagem específica é o que diferencia um domínio útil de um que só
    // devolve NullReferenceException.
    // ------------------------------------------------------------------------
    Console.WriteLine("Quebrando de propósito:");

    // Por que capturar Exception e não tipo específico aqui: este bloco é um
    // demonstrador, quero mostrar QUALQUER falha. Em código de produção,
    // capture o tipo mais específico que você sabe tratar — capturar
    // Exception genérica esconde bug de programação junto com erro esperado.
    Tentar("ItemNota com quantidade 0",
        () => new ItemNota("Item ruim", 0m, 10m));

    Tentar("AdicionarItem em nota já autorizada",
        () => nota.AdicionarItem(new ItemNota("Item atrasado", 1m, 10m)));

    Tentar("Cancelar nota EmDigitacao",
        () => new NotaFiscal(999, emitente, DateTime.Today).Cancelar(DateTime.Today));

    Tentar("Autorizar nota sem itens",
        () => new NotaFiscal(998, emitente, DateTime.Today).Autorizar());

    Tentar("Empresa com CNPJ inválido",
        () => new Empresa("Fantasma ME", "12345678000199", RegimeTributario.LucroReal));
}

// ============================================================================
// EXERCÍCIO 2
// ============================================================================
void Exercicio2()
{
    // Tupla nomeada como massa de teste: leve, legível, sem criar um tipo só
    // para isso. Na Semana 11 isto vira [Theory]/[InlineData] no xUnit.
    (string? Entrada, bool Esperado, string Motivo)[] casos =
    [
        ("11222333000181",     true,  "14 dígitos, DV fecha"),
        ("11.222.333/0001-81", true,  "mesmo CNPJ, com pontuação"),
        ("11222333000180",     false, "DV errado no último dígito"),
        ("11111111111111",     false, "todos iguais — e o DV também não fecha (calculado 80)"),
        ("00000000000000",     false, "todos iguais — este SIM fecha na conta, só a regra o barra"),
        ("112223330001",       false, "só 12 dígitos"),
        ("",                   false, "vazio"),
        (null,                 false, "null, sem estourar exceção"),
        ("11222333000181abc",  true,  "letras são descartadas na limpeza"),
        ("12345678000199",     false, "CNPJ do notas.csv — NÃO fecha no DV"),
    ];

    int falhas = 0;

    foreach (var caso in casos)
    {
        bool obtido = ValidadorCnpj.EhValido(caso.Entrada);
        bool passou = obtido == caso.Esperado;

        if (!passou)
            falhas++;

        Console.WriteLine(
            $"  {(passou ? "OK  " : "FALHA")} {Mostrar(caso.Entrada),-22} " +
            $"esperado={Rotulo(caso.Esperado),-8} obtido={Rotulo(obtido),-8} {caso.Motivo}");
    }

    Console.WriteLine();
    Console.WriteLine(falhas == 0
        ? $"  {casos.Length} casos, nenhuma falha."
        : $"  {falhas} de {casos.Length} casos falharam.");

    Console.WriteLine();
    Console.WriteLine($"  Formatar(\"11222333000181\") -> {ValidadorCnpj.Formatar("11222333000181")}");
    Console.WriteLine($"  Formatar(\"112223330001\")   -> {ValidadorCnpj.Formatar("112223330001")} (não tem 14, devolve intacto)");

    static string Rotulo(bool v) => v ? "válido" : "inválido";
    static string Mostrar(string? s) => s is null ? "(null)" : s.Length == 0 ? "(vazio)" : s;
}

// ============================================================================
// EXERCÍCIO 3
// ============================================================================
void Exercicio3()
{
    // Item de R$ 1.000,00 exatos: facilita conferir o imposto de cabeça.
    var item = new ItemNota("Produto padrão", 10m, 100m);

    (string Origem, string Destino, RegimeTributario Regime, string Situacao)[] casos =
    [
        ("SP", "SP", RegimeTributario.LucroReal,       "origem = destino"),
        ("SP", "BA", RegimeTributario.LucroReal,       "Sudeste -> Nordeste"),
        ("RS", "MT", RegimeTributario.LucroPresumido,  "Sul -> Centro-Oeste"),
        ("BA", "SP", RegimeTributario.LucroReal,       "Nordeste -> Sudeste (volta é 12%)"),
        ("RS", "PR", RegimeTributario.LucroReal,       "Sul -> Sul, interestadual"),
        ("SP", "BA", RegimeTributario.SimplesNacional, "Simples zera, ignora as UFs"),
        ("sp", "ba", RegimeTributario.LucroReal,       "minúsculo funciona igual"),
    ];

    Console.WriteLine($"  Item: {item.Quantidade:N2} x {item.ValorUnitario:C} = {item.Total:C}");
    Console.WriteLine();
    Console.WriteLine($"  {"Rota",-10}{"Regime",-18}{"Alíquota",10}{"ICMS",14}   Situação");
    Console.WriteLine("  " + new string('-', 78));

    foreach (var caso in casos)
    {
        var r = CalculadoraIcms.Calcular(item, caso.Origem, caso.Destino, caso.Regime);

        Console.WriteLine(
            $"  {caso.Origem + " -> " + caso.Destino,-10}{caso.Regime,-18}" +
            $"{r.AliquotaPercentual,9:N0}%{r.Valor,14:N2}   {caso.Situacao}");
    }

    Console.WriteLine();

    // Prova do arredondamento AwayFromZero. Base 2,345 com 18%... use um caso
    // que caia exatamente no meio: 0,125 arredonda para 0,13, não 0,12.
    Console.WriteLine("  Arredondamento — Math.Round(0.125m, 2, ...):");
    Console.WriteLine($"    ToEven (padrão do .NET) : {Math.Round(0.125m, 2, MidpointRounding.ToEven):N3}");
    Console.WriteLine($"    AwayFromZero (Receita)  : {Math.Round(0.125m, 2, MidpointRounding.AwayFromZero):N3}");
    Console.WriteLine("    Um centavo de diferença por item. Vezes 40.000 itens/mês = SPED rejeitado.");

    Tentar("UF vazia", () => CalculadoraIcms.Calcular(item, "", "BA", RegimeTributario.LucroReal));
    Tentar("UF com 3 letras", () => CalculadoraIcms.Calcular(item, "SPO", "BA", RegimeTributario.LucroReal));
}

// ============================================================================
// EXERCÍCIO 4
// ============================================================================
List<NotaFiscalCsv> Exercicio4()
{
    // AppContext.BaseDirectory = pasta do executável (bin/Debug/net10.0).
    // É para lá que o <CopyToOutputDirectory> do .csproj manda o notas.csv.
    // NÃO use caminho relativo simples: o diretório atual do processo pode
    // ser qualquer coisa dependendo de quem chamou o programa.
    string caminho = Path.Combine(AppContext.BaseDirectory, "dados", "notas.csv");

    var resultados = LeitorCsv.Ler(caminho);

    var notas = new List<NotaFiscalCsv>();
    var erros = new List<string>();

    foreach (var resultado in resultados)
    {
        // Pattern matching com declaração: se Sucesso e Dados não é null,
        // `dados` já entra tipado como NotaFiscalCsv (não NotaFiscalCsv?).
        // Sem isso o compilador reclamaria de desreferenciar possível null.
        if (resultado.Sucesso && resultado.Dados is { } dados)
            notas.Add(dados);
        else
            erros.Add(resultado.Erro ?? "erro desconhecido");
    }

    Console.WriteLine($"  Arquivo: {caminho}");
    Console.WriteLine($"  {resultados.Count} linhas de dados processadas");
    Console.WriteLine();
    Console.WriteLine($"  OK      {notas.Count} notas carregadas");
    Console.WriteLine($"  REJEITADAS {erros.Count} linhas:");

    foreach (var erro in erros)
        Console.WriteLine($"    - {erro}");

    Console.WriteLine();

    // ------------------------------------------------------------------------
    // Resposta à pergunta 2 do exercício, em código: o dígito verificador é
    // verificado AQUI, num passo separado, e não dentro do LeitorCsv.
    // O leitor cuida de formato; DV é regra de negócio.
    // ------------------------------------------------------------------------
    Console.WriteLine("  Segundo passo — dígito verificador (regra de negócio, não formato):");

    int reprovados = 0;

    foreach (var nota in notas)
    {
        if (!ValidadorCnpj.EhValido(nota.CnpjEmitente))
        {
            reprovados++;
            Console.WriteLine($"    - NF {nota.Numero}: CNPJ {nota.CnpjEmitente} reprovado no DV");
        }
    }

    Console.WriteLine();
    Console.WriteLine($"    {reprovados} de {notas.Count} notas com CNPJ que não fecha.");
    Console.WriteLine("    A massa de teste do notas.csv usa CNPJs fictícios que não fecham no");
    Console.WriteLine("    DV. Elas seguem para o relatório marcadas com *, em vez de serem");
    Console.WriteLine("    descartadas — importação de arquivo reporta o problema, não some");
    Console.WriteLine("    com a linha em silêncio.");

    return notas;
}

// ============================================================================
// EXERCÍCIOS 5 e 6
// ============================================================================
void Exercicio56(List<NotaFiscalCsv> notas)
{
    var linhas = Relatorio.AgruparPorEmitente(notas);

    Console.WriteLine($"  {notas.Count} notas agrupadas em {linhas.Count} emitentes");
    Console.WriteLine("  (uma passada na lista, Dictionary — O(n), não O(n²))");
    Console.WriteLine();

    Relatorio.OrdenarPorValorDesc(linhas);
    Relatorio.Imprimir(linhas, ptBr);
}

// ============================================================================
// Utilitários de apresentação
// ============================================================================
void Titulo(string texto)
{
    Console.WriteLine();
    Console.WriteLine(new string('=', 78));
    Console.WriteLine(texto);
    Console.WriteLine(new string('=', 78));
}

// Action é um delegate que não recebe e não devolve nada. Passar código como
// parâmetro — mesmo mecanismo do (a, b) => do exercício 6.
void Tentar(string descricao, Action acao)
{
    try
    {
        acao();
        Console.WriteLine($"    !! '{descricao}' NÃO estourou — era esperado que estourasse");
    }
    catch (Exception ex)
    {
        // GetType().Name mostra QUAL exceção. O tipo é informação: quem trata
        // lá em cima decide o que fazer com base nele, não na mensagem.
        Console.WriteLine($"    {descricao}");
        Console.WriteLine($"      -> {ex.GetType().Name}: {ex.Message}");
    }
}
