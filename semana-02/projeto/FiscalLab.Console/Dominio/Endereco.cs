// ============================================================================
// Endereco — RECORD, porque é DADO e não entidade.
//
// Critério de decisão (o que importa levar da Semana 2):
//   record -> comparado por VALOR, imutável, sem ciclo de vida próprio
//   class  -> comparado por REFERÊNCIA, tem identidade, muda no tempo
//
// Dois endereços com todos os campos iguais SÃO o mesmo endereço. Não existe
// "este endereço aqui" versus "aquele endereço lá" com os mesmos dados.
// Isso é a definição de igualdade por valor.
//
// Em Delphi o análogo mais próximo é um record/objeto imutável com Equals
// escrito na mão. Aqui o compilador gera construtor, propriedades init,
// Equals, GetHashCode, ToString e o operador `with` — em uma linha.
// ============================================================================

namespace FiscalLab.Domain;

public record Endereco(
    string Logradouro,
    string Numero,
    string Municipio,
    string Uf,
    string Cep)
{
    // Record aceita membros extras. Aqui uma propriedade calculada:
    // => é corpo de expressão. Não guarda nada, calcula na hora da leitura.
    public string Resumo => $"{Logradouro}, {Numero} — {Municipio}/{Uf}";
}
