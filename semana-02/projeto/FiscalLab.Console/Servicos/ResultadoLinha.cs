// ============================================================================
// ResultadoLinha — "deu certo ou não" como RETORNO, não como exceção.
//
// Linha inválida em arquivo de importação é ROTINA, não excepcional. Você
// quer processar as 9.999 linhas boas e reportar a ruim, não abortar tudo
// na primeira. Exceção serve para o que você NÃO esperava.
//
// Custo de exceção também pesa: lançar e capturar exceção é ordens de
// magnitude mais caro que devolver um record. Num SPED de 2 milhões de
// linhas com 5% de erro, isso é a diferença entre segundos e minutos.
// ============================================================================

namespace FiscalLab.Servicos;

public record ResultadoLinha(bool Sucesso, NotaFiscalCsv? Dados, string? Erro)
{
    // Métodos de fábrica: ResultadoLinha.Ok(x) lê melhor que
    // new ResultadoLinha(true, x, null) — e impede a combinação impossível
    // new ResultadoLinha(true, null, "erro").
    public static ResultadoLinha Ok(NotaFiscalCsv dados) => new(true, dados, null);

    public static ResultadoLinha Falha(string erro) => new(false, null, erro);
}
