// ============================================================================
// LeitorCsv — dado que vem de fora é hostil até prova em contrário.
//
// Mesmo princípio da Semana 1. Lá era o payload do formulário; aqui é o
// arquivo. Nada de Parse direto, nada de confiar no formato, nada de
// depender da cultura da máquina.
//
// LIMITE DE RESPONSABILIDADE deste tipo: ele valida FORMATO, não REGRA DE
// NEGÓCIO. "abc" não é um int -> problema de formato, morre aqui.
// "11111111111111" tem 14 dígitos e é um CNPJ sintaticamente possível ->
// passa aqui, e é barrado depois, no domínio. Misturar as duas coisas
// significa que trocar o formato do arquivo obriga a reescrever regra fiscal.
// ============================================================================

using System.Globalization;

namespace FiscalLab.Servicos;

public static class LeitorCsv
{
    // Separador em constante: aparece em mais de um lugar e um dia muda.
    private const char Separador = ';';

    private const int CamposEsperados = 4;

    /// <summary>
    /// Lê o arquivo inteiro. Devolve sucessos e falhas JUNTOS, na ordem do
    /// arquivo, para o chamador decidir o que fazer com cada um.
    /// </summary>
    public static List<ResultadoLinha> Ler(string caminho)
    {
        var resultados = new List<ResultadoLinha>();

        // Checagem explícita antes de abrir: mensagem clara em vez de uma
        // FileNotFoundException genérica subindo pela pilha.
        if (!File.Exists(caminho))
            throw new FileNotFoundException($"Arquivo não encontrado: {caminho}", caminho);

        // using: fecha o arquivo ao sair do método, INCLUSIVE se der exceção.
        // Sem isso o handle fica preso até o GC passar — e no Windows o
        // arquivo fica travado para outros processos.
        //
        // StreamReader lê linha a linha. File.ReadAllLines carrega TUDO na
        // memória: para 17 linhas dá no mesmo, para um SPED de 2 GB não.
        using var leitor = new StreamReader(caminho);

        string? linha;
        int numeroLinha = 0;

        while ((linha = leitor.ReadLine()) != null)
        {
            numeroLinha++;

            if (numeroLinha == 1)
                continue;   // cabeçalho

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

        // Nunca acesse campos[3] antes de conferir o tamanho. Uma coluna a
        // menos derrubaria o programa com IndexOutOfRangeException.
        if (campos.Length != CamposEsperados)
            return ResultadoLinha.Falha(
                $"Linha {numeroLinha}: esperava {CamposEsperados} campos, veio {campos.Length}");

        // --- Número ---
        // TryParse, não Parse. Parse lança exceção; TryParse devolve bool e
        // preenche o out. Para dado externo, TryParse sempre.
        if (!int.TryParse(campos[0].Trim(), out int numero))
            return ResultadoLinha.Falha(
                $"Linha {numeroLinha}: número inválido '{campos[0]}'");

        // --- CNPJ ---
        // Só presença. O dígito verificador é regra de negócio e não é
        // checado aqui de propósito — ver o cabeçalho do arquivo.
        string cnpj = campos[1].Trim();

        if (string.IsNullOrWhiteSpace(cnpj))
            return ResultadoLinha.Falha($"Linha {numeroLinha}: CNPJ vazio");

        // --- Valor ---
        // InvariantCulture explícito: o arquivo usa PONTO decimal. Sem isso,
        // numa máquina pt-BR "1234.56" seria lido como 123456 — o ponto
        // viraria separador de milhar. Erro de fator 100, silencioso.
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
        // TryParseExact com formato fixo. "01/02/2026" é 1º de fevereiro em
        // pt-BR e 2 de janeiro em en-US. Adivinhar formato de data é a origem
        // de bugs que só aparecem depois do dia 12 do mês.
        //
        // Efeito colateral de graça: 31/02/2026 é rejeitado sem nenhuma regra
        // de calendário escrita por você. DateTime só existe em data real.
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
// CSV de verdade tem regras de aspas e escape (RFC 4180). Escrever um parser
// completo é trabalho perdido — em produção use CsvHelper.
//
// Está simples de propósito: o objetivo da semana é C#, não parsing.
// Mas SAIBA que é uma limitação, e não descubra isso em produção.
// ============================================================================
