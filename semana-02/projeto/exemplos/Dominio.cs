// ============================================================================
// FiscalLab — Semana 2 — Modelo de domínio (EXEMPLO COMENTADO)
//
// Este arquivo é material de estudo. Leia os comentários: eles explicam
// POR QUE cada decisão foi tomada, não o que o código faz.
//
// Copie para o seu projeto como ponto de partida. O que falta (validação
// de CNPJ, cálculo de ICMS, leitura do CSV) é o SEU exercício — está
// marcado com "TODO" no GUIA-PROJETO.md.
//
// Na Semana 4 este arquivo sai do console e vira a class library
// FiscalLab.Domain, consumida pela aplicação web.
// ============================================================================

namespace FiscalLab.Domain;

// ----------------------------------------------------------------------------
// ENUM — conjunto fechado de valores nomeados
//
// Por que enum e não int/string: o compilador impede valor inválido.
// "RegimeTributario.LucroReal" não tem como estar escrito errado;
// já a string "Lucro Real" pode virar "lucro real", "LUCRO REAL"...
//
// Os números são explícitos de propósito: eles vão para o banco e para
// o XML da NF-e (o campo CRT usa exatamente esses códigos). Se você
// reordenar um enum sem número explícito, os dados gravados mudam de
// significado silenciosamente. Nunca deixe implícito o que é persistido.
// ----------------------------------------------------------------------------
public enum RegimeTributario
{
    SimplesNacional = 1,
    LucroPresumido = 2,
    LucroReal = 3
}

public enum SituacaoNota
{
    EmDigitacao = 0,
    Autorizada = 1,
    Cancelada = 2
}

// ----------------------------------------------------------------------------
// RECORD — endereço é um DADO, não uma entidade
//
// Não tem identidade própria: dois endereços com os mesmos campos SÃO
// o mesmo endereço. É exatamente o critério para escolher record:
// comparação por valor, imutável, sem ciclo de vida próprio.
//
// A forma posicional gera construtor, propriedades init, Equals,
// GetHashCode, ToString e o operador `with`. Tudo em uma linha.
// ----------------------------------------------------------------------------
public record Endereco(
    string Logradouro,
    string Numero,
    string Municipio,
    string Uf,
    string Cep)
{
    // Um record pode ter membros extras. Aqui, uma propriedade calculada.
    // => é corpo de expressão: não guarda nada, calcula quando lido.
    public string Resumo => $"{Logradouro}, {Numero} — {Municipio}/{Uf}";
}

// ----------------------------------------------------------------------------
// CLASS — Empresa é uma ENTIDADE
//
// Tem identidade (o CNPJ), muda ao longo do tempo (endereço, situação)
// e na Semana 7 vira uma linha no banco. Entidade = class, não record.
// ----------------------------------------------------------------------------
public class Empresa
{
    // Construtor com validação: garante que uma Empresa NUNCA existe
    // em estado inválido. Se o objeto não pode nascer errado, você não
    // precisa checar de novo em cada método que o recebe.
    public Empresa(string razaoSocial, string cnpj, RegimeTributario regime)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new ArgumentException("Razão social é obrigatória", nameof(razaoSocial));

        // TODO (seu exercício): trocar por ValidadorCnpj.EhValido(cnpj)
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new ArgumentException("CNPJ é obrigatório", nameof(cnpj));

        RazaoSocial = razaoSocial.Trim();
        Cnpj = SomenteDigitos(cnpj);      // normaliza na ENTRADA, uma vez só
        Regime = regime;
    }

    // { get; } sem set: só o construtor atribui. Imutável depois de criada.
    public string Cnpj { get; }

    // { get; set; }: razão social pode mudar (alteração contratual).
    public string RazaoSocial { get; set; }

    // string? = pode ser null. Nem toda empresa tem nome fantasia.
    // Note a diferença semântica: null = "não tem"; "" = "tem, mas vazio".
    // Escolher entre os dois é decisão de modelagem, não detalhe.
    public string? NomeFantasia { get; set; }

    public RegimeTributario Regime { get; set; }
    public Endereco? Endereco { get; set; }
    public bool Ativa { get; set; } = true;

    // Formatação é responsabilidade da EXIBIÇÃO, não do armazenamento.
    // Guarde 14 dígitos limpos; formate só na hora de mostrar.
    public string CnpjFormatado =>
        Cnpj.Length == 14
            ? $"{Cnpj[..2]}.{Cnpj[2..5]}.{Cnpj[5..8]}/{Cnpj[8..12]}-{Cnpj[12..]}"
            : Cnpj;
    // Cnpj[2..5] é "range operator": do índice 2 (inclusive) ao 5 (exclusive).
    // Cnpj[..2] = do início ao 2.  Cnpj[12..] = do 12 até o fim.

    // static porque não depende de nenhuma instância — é função pura.
    // private porque só interessa a esta classe.
    private static string SomenteDigitos(string texto)
    {
        // StringBuilder porque concatenar string em laço cria uma nova
        // string a cada volta (Teoria 1, item 3).
        var resultado = new System.Text.StringBuilder(texto.Length);

        foreach (char c in texto)
            if (char.IsDigit(c))
                resultado.Append(c);

        return resultado.ToString();
    }

    // ToString aparece no Console.WriteLine e no depurador.
    // Sobrescrever economiza tempo em toda sessão de debug.
    public override string ToString() => $"{RazaoSocial} ({CnpjFormatado})";
}

// ----------------------------------------------------------------------------
// ItemNota — imutável de propósito
//
// Item de nota emitida NÃO muda. Se o valor está errado, cancela-se a nota
// e emite-se outra — é assim na legislação. O modelo deve refletir a regra
// do negócio, não a conveniência do programador.
//
// Todas as propriedades são { get; }: só o construtor escreve.
// ----------------------------------------------------------------------------
public class ItemNota
{
    public ItemNota(string descricao, decimal quantidade, decimal valorUnitario)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição é obrigatória", nameof(descricao));
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantidade));
        if (valorUnitario < 0)
            throw new ArgumentException("Valor unitário não pode ser negativo", nameof(valorUnitario));

        Descricao = descricao.Trim();
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }

    public string Descricao { get; }

    // decimal, nunca double: quantidade fiscal aceita 4 casas decimais
    // e o total precisa fechar ao centavo (Teoria 1, item 5).
    public decimal Quantidade { get; }
    public decimal ValorUnitario { get; }

    // Calculado, não armazenado. Guardar total junto de quantidade e valor
    // cria três fontes da mesma verdade — e uma hora elas divergem.
    public decimal Total => Quantidade * ValorUnitario;

    public override string ToString() =>
        $"{Descricao} — {Quantidade:N2} x {ValorUnitario:C} = {Total:C}";
}

// ----------------------------------------------------------------------------
// NotaFiscal — entidade com ciclo de vida
//
// Repare no padrão da coleção de itens: a lista é privada e exposta como
// somente leitura. Quem quiser acrescentar item passa por AdicionarItem,
// que valida. Se a List fosse pública, qualquer código poderia fazer
// nota.Itens.Add(itemInvalido) e furar toda a validação.
//
// Isso se chama ENCAPSULAMENTO. Vale para Delphi, C# e qualquer linguagem.
// ----------------------------------------------------------------------------
public class NotaFiscal
{
    private readonly List<ItemNota> _itens = new();
    // readonly: a variável não pode apontar para OUTRA lista depois de criada.
    // Atenção: o conteúdo da lista continua alterável — readonly protege a
    // referência, não o conteúdo. Confusão comum.

    public NotaFiscal(int numero, Empresa emitente, DateTime dataEmissao)
    {
        if (numero <= 0)
            throw new ArgumentException("Número deve ser positivo", nameof(numero));

        Numero = numero;
        // ?? throw: se emitente vier null, lança na hora, com a causa clara.
        // Sem isso, o erro apareceria depois, em outro lugar, sem contexto.
        Emitente = emitente ?? throw new ArgumentNullException(nameof(emitente));
        DataEmissao = dataEmissao;
    }

    public int Numero { get; }
    public Empresa Emitente { get; }
    public DateTime DataEmissao { get; }
    public SituacaoNota Situacao { get; private set; } = SituacaoNota.EmDigitacao;
    // private set: de fora ninguém atribui direto; só os métodos abaixo mudam
    // a situação, e eles sabem as regras de transição.

    public DateTime? DataCancelamento { get; private set; }

    // IReadOnlyList expõe para leitura e iteração, sem Add/Remove.
    // O chamador enxerga os itens mas não consegue alterar a coleção.
    public IReadOnlyList<ItemNota> Itens => _itens;

    public decimal ValorTotal
    {
        get
        {
            // Sem LINQ de propósito — na Semana 3 isto vira _itens.Sum(i => i.Total)
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
        // o argumento, é o ESTADO do objeto. Escolher a exceção certa é o
        // que permite tratar cada caso de forma diferente lá em cima.
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
        if (Situacao != SituacaoNota.Autorizada)
            throw new InvalidOperationException(
                $"Só nota autorizada pode ser cancelada. Situação atual: {Situacao}");

        Situacao = SituacaoNota.Cancelada;
        DataCancelamento = dataCancelamento;
    }

    public override string ToString() =>
        $"NF {Numero:D6} — {Emitente.RazaoSocial} — {ValorTotal:C} — {Situacao}";
    // {Numero:D6} preenche com zeros à esquerda: 128 vira 000128
}
