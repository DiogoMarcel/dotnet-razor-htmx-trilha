// ============================================================================
// FiscalLab — Semana 2 — Leitura de CSV (EXEMPLO COMENTADO)
//
// Mostra o padrão de ler dado EXTERNO com segurança: nada de Parse direto,
// nada de confiar no formato, nada de depender da cultura da máquina.
//
// É o mesmo princípio da Semana 1: dado que vem de fora é hostil até
// prova em contrário. Lá era o payload do formulário; aqui é o arquivo.
// ============================================================================

using System.Globalization;

namespace FiscalLab.Domain;

// ----------------------------------------------------------------------------
// Resultado da leitura de UMA linha.
//
// record porque é dado puro, sem identidade nem comportamento.
// Modelar "deu certo ou não" como retorno — em vez de exceção — porque
// linha inválida em arquivo de importação é ROTINA, não excepcional
// (Teoria 3, item 4). Você quer processar as 9.999 linhas boas e
// reportar a ruim, não abortar tudo.
// ----------------------------------------------------------------------------
public record ResultadoLinha(bool Sucesso, NotaFiscalCsv? Dados, string? Erro)
{
    // Métodos de fábrica: quem chama escreve ResultadoLinha.Ok(x), que é
    // mais legível do que new ResultadoLinha(true, x, null).
    public static ResultadoLinha Ok(NotaFiscalCsv dados) => new(true, dados, null);
    public static ResultadoLinha Falha(string erro) => new(false, null, erro);
}

// DTO — Data Transfer Object. Espelha o arquivo, não o domínio.
// Por que separar do domínio: o CSV pode ter formato feio, campos a mais
// ou a menos, e mudar quando o fornecedor quiser. Se você ler direto para
// NotaFiscal, toda mudança no arquivo vaza para dentro do seu modelo.
public record NotaFiscalCsv(
    int Numero,
    string CnpjEmitente,
    decimal Valor,
    DateTime DataEmissao);

public static class LeitorCsv
{
    // Separador em constante: aparece em vários pontos e um dia muda para ';'
    private const char Separador = ';';

    /// <summary>
    /// Lê o arquivo inteiro. Devolve uma lista de resultados — sucessos e
    /// falhas juntos, para o chamador decidir o que fazer com cada um.
    /// </summary>
    public static List<ResultadoLinha> Ler(string caminho)
    {
        var resultados = new List<ResultadoLinha>();

        // Checagem explícita antes de tentar abrir: mensagem clara em vez
        // de uma FileNotFoundException genérica subindo pela pilha.
        if (!File.Exists(caminho))
            throw new FileNotFoundException($"Arquivo não encontrado: {caminho}", caminho);

        // using: fecha o arquivo ao sair do método, inclusive se der exceção.
        // Sem isso o arquivo fica travado até o garbage collector passar.
        //
        // StreamReader lê linha a linha; File.ReadAllLines carrega TUDO na
        // memória. Para 10 linhas dá no mesmo; para um SPED de 2 GB, não.
        // Pegue o hábito certo desde já.
        using var leitor = new StreamReader(caminho);

        string? linha;
        int numeroLinha = 0;

        while ((linha = leitor.ReadLine()) != null)
        {
            numeroLinha++;

            // Primeira linha é o cabeçalho
            if (numeroLinha == 1)
                continue;

            // Linha em branco no fim do arquivo é comum. Ignore, não é erro.
            if (string.IsNullOrWhiteSpace(linha))
                continue;

            resultados.Add(InterpretarLinha(linha, numeroLinha));
        }

        return resultados;
    }

    private static ResultadoLinha InterpretarLinha(string linha, int numeroLinha)
    {
        var campos = linha.Split(Separador);

        // Nunca acesse campos[3] antes de conferir o tamanho. Arquivo com
        // uma coluna a menos derrubaria o programa com IndexOutOfRange.
        if (campos.Length != 4)
            return ResultadoLinha.Falha(
                $"Linha {numeroLinha}: esperava 4 campos, veio {campos.Length}");

        // --- Número ---
        // TryParse, não Parse: dado externo não vira exceção (Teoria 1, item 6).
        if (!int.TryParse(campos[0].Trim(), out int numero))
            return ResultadoLinha.Falha(
                $"Linha {numeroLinha}: número inválido '{campos[0]}'");

        // --- CNPJ ---
        string cnpj = campos[1].Trim();
        if (string.IsNullOrWhiteSpace(cnpj))
            return ResultadoLinha.Falha($"Linha {numeroLinha}: CNPJ vazio");

        // --- Valor ---
        // InvariantCulture explícito: o arquivo usa PONTO decimal.
        // Sem isso, numa máquina pt-BR "1234.56" viraria 123456.
        // NumberStyles.Number aceita separador de milhar e sinal.
        if (!decimal.TryParse(campos[2].Trim(),
                              NumberStyles.Number,
                              CultureInfo.InvariantCulture,
                              out decimal valor))
            return ResultadoLinha.Falha(
                $"Linha {numeroLinha}: valor inválido '{campos[2]}'");

        if (valor < 0)
            return ResultadoLinha.Falha(
                $"Linha {numeroLinha}: valor negativo ({valor})");

        // --- Data ---
        // ParseExact com formato fixo: "01/02/2026" é 1º de fevereiro em
        // pt-BR e 2 de janeiro em en-US. Adivinhar formato de data é a
        // origem de bugs que só aparecem depois do dia 12 do mês.
        if (!DateTime.TryParseExact(campos[3].Trim(),
                                    "dd/MM/yyyy",
                                    CultureInfo.InvariantCulture,
                                    DateTimeStyles.None,
                                    out DateTime data))
            return ResultadoLinha.Falha(
                $"Linha {numeroLinha}: data inválida '{campos[3]}' (esperado dd/MM/yyyy)");

        return ResultadoLinha.Ok(new NotaFiscalCsv(numero, cnpj, valor, data));
    }
}

// ============================================================================
// LIMITAÇÃO DECLARADA
//
// Este leitor quebra se um campo contiver o separador dentro de aspas:
//     123;"Empresa Alfa; Filial";100.00;01/07/2026
//
// CSV de verdade tem regras de aspas e escape (RFC 4180). Escrever um
// parser completo é trabalho perdido — use a biblioteca CsvHelper.
//
// Deixei simples de propósito: o objetivo da semana é C#, não parsing.
// Mas SAIBA que é uma limitação, e não descubra isso em produção.
// Reconhecer o limite do próprio código vale mais do que fingir que
// não existe.
// ============================================================================
