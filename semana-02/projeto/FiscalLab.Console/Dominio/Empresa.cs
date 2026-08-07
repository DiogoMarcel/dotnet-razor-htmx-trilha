// ============================================================================
// Empresa — CLASS, porque é ENTIDADE.
//
// Tem identidade (o CNPJ), muda no tempo (endereço, razão social, situação)
// e na Semana 7 vira uma linha no banco. Entidade = class, não record.
//
// SMELL DECLARADO: o domínio (FiscalLab.Domain) está usando um serviço
// (FiscalLab.Servicos). A seta de dependência está apontando para o lado
// errado — domínio não deveria conhecer camada de serviço. Está assim porque
// o GUIA-PROJETO manda o validador viver em Servicos/. Na Semana 4, quando
// isto virar a class library FiscalLab.Domain, ValidadorCnpj vem PARA DENTRO
// do domínio: validar CNPJ é regra de negócio, não serviço de aplicação.
// Reconhecer o problema agora vale mais do que escondê-lo.
// ============================================================================

using FiscalLab.Servicos;

namespace FiscalLab.Domain;

public class Empresa
{
    public Empresa(string razaoSocial, string cnpj, RegimeTributario regime)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new ArgumentException("Razão social é obrigatória", nameof(razaoSocial));

        // Exercício 2: o TODO do material saiu daqui. Antes era só
        // IsNullOrWhiteSpace — o que aceitava "abc" como CNPJ.
        if (!ValidadorCnpj.EhValido(cnpj))
            throw new ArgumentException($"CNPJ inválido: '{cnpj}'", nameof(cnpj));

        RazaoSocial = razaoSocial.Trim();

        // Normaliza na ENTRADA, uma vez só. Depois disso, todo código que ler
        // Cnpj sabe que tem 14 dígitos limpos e não precisa tratar pontuação.
        Cnpj = SomenteDigitos(cnpj);
        Regime = regime;
    }

    // { get; } sem set: só o construtor atribui. Identidade não muda.
    public string Cnpj { get; }

    // { get; set; }: razão social muda (alteração contratual).
    public string RazaoSocial { get; set; }

    // string? = pode ser null. Nem toda empresa tem nome fantasia.
    // null = "não tem". "" = "tem, e é vazio". A diferença é modelagem,
    // não detalhe — e some se você usar string simples pra tudo.
    public string? NomeFantasia { get; set; }

    public RegimeTributario Regime { get; set; }
    public Endereco? Endereco { get; set; }
    public bool Ativa { get; set; } = true;

    // Formatação é responsabilidade da EXIBIÇÃO. Guarda 14 dígitos limpos,
    // formata só na hora de mostrar. Delega ao validador em vez de repetir
    // a lógica de máscara aqui.
    public string CnpjFormatado => ValidadorCnpj.Formatar(Cnpj);

    private static string SomenteDigitos(string texto)
    {
        var resultado = new System.Text.StringBuilder(texto.Length);

        foreach (char c in texto)
            if (char.IsDigit(c))
                resultado.Append(c);

        return resultado.ToString();
    }

    // ToString aparece no Console.WriteLine e no depurador. Sobrescrever
    // economiza tempo em toda sessão de debug daqui pra frente.
    public override string ToString() => $"{RazaoSocial} ({CnpjFormatado})";
}
