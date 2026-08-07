// ============================================================================
// NotaFiscalCsv — DTO (Data Transfer Object). Espelha o ARQUIVO, não o domínio.
//
// Por que não ler direto para NotaFiscal:
//   1. O CSV tem 4 campos; NotaFiscal precisa de Empresa e itens. Não bate.
//   2. O formato do arquivo muda quando o fornecedor quiser. Se o parser
//      escrevesse direto no domínio, toda mudança de layout vazaria para
//      dentro do modelo de negócio.
//   3. O arquivo pode trazer CNPJ inválido. O domínio se recusa a existir
//      inválido — então o dado ruim precisa de um lugar onde POSSA existir
//      antes de ser julgado. É esse lugar.
//
// Este é o padrão anti-corrupção na fronteira do sistema. Ele volta na
// Semana 5, quando o binding do Razor Pages preencher um DTO de formulário
// em vez de escrever direto na entidade.
// ============================================================================

namespace FiscalLab.Servicos;

public record NotaFiscalCsv(
    int Numero,
    string CnpjEmitente,
    decimal Valor,
    DateTime DataEmissao);
