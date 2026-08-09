// ============================================================================
// DÍVIDA 2 — IReadOnlyList (Semana 2, Q6)
//
// Sua resposta, nas duas versões: "Um list<> apenas pode adicionar e remover,
// mas o IReadOnlyList<> contém mais funcionalidades como count e foreach".
//
// Esta demo não argumenta. Ela CONTA os métodos por reflection, e depois
// mostra o furo de segurança concreto que a escolha errada abre.
// ============================================================================

using System.Reflection;

namespace Quitacao.Demos;

// Entidade de mentira, só com o essencial para o ponto.
public class NotaComItens
{
    private readonly List<string> _itens = [];

    public bool Autorizada { get; private set; }

    // A escolha que está em julgamento nesta demo.
    public IReadOnlyList<string> Itens => _itens;

    public void AdicionarItem(string item)
    {
        if (Autorizada)
            throw new InvalidOperationException("Nota autorizada não aceita novos itens");

        _itens.Add(item);
    }

    public void Autorizar() => Autorizada = true;
}

public static class Demo2ReadOnlyList
{
    public static void Executar()
    {
        Console.WriteLine("DÍVIDA 2 — IReadOnlyList: quem tem MAIS?");
        Console.WriteLine(new string('-', 70));
        Console.WriteLine();
        Console.WriteLine("  >>> PREVEJA os dois números antes de olhar:");
        Console.WriteLine("  >>> quantos membros públicos tem List<string>?");
        Console.WriteLine("  >>> quantos tem IReadOnlyList<string>?");
        Console.WriteLine();

        // Reflection: pergunta ao runtime quais membros o tipo expõe.
        // Não é opinião minha, é o metadado do assembly.
        var membrosList = Membros(typeof(List<string>));
        var membrosReadOnly = MembrosComHerdados(typeof(IReadOnlyList<string>));

        Console.WriteLine($"  List<string>         : {membrosList.Count} membros públicos");
        Console.WriteLine($"  IReadOnlyList<string>: {membrosReadOnly.Count} membros públicos");
        Console.WriteLine();

        Console.WriteLine("  IReadOnlyList<string> expõe EXATAMENTE isto:");
        foreach (var m in membrosReadOnly.OrderBy(x => x))
            Console.WriteLine($"      {m}");

        Console.WriteLine();
        Console.WriteLine("  O que List<string> tem e IReadOnlyList<string> NÃO tem");
        Console.WriteLine("  (amostra dos que importam):");

        string[] interessantes =
        [
            "Add", "Remove", "RemoveAt", "Insert", "Clear", "Sort", "Reverse",
            "AddRange", "IndexOf", "Contains", "BinarySearch", "ToArray"
        ];

        foreach (var nome in interessantes)
        {
            bool temNaList = membrosList.Contains(nome);
            bool temNaInterface = membrosReadOnly.Contains(nome);

            if (temNaList && !temNaInterface)
                Console.WriteLine($"      {nome}");
        }

        Console.WriteLine();
        Console.WriteLine("  Count e o indexador estão nos DOIS. foreach também");
        Console.WriteLine("  (via GetEnumerator, herdado de IEnumerable<T>).");
        Console.WriteLine();
        Console.WriteLine("  CONCLUSÃO, e é o oposto da sua resposta:");
        Console.WriteLine("    IReadOnlyList<T> é um SUBCONJUNTO de List<T>.");
        Console.WriteLine("    Ela não adiciona nada. Ela SUBTRAI mutação.");
        Console.WriteLine();
        Console.WriteLine("    A hierarquia, de menos para mais capacidade:");
        Console.WriteLine("      IEnumerable<T>            -> foreach");
        Console.WriteLine("        IReadOnlyCollection<T>  -> + Count");
        Console.WriteLine("          IReadOnlyList<T>      -> + indexador [i]   (fim)");
        Console.WriteLine("      List<T> implementa as três E MAIS IList<T>/ICollection<T>,");
        Console.WriteLine("              que é onde vivem Add/Remove/Insert/Clear.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // Agora o furo concreto. Isto é o que a escolha compra.
        // ---------------------------------------------------------------
        Console.WriteLine("  O QUE A ESCOLHA COMPRA — nota autorizada, tentando furar:");
        Console.WriteLine();

        var nota = new NotaComItens();
        nota.AdicionarItem("Chapa de aço");
        nota.AdicionarItem("Tubo redondo");
        nota.Autorizar();

        Console.WriteLine($"    Nota autorizada com {nota.Itens.Count} itens.");

        // Caminho 1: pela porta da frente. A regra pega.
        try
        {
            nota.AdicionarItem("Contrabando");
            Console.WriteLine("    !! AdicionarItem passou — não deveria");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"    AdicionarItem  -> BARRADO: {ex.Message}");
        }

        // Caminho 2: pela propriedade. Não compila — e é o ponto todo.
        Console.WriteLine("    nota.Itens.Add(...) -> NÃO COMPILA.");
        Console.WriteLine("        'IReadOnlyList<string>' não contém definição para 'Add'");
        Console.WriteLine("        (descomente a linha no código-fonte para ver o erro)");
        // nota.Itens.Add("Contrabando");   // <- descomente: erro CS1061

        Console.WriteLine();
        Console.WriteLine("    Se Itens fosse List<string>, a linha acima COMPILARIA,");
        Console.WriteLine("    rodaria, e a validação de AdicionarItem seria decorativa.");
        Console.WriteLine();

        // ---------------------------------------------------------------
        // O limite honesto: não é cofre.
        // ---------------------------------------------------------------
        Console.WriteLine("  LIMITE HONESTO — não é imutabilidade real:");

        var furado = (List<string>)nota.Itens;   // o objeto por trás É uma List
        furado.Add("Contrabando via cast");

        Console.WriteLine($"    (List<string>)nota.Itens -> cast funcionou.");
        Console.WriteLine($"    Nota agora tem {nota.Itens.Count} itens, e ela está AUTORIZADA.");
        Console.WriteLine();
        Console.WriteLine("    IReadOnlyList impede ACIDENTE, não sabotagem.");
        Console.WriteLine("    É o que se pede de um design, não de um cofre.");
        Console.WriteLine("    Garantia real custa alocação: ImmutableList<T> ou .ToArray()");
        Console.WriteLine("    em cada leitura.");
        Console.WriteLine();
        Console.WriteLine("  PARALELO DELPHI: é o mesmo motivo pelo qual você expõe");
        Console.WriteLine("  TEnumerable<T> em vez da TObjectList<> direto. Você já faz.");
    }

    private static HashSet<string> Membros(Type tipo) =>
        tipo.GetMembers(BindingFlags.Public | BindingFlags.Instance)
            .Select(m => m.Name)
            .ToHashSet();

    // Interface não herda membros via GetMembers como classe faz —
    // é preciso somar as interfaces-base explicitamente.
    private static HashSet<string> MembrosComHerdados(Type tipoInterface)
    {
        var nomes = Membros(tipoInterface);

        foreach (var baseInterface in tipoInterface.GetInterfaces())
            nomes.UnionWith(Membros(baseInterface));

        return nomes;
    }
}
