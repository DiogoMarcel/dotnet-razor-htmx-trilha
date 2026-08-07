// ============================================================================
// ValidadorCnpj — dígito verificador na mão.
//
// Classe static: não tem estado, não faz sentido instanciar. É uma função
// pura empacotada. `static class` impede `new ValidadorCnpj()` no compilador.
//
// ATENÇÃO ao campo static abaixo: array static é compartilhado por TODAS as
// threads do processo. Aqui é seguro porque nunca é escrito depois da
// inicialização. Num servidor web (Semana 4+) um campo static MUTÁVEL é
// bug garantido: duas requisições simultâneas veem o mesmo objeto.
// ============================================================================

namespace FiscalLab.Servicos;

public static class ValidadorCnpj
{
    // Os dois dígitos usam o MESMO algoritmo com pesos diferentes.
    // Por isso dois arrays e um único método de cálculo — não dois laços.
    private static readonly int[] PesosPrimeiroDigito =
        [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    private static readonly int[] PesosSegundoDigito =
        [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

    /// <summary>
    /// Valida CNPJ pelo dígito verificador. Aceita com ou sem pontuação.
    /// Nunca lança exceção: entrada inválida devolve false.
    /// </summary>
    /// <remarks>
    /// O parâmetro é <c>string?</c> e não <c>string</c> de propósito. Com
    /// nullable reference types ligado, declarar <c>string</c> é uma PROMESSA
    /// de que null nunca chega. Mas CNPJ vem de formulário, de CSV, de
    /// integração — null chega. Declarar <c>string?</c> é dizer a verdade ao
    /// compilador, e ele passa a exigir a checagem de null aqui dentro.
    /// </remarks>
    public static bool EhValido(string? cnpj)
    {
        // Cobre null, "" e "   " numa chamada só.
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        string digitos = SomenteDigitos(cnpj);

        // Precisa sobrar exatamente 14. "112223330001" (12) morre aqui.
        if (digitos.Length != 14)
            return false;

        // Dígitos todos iguais: rejeitado por convenção da Receita.
        //
        // CUIDADO com o folclore: dizem que "todos iguais fecha no cálculo".
        // Para CNPJ isso é FALSO em 9 dos 10 casos — 11111111111111 dá DV
        // calculado 80, não 11. O único que realmente fecha é 00000000000000
        // (soma zero, resto zero, DV 00). O folclore vem do CPF, onde
        // 11111111111 fecha de verdade.
        //
        // A regra continua necessária: sem ela, 00000000000000 entra no banco.
        // E vale como defesa contra dado de teste vazando para produção.
        if (TodosOsDigitosIguais(digitos))
            return false;

        // Primeiro DV: calculado sobre os 12 primeiros dígitos.
        int primeiroDigito = CalcularDigito(digitos[..12], PesosPrimeiroDigito);

        // Segundo DV: calculado sobre os 13 primeiros, já INCLUINDO o primeiro
        // DV que acabou de ser calculado. Não é o do CNPJ recebido — é o
        // calculado. Se usasse o recebido, um CNPJ com o 13º dígito trocado
        // poderia passar.
        int segundoDigito = CalcularDigito(digitos[..12] + primeiroDigito, PesosSegundoDigito);

        // char - '0' converte '8' em 8. Funciona porque os dígitos são
        // contíguos na tabela ASCII/Unicode. É mais rápido que int.Parse.
        return (digitos[12] - '0') == primeiroDigito
            && (digitos[13] - '0') == segundoDigito;
    }

    /// <summary>
    /// 14 dígitos -> 00.000.000/0000-00. Devolve a entrada intacta se não
    /// tiver 14 dígitos — formatar não é validar, e uma função de exibição
    /// não deve estourar por causa de dado ruim.
    /// </summary>
    public static string Formatar(string? cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return string.Empty;

        string d = SomenteDigitos(cnpj);

        if (d.Length != 14)
            return cnpj;

        // Range operator: d[..2] = do início ao índice 2 (exclusive).
        //                 d[2..5] = do 2 (inclusive) ao 5 (exclusive).
        //                 d[12..] = do 12 até o fim.
        return $"{d[..2]}.{d[2..5]}.{d[5..8]}/{d[8..12]}-{d[12..]}";
    }

    // ------------------------------------------------------------------------
    // O cálculo, um só, parametrizado pelos pesos.
    //
    // Se este método fosse escrito duas vezes (uma por dígito), qualquer
    // correção teria que ser feita nos dois lugares — e um dia só um deles
    // é corrigido. Duplicação em regra fiscal é dívida com juros.
    // ------------------------------------------------------------------------
    private static int CalcularDigito(string baseCalculo, int[] pesos)
    {
        // Contrato interno explícito. Se alguém chamar errado, quebra AQUI,
        // com a causa clara, e não com um resultado errado silencioso.
        if (baseCalculo.Length != pesos.Length)
            throw new ArgumentException(
                $"Base de {baseCalculo.Length} dígitos não combina com {pesos.Length} pesos",
                nameof(baseCalculo));

        int soma = 0;

        for (int i = 0; i < baseCalculo.Length; i++)
            soma += (baseCalculo[i] - '0') * pesos[i];

        int resto = soma % 11;

        // Regra da Receita: resto 0 ou 1 -> dígito 0. Senão, 11 - resto.
        return resto < 2 ? 0 : 11 - resto;
    }

    private static bool TodosOsDigitosIguais(string digitos)
    {
        for (int i = 1; i < digitos.Length; i++)
            if (digitos[i] != digitos[0])
                return false;

        return true;
    }

    private static string SomenteDigitos(string texto)
    {
        // StringBuilder porque concatenar string dentro de laço aloca uma
        // string NOVA a cada volta — string é imutável em .NET.
        var resultado = new System.Text.StringBuilder(texto.Length);

        foreach (char c in texto)
            if (char.IsDigit(c))
                resultado.Append(c);

        return resultado.ToString();
    }
}
