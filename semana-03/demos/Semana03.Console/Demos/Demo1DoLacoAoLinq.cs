// ============================================================================
// DEMO 1 — do laço ao LINQ. Nada novo aconteceu.
//
// Esta demo existe para matar uma ideia errada antes que ela nasça: LINQ NÃO
// é uma capacidade nova da linguagem. É a dívida 1 do bloco de quitação —
// inversão de controle — aplicada a coleções, com nomes prontos.
//
// Lá, `Ordenar` implementava o que não varia (percorrer e trocar) e recebia
// o critério de fora, como valor. Aqui:
//
//     Where(x => ...)   implementa "percorrer e deixar passar", recebe Func<T, bool>
//     Select(x => ...)  implementa "percorrer e transformar", recebe Func<T, TResult>
//     Sum(x => ...)     implementa "percorrer e somar", recebe Func<T, decimal>
//
// Mesma mecânica, três nomes. Se você já entendeu Comparison<T>, já entendeu
// Func<T, bool> — muda o formato do delegate, não a ideia.
//
// Os dois lados abaixo produzem resultado IDÊNTICO. A demo compara e afirma.
// ============================================================================

using Semana03.Dominio;

namespace Semana03.Demos;

public static class Demo1DoLacoAoLinq
{
    public static void Executar()
    {
        var notas = Massa.Notas();

        Console.WriteLine("  Pergunta: notas AUTORIZADAS de julho/2026, da maior para a menor.");
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA: quais números saem, em que ordem? E o total?");
        Console.WriteLine();

        // --------------------------------------------------------------
        // (a) COMO NA SEMANA 2 — na mão. É C# com sotaque de Delphi, e
        //     funciona perfeitamente. O problema não é correção.
        // --------------------------------------------------------------
        var manual = new List<NotaFiscal>();

        foreach (var n in notas)
        {
            if (n.Situacao == SituacaoNota.Autorizada &&
                n.Emissao.Year == 2026 && n.Emissao.Month == 7)
            {
                manual.Add(n);
            }
        }

        // Sort com Comparison<T> — exatamente o delegate da dívida 1.
        manual.Sort((x, y) => y.Valor.CompareTo(x.Valor));

        decimal totalManual = 0m;
        foreach (var n in manual)
            totalManual += n.Valor;

        // --------------------------------------------------------------
        // (b) O MESMO, com LINQ.
        //
        // Leia de cima para baixo como uma frase: das notas, onde
        // (autorizada e de julho), ordene decrescente por valor.
        // --------------------------------------------------------------
        var comLinq = notas
            .Where(n => n.Situacao == SituacaoNota.Autorizada)
            .Where(n => n.Emissao.Year == 2026 && n.Emissao.Month == 7)
            .OrderByDescending(n => n.Valor)
            .ToList();

        decimal totalLinq = comLinq.Sum(n => n.Valor);

        // --------------------------------------------------------------
        Console.WriteLine($"  (a) laço na mão : {Massa.Numeros(manual)}");
        Console.WriteLine($"      total       : {totalManual,12:N2}");
        Console.WriteLine();
        Console.WriteLine($"  (b) LINQ        : {Massa.Numeros(comLinq)}");
        Console.WriteLine($"      total       : {totalLinq,12:N2}");
        Console.WriteLine();

        bool iguais = manual.SequenceEqual(comLinq) && totalManual == totalLinq;
        Console.WriteLine($"  Resultados idênticos? {iguais}");
        Console.WriteLine();

        Console.WriteLine("  Linhas de código: laço = 12, LINQ = 5. Mas o ganho NÃO é");
        Console.WriteLine("  tamanho. É que o laço mistura três decisões numa massa só —");
        Console.WriteLine("  filtrar, ordenar e somar — e o LINQ as mantém separadas e");
        Console.WriteLine("  nomeadas. Quando o requisito fiscal mudar (e ele muda), você");
        Console.WriteLine("  troca UMA linha em vez de reler o laço inteiro procurando");
        Console.WriteLine("  onde o filtro terminava.");
        Console.WriteLine();

        // --------------------------------------------------------------
        // A prova de que é a dívida 1: o critério como VALOR.
        //
        // Se lambda não fosse valor, isto seria impossível — mesma prova
        // que você viu no 1.6, agora com Func<T, bool> em vez de
        // Comparison<T>.
        // --------------------------------------------------------------
        Console.WriteLine("  O FILTRO É UM VALOR — igual ao critério de ordenação da dívida 1:");
        Console.WriteLine();

        var filtros = new List<(string Nome, Func<NotaFiscal, bool> Criterio)>
        {
            ("acima de 5 mil",  n => n.Valor > 5_000m),
            ("destino SP",      n => n.UfDestino == "SP"),
            ("canceladas",      n => n.Situacao == SituacaoNota.Cancelada),
        };

        foreach (var (nome, criterio) in filtros)
            Console.WriteLine($"      {nome,-16} -> {Massa.Numeros(notas.Where(criterio))}");

        Console.WriteLine();
        Console.WriteLine("  `Where` foi escrito UMA vez, na biblioteca padrão, em 2007.");
        Console.WriteLine("  Você forneceu o que varia. É a mesma frase da dívida 1.");
        Console.WriteLine();

        // --------------------------------------------------------------
        // O vocabulário que precisa ficar certo — e aqui a palavra importa
        // porque muda a assinatura de quem ler a sua especificação.
        // --------------------------------------------------------------
        Console.WriteLine("  VOCABULÁRIO — três delegates, três formatos:");
        Console.WriteLine("      Func<NotaFiscal, bool>     recebe nota, devolve bool     -> Where");
        Console.WriteLine("      Func<NotaFiscal, decimal>  recebe nota, devolve decimal  -> Sum, OrderBy");
        Console.WriteLine("      Action<NotaFiscal>         recebe nota, NÃO devolve nada -> ForEach");
        Console.WriteLine();
        Console.WriteLine("      Regra do Func<>: o ÚLTIMO parâmetro genérico é o retorno.");
        Console.WriteLine("      Func<A, B, C> recebe A e B, devolve C. Action<> nunca devolve.");
    }
}
