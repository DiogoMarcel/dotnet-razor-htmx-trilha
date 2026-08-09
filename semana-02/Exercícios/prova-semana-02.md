# Prova de conhecimento — Semana 2

**Regra:** sem consultar. Nem a teoria, nem o código do `FiscalLab.Console`, nem —
principalmente — o `gabarito-semana-02-CLAUDE.md`. Se você abrir o gabarito antes de
responder, a prova deixa de medir qualquer coisa.

Responder "não sei" numa questão vale mais que chutar. Lacuna identificada é lacuna que
eu consigo cobrir; chute certo por sorte vira dívida silenciosa.

Escreva abaixo de cada pergunta. Tempo alvo: 1h.

---

## 1. Valor vs referência

Uma `Empresa` é passada para um método que altera `RazaoSocial`. O chamador vê a
alteração? E se fosse um `decimal`? Explique o mecanismo.

**Resposta:**

Sim.
O motivo é que Empresa é uma classe e um decimal é uma estrutura, ou seja, uma classe passada por parâmetro é uma cópia direta do endereço, já uma estrutura é copiado o conteúdo, a origem não é alterada.

---

## 2. `decimal` vs `double`

Por que `decimal` e não `double` para valor de nota? Dê um exemplo numérico.

**Resposta:**

Double utiliza ponto flutuante binário (base 2);
Decimal utiliza representação decimal (base 10);

Em double, frações como 0.1 ou 0.2 não possuem representação exata em binário, gerando erros de arredondamento. 

Decimal é a única escolha correta, em que valores financeiros precisam ter precisão exata.

Exemplo:
// Usando DOUBLE (Ponto Flutuante Binário)
double valorDouble = 0.1 + 0.1 + 0.1;
Console.WriteLine(valorDouble == 0.3); 
// Resultado: False! (valorDouble é 0.30000000000000004)

// Usando DECIMAL (Precisão Exata Base 10)
decimal valorDecimal = 0.1m + 0.1m + 0.1m;
Console.WriteLine(valorDecimal == 0.3m); 
// Resultado: True! (valorDecimal é exatamente 0.3)

---

## 3. `record` vs `class`

Diferença entre `record` e `class`. Por que `Endereco` é record e `Empresa` é class?

**Resposta:**

Comportamento diferente: 
Class = Identidade de objeto / Record = dados por atributos.
Passado por referência / Passado por referência.
Mutável (get-set) / Imutável.
Requer construtor / Sintaxe local
---
Endereço é record por não necessitar ID, podendo existir e ser validado diretamente pelo record.
Empresa é class por ser uma entidade que precisa de ID.

---

## 4. Expressão `switch`

Escreva de cabeça uma expressão `switch` que devolve a alíquota conforme UF de origem e
destino. (18% mesmo estado · 7% Sul/Sudeste → N/NE/CO · 12% demais interestaduais · 0%
Simples Nacional.)

**Resposta:**

public static decimal ObterAliquotaIcms(string ufOrigem, string ufDestino) => (ufOrigem.ToUpper(), ufDestino.ToUpper()) switch
{
    (var origem, var destino) when origem == destino => origem switch
    {
        "SP" => 0.18m,
        "RJ" => 0.20m,
        "MG" => 0.18m,
        _    => 0.17m
    },

    ("SP" or "RJ" or "MG" or "PR" or "RS" or "SC", 
     "AC" or "AL" or "AM" or "AP" or "BA" or "CE" or "DF" or "ES" or "GO" or "MA" or "MT" or "MS" or "PA" or "PB" or "PE" or "PI" or "RN" or "RO" or "RR" or "SE" or "TO") => 0.07m,

    _ => 0.12m
};

---

## 5. `throw;` vs `throw ex;`

Diferença entre `throw;` e `throw ex;` dentro de um `catch`.

**Resposta:**

A diferença entre throw; e throw ex; está na preservação do Stack Trace (a pilha de chamadas que mostra exatamente em qual linha e arquivo a exceção original ocorreu).

throw: Relança a exceção original preservando o Stack Trace intacto.

throw ex: Relança a mesma exceção, mas reinicia o Stack Trace a partir da linha, destruindo o histórico da chamada original.

---

## 6. `IReadOnlyList<T>`

Por que `IReadOnlyList<ItemNota>` em vez de `List<ItemNota>` na propriedade `Itens`?

**Resposta:**

Um list<> apenas pode adicionar e remover, mas o IReadOnlyList<> contem mais funcionalidades como count e foreach, além de que garante que a propriedade não será alterada por acidente.

---

## 7. `TryParse` e `InvariantCulture`

Por que ler CSV com `TryParse` e `CultureInfo.InvariantCulture` em vez de `Parse` direto?

**Resposta:**


TryParse retorna bool e não aborta um processo com várias linhas, ou seja, depois de processar quase tudo ainda continua com o processo, do contrário poderia chegar ao último registro e cancelar todo o processo.
InvariantCulture: o comportamento de valores e data podem ser diferentes conforme a região do usuário, portanto utilizar a propriedade é uma garantia de dados corretos.

---

## 8. Campo `static`

O que é um campo `static` e por que ele é perigoso num servidor web?

**Resposta:**

Static tem apenas 1 instância, independente de quantos processos estão sendo executados.
O perigo em web é a concorrência, pois se há somente uma instância, dois usuários com duas empresas poderão visualizar vazamento de informações.

---

# Perguntas do projeto

Estas três estavam no exercício 4 do `GUIA-PROJETO.md`. São de raciocínio, não de código —
o tipo que interessa a você. A saída real do programa está em
`projeto/FiscalLab.Console`, mas responda **antes** de rodar.

## 9. Onde barrar o CNPJ inválido

A linha `132;11111111111111;500.00;30/07/2026` **passou** no `LeitorCsv`. Deveria? Onde é o
lugar certo de barrá-la, e por quê?

**Resposta:**

Sim, a linha parece estar correta.
Deveria barrar ao validar CNPJ com números repetidos.

---

## 10. 31 de fevereiro

A linha `135;...;300.00;31/02/2026` foi rejeitada. Por que 31 de fevereiro foi barrado sem
ninguém escrever nenhuma regra de calendário?

**Resposta:**

O tipo do campo é uma data, e acredito que por esse motivo é nativo que a data inválida seja barrada.

---

## 11. Delegate

Explique o que é o `(a, b) => b.ValorTotal.CompareTo(a.ValorTotal)` passado para
`List<T>.Sort`. Que **tipo** é isso, por que um método aceita código como parâmetro, e
qual o equivalente em Delphi?

**Resposta:**

1. É uma expressão lambda, isto é, o valor retornado é uma expressão que irá executar no futuro, não é um valor de fato, o retorno será carregado quando a expressão for executada. 

2. Isso é um delegate e funciona como ponteiro.

3. Em Delphi a representação é 'reference to function(...)'.
exemplo: Notas.Sort(TComparer<TNotaFiscal>.Construct(
  function(const A, B: TNotaFiscal): Integer
  begin
    // Invertido (B depois A) para ordem decrescente
    Result := CompareValue(B.ValorTotal, A.ValorTotal); 
  end
));

---

## 12. Ligação com Delphi — pergunta nova

Esta não estava no guia. Vale mais que as outras para o seu objetivo.

Em Delphi, um objeto de classe você cria e destrói: `try ... finally Obj.Free; end`. No
`FiscalLab.Console` não existe um único `Free`, `Dispose` explícito ou `try..finally` de
liberação — **exceto um**, no `LeitorCsv`.

**(a)** Qual é, e por que só ali?
**(b)** O que exatamente o `using` faz, e por que ele não é necessário para `Empresa`,
`NotaFiscal` ou `ItemNota`?
**(c)** O GC te livra de pensar em ciclo de vida ou só muda a pergunta? Se muda, para qual?

**Resposta:**

a. "using var leitor = new StreamReader(caminho);" - Somente ali devido o método carregado StreamReader, que utiliza recursos diretos do SO, isso faz com que o GC do C# não consiga administrar ao finalizar a leitura do arquivo, desta forma ao finalizar o processo de leitura, é necessário deixar explicito a limpeza de memória.

b. using é uma forma resumida para fazer a chamada, por baixo dos panos o C# sabe que deve ser um try...finally com dispose. As variáveis Empresa, Nota e ItemNota não precisam implementar por já serem dados cuidados pela RAM, não precisam segurar valor ou controlar de forma explícita.

c. Não livra de pensar no ciclo de vida, altera a pergunta. A nova pergunta poderia ser: O objeto tem recursos do SO que precisam de liberação ou utiliza a memória RAM? Em outras palavras o GC automatiza a limpeza da RAM, mas transfere ao desenvolvedor gerenciar recursos externos.