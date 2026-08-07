// ============================================================================
// NotaFiscal — entidade com ciclo de vida.
//
// A ideia central deste arquivo é ENCAPSULAMENTO da coleção:
// a List é privada, exposta como IReadOnlyList. Quem quiser acrescentar item
// passa por AdicionarItem, que valida o estado da nota.
//
// Se a List fosse pública, qualquer código faria nota.Itens.Add(qualquerCoisa)
// e furaria toda a validação — inclusive numa nota já autorizada.
// ============================================================================

namespace FiscalLab.Domain;

public class NotaFiscal
{
    // readonly protege a REFERÊNCIA, não o conteúdo: ninguém aponta _itens
    // para outra lista depois da construção, mas Add/Remove continuam
    // funcionando aqui dentro. Confusão clássica de quem chega no C#.
    private readonly List<ItemNota> _itens = [];

    public NotaFiscal(int numero, Empresa emitente, DateTime dataEmissao)
    {
        if (numero <= 0)
            throw new ArgumentException("Número deve ser positivo", nameof(numero));

        Numero = numero;

        // ?? throw: se emitente vier null, estoura AQUI, com a causa clara.
        // Sem isso o NullReferenceException apareceria três métodos depois,
        // sem nenhuma pista de onde o null entrou.
        Emitente = emitente ?? throw new ArgumentNullException(nameof(emitente));
        DataEmissao = dataEmissao;
    }

    public int Numero { get; }
    public Empresa Emitente { get; }
    public DateTime DataEmissao { get; }

    // private set: de fora ninguém atribui a situação direto. Só os métodos
    // desta classe mudam o estado, e só eles conhecem as transições válidas.
    public SituacaoNota Situacao { get; private set; } = SituacaoNota.EmDigitacao;

    public DateTime? DataCancelamento { get; private set; }

    // IReadOnlyList: dá leitura e foreach, não dá Add/Remove.
    public IReadOnlyList<ItemNota> Itens => _itens;

    public decimal ValorTotal
    {
        get
        {
            // Sem LINQ de propósito. Na Semana 3 isto vira:
            //     _itens.Sum(i => i.Total)
            decimal total = 0m;

            foreach (var item in _itens)
                total += item.Total;

            return total;
        }
    }

    public void AdicionarItem(ItemNota item)
    {
        ArgumentNullException.ThrowIfNull(item);   // forma curta do .NET moderno

        // InvalidOperationException, não ArgumentException: o problema não é
        // o argumento recebido, é o ESTADO deste objeto. Escolher a exceção
        // certa é o que permite tratar cada caso diferente lá em cima —
        // ArgumentException vira "erro 400 do usuário", InvalidOperation
        // vira "conflito de estado".
        if (Situacao != SituacaoNota.EmDigitacao)
            throw new InvalidOperationException(
                $"Nota {Numero} está {Situacao} e não aceita novos itens");

        _itens.Add(item);
    }

    public void Autorizar()
    {
        if (Situacao != SituacaoNota.EmDigitacao)
            throw new InvalidOperationException($"Nota {Numero} já está {Situacao}");

        if (_itens.Count == 0)
            throw new InvalidOperationException($"Nota {Numero} não tem itens");

        Situacao = SituacaoNota.Autorizada;
    }

    public void Cancelar(DateTime dataCancelamento)
    {
        // Nota EmDigitacao não se cancela: se ela nunca foi autorizada, não
        // existe no SEFAZ, então não há o que cancelar — descarta-se.
        if (Situacao != SituacaoNota.Autorizada)
            throw new InvalidOperationException(
                $"Só nota autorizada pode ser cancelada. Situação atual: {Situacao}");

        Situacao = SituacaoNota.Cancelada;
        DataCancelamento = dataCancelamento;
    }

    // {Numero:D6} preenche com zeros à esquerda: 128 vira 000128.
    public override string ToString() =>
        $"NF {Numero:D6} — {Emitente.RazaoSocial} — {ValorTotal:C} — {Situacao}";
}
