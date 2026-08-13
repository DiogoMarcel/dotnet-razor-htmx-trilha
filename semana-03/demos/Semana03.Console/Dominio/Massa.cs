// ============================================================================
// A massa de teste das 6 demos.
//
// É a MESMA em todas, de propósito: quando o resultado muda, a causa é o
// código, nunca o dado. Isso é o que torna previsão possível — se a massa
// variasse, errar a previsão não diria nada sobre o que você entendeu.
//
// NotaFiscal aqui é `record` porque é DADO: sem ciclo de vida, sem regra,
// só carrega valores para as consultas mastigarem. A entidade de verdade,
// com Autorizar()/Cancelar() e a List privada, é a da Semana 2.
// ============================================================================

namespace Semana03.Dominio;

public enum SituacaoNota
{
    EmDigitacao,
    Autorizada,
    Cancelada,
}

public record NotaFiscal(
    int Numero,
    string Cnpj,
    string RazaoSocial,
    string UfDestino,
    DateTime Emissao,
    decimal Valor,
    SituacaoNota Situacao);

public static class Massa
{
    // 12 notas · 4 emitentes · 3 situações · julho, com UMA de agosto.
    // A de agosto (1006) existe só para te fazer errar filtro de período —
    // e ela é a de menor valor, então some sem doer no total. É de propósito.
    public static List<NotaFiscal> Notas() =>
    [
        new(1001, "11222333000181", "Metalúrgica Aurora",      "SP", new(2026, 07, 03),  1_250.00m, SituacaoNota.Autorizada),
        new(1002, "11222333000181", "Metalúrgica Aurora",      "MG", new(2026, 07, 05),    890.50m, SituacaoNota.Autorizada),
        new(1003, "11222333000181", "Metalúrgica Aurora",      "BA", new(2026, 07, 11),  3_400.00m, SituacaoNota.Cancelada),
        new(1004, "45612378000105", "Distribuidora Boa Vista", "SP", new(2026, 07, 02), 12_000.00m, SituacaoNota.Autorizada),
        new(1005, "45612378000105", "Distribuidora Boa Vista", "ES", new(2026, 07, 09),  7_310.25m, SituacaoNota.Autorizada),
        new(1006, "45612378000105", "Distribuidora Boa Vista", "RJ", new(2026, 08, 01),    450.00m, SituacaoNota.Autorizada),
        new(1007, "33445566000199", "Transportes Cedro",       "PR", new(2026, 07, 15),  2_075.00m, SituacaoNota.Autorizada),
        new(1008, "33445566000199", "Transportes Cedro",       "SC", new(2026, 07, 20),    980.00m, SituacaoNota.EmDigitacao),
        new(1009, "33445566000199", "Transportes Cedro",       "SP", new(2026, 07, 28),  5_600.00m, SituacaoNota.Autorizada),
        new(1010, "78901234000156", "Comercial Damasco",       "AM", new(2026, 07, 08), 15_750.00m, SituacaoNota.Autorizada),
        new(1011, "78901234000156", "Comercial Damasco",       "SP", new(2026, 07, 19),     95.90m, SituacaoNota.Cancelada),
        new(1012, "78901234000156", "Comercial Damasco",       "GO", new(2026, 07, 30),  8_420.00m, SituacaoNota.Autorizada),
    ];

    public static void Imprimir(IEnumerable<NotaFiscal> notas, string titulo)
    {
        Console.WriteLine($"  {titulo}");

        foreach (var n in notas)
            Console.WriteLine(
                $"      NF {n.Numero}  {n.RazaoSocial,-24} {n.UfDestino}  " +
                $"{n.Emissao:dd/MM}  {n.Valor,10:N2}  {n.Situacao}");
    }

    // Só os números, em uma linha. É o formato que as previsões cobram.
    public static string Numeros(IEnumerable<NotaFiscal> notas) =>
        string.Join(", ", notas.Select(n => n.Numero));
}
