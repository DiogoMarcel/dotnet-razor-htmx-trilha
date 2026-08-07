// ============================================================================
// ItemNota — imutável de propósito.
//
// Item de nota emitida NÃO muda. Se o valor está errado, a nota é cancelada
// e outra é emitida — é assim na legislação. O modelo reflete a regra do
// negócio, não a conveniência de quem programa.
//
// Todas as propriedades são { get; }: só o construtor escreve.
// ============================================================================

namespace FiscalLab.Domain;

public class ItemNota
{
    public ItemNota(string descricao, decimal quantidade, decimal valorUnitario)
    {
        // Validação no construtor = objeto que não consegue nascer inválido.
        // Consequência prática: nenhum método que receba um ItemNota precisa
        // checar de novo se a quantidade é positiva. A garantia é do tipo.
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

    // decimal, nunca double. Quantidade fiscal aceita 4 casas e o total
    // precisa fechar ao centavo. double é base 2: 0.1 não existe exato nele.
    public decimal Quantidade { get; }
    public decimal ValorUnitario { get; }

    // Calculado, não armazenado. Guardar o total ao lado de quantidade e
    // valor unitário cria três fontes da mesma verdade — e uma hora divergem.
    public decimal Total => Quantidade * ValorUnitario;

    public override string ToString() =>
        $"{Descricao} — {Quantidade:N2} x {ValorUnitario:C} = {Total:C}";
}
