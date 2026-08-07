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

<!-- escreva aqui -->

---

## 2. `decimal` vs `double`

Por que `decimal` e não `double` para valor de nota? Dê um exemplo numérico.

**Resposta:**

<!-- escreva aqui -->

---

## 3. `record` vs `class`

Diferença entre `record` e `class`. Por que `Endereco` é record e `Empresa` é class?

**Resposta:**

<!-- escreva aqui -->

---

## 4. Expressão `switch`

Escreva de cabeça uma expressão `switch` que devolve a alíquota conforme UF de origem e
destino. (18% mesmo estado · 7% Sul/Sudeste → N/NE/CO · 12% demais interestaduais · 0%
Simples Nacional.)

**Resposta:**

<!-- escreva aqui -->

---

## 5. `throw;` vs `throw ex;`

Diferença entre `throw;` e `throw ex;` dentro de um `catch`.

**Resposta:**

<!-- escreva aqui -->

---

## 6. `IReadOnlyList<T>`

Por que `IReadOnlyList<ItemNota>` em vez de `List<ItemNota>` na propriedade `Itens`?

**Resposta:**

<!-- escreva aqui -->

---

## 7. `TryParse` e `InvariantCulture`

Por que ler CSV com `TryParse` e `CultureInfo.InvariantCulture` em vez de `Parse` direto?

**Resposta:**

<!-- escreva aqui -->

---

## 8. Campo `static`

O que é um campo `static` e por que ele é perigoso num servidor web?

**Resposta:**

<!-- escreva aqui -->

---

# Perguntas do projeto

Estas três estavam no exercício 4 do `GUIA-PROJETO.md`. São de raciocínio, não de código —
o tipo que interessa a você. A saída real do programa está em
`projeto/FiscalLab.Console`, mas responda **antes** de rodar.

## 9. Onde barrar o CNPJ inválido

A linha `132;11111111111111;500.00;30/07/2026` **passou** no `LeitorCsv`. Deveria? Onde é o
lugar certo de barrá-la, e por quê?

**Resposta:**

<!-- escreva aqui -->

---

## 10. 31 de fevereiro

A linha `135;...;300.00;31/02/2026` foi rejeitada. Por que 31 de fevereiro foi barrado sem
ninguém escrever nenhuma regra de calendário?

**Resposta:**

<!-- escreva aqui -->

---

## 11. Delegate

Explique o que é o `(a, b) => b.ValorTotal.CompareTo(a.ValorTotal)` passado para
`List<T>.Sort`. Que **tipo** é isso, por que um método aceita código como parâmetro, e
qual o equivalente em Delphi?

**Resposta:**

<!-- escreva aqui -->

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

<!-- escreva aqui -->
