# Previsões — Semana 3

**Escreva aqui ANTES de rodar `dotnet run`.** Esta folha é o exercício; as demos são a
conferência.

No bloco de quitação você acertou 17 de 21 escrevendo antes. Os 4 erros só apareceram
**porque** havia previsão escrita — sem ela, você teria lido a explicação, concordado, e
saído achando que sabia as quatro.

**"Não sei" é resposta válida e útil.** Chute certo por sorte vira dívida silenciosa.

**Regra em vigor desde 12/08:** resposta que reformula o enunciado conta como não-resposta.
Se a sua frase não permitiria a outra pessoa implementar a coisa, ela não está pronta.

---

# BLOCO A — LINQ (`dotnet run -- linq`)

## Demo 1 — do laço ao LINQ

A massa, na ordem de criação:

| NF | CNPJ | Razão social | UF | Emissão | Valor | Situação |
|---|---|---|---|---|---|---|
| 1001 | 11222333000181 | Metalúrgica Aurora | SP | 03/07 | 1.250,00 | Autorizada |
| 1002 | 11222333000181 | Metalúrgica Aurora | MG | 05/07 | 890,50 | Autorizada |
| 1003 | 11222333000181 | Metalúrgica Aurora | BA | 11/07 | 3.400,00 | Cancelada |
| 1004 | 45612378000105 | Distribuidora Boa Vista | SP | 02/07 | 12.000,00 | Autorizada |
| 1005 | 45612378000105 | Distribuidora Boa Vista | ES | 09/07 | 7.310,25 | Autorizada |
| 1006 | 45612378000105 | Distribuidora Boa Vista | RJ | **01/08** | 450,00 | Autorizada |
| 1007 | 33445566000199 | Transportes Cedro | PR | 15/07 | 2.075,00 | Autorizada |
| 1008 | 33445566000199 | Transportes Cedro | SC | 20/07 | 980,00 | EmDigitacao |
| 1009 | 33445566000199 | Transportes Cedro | SP | 28/07 | 5.600,00 | Autorizada |
| 1010 | 78901234000156 | Comercial Damasco | AM | 08/07 | 15.750,00 | Autorizada |
| 1011 | 78901234000156 | Comercial Damasco | SP | 19/07 | 95,90 | Cancelada |
| 1012 | 78901234000156 | Comercial Damasco | GO | 30/07 | 8.420,00 | Autorizada |

**1.1** Notas **autorizadas de julho/2026**, da maior para a menor. Escreva os números na ordem.

> resposta:

**1.2** O total dessas notas.

> resposta:

**1.3** `Func<NotaFiscal, decimal>` — o que ele recebe e o que devolve? E `Action<NotaFiscal>`?

> resposta:

**1.4** Sem rodar: qual a relação entre `Where(Func<T, bool>)` e o `Ordenar(List<T>,
Comparison<T>)` do bloco de quitação? Responda em uma frase que **não** use a palavra "lambda".

> resposta:

---

## Demo 2 — execução adiada

Um contador sobe a cada avaliação da lambda do `Where`. A massa tem **12** notas.

**2.1** `var q = notas.Where(...);` e mais nada. Contador?

> resposta:

**2.2** Depois de **um** `foreach` sobre `q`. Contador?

> resposta:

**2.3** Depois de um **segundo** `foreach` sobre a **mesma** variável `q`. Contador?

> resposta:

**2.4** `notas.Where(...).ToList()` e depois **dois** `foreach` sobre a lista. Contador?

> resposta:

**2.5** `notas.Where(...).First()`. Contador?

> resposta, e por quê:

**2.6** A consulta `notas.Where(n => n.Valor > 10_000m)` é escrita. **Depois** disso alguém
faz `notas.Add(...)` de uma nota de 99.000,00. A nota nova aparece no resultado?
E se a consulta tivesse `.ToList()` no fim?

> resposta:

**2.7** Complete a régua, sem consultar: como você olha para uma linha de LINQ e sabe se ela
**já executou** ou **ainda não**?

> resposta:

---

## Demo 3 — GroupBy e agregados

**3.1** O relatório agrupa as **autorizadas** por CNPJ. Quantas linhas ele tem?

> resposta:

**3.2** Qual emitente aparece em **primeiro** (maior total), e qual é esse total?

> resposta:

**3.3** Sobre uma lista **vazia**, quais devolvem valor e quais estouram?

| | devolve o quê, ou estoura? |
|---|---|
| `Count()` | |
| `Sum(n => n.Valor)` | |
| `Max(n => n.Valor)` | |
| `Average(n => n.Valor)` | |

**3.4** Por que `Sum` e `Max` se comportam **diferente** com sequência vazia? Não é
inconsistência da biblioteca — qual é o critério?

> resposta:

**3.5** A massa tem 4 notas com destino SP. O que cada linha devolve?

| | resultado |
|---|---|
| `Where(UF=="SP").First()` | |
| `Where(UF=="SP").Single()` | |
| `Where(UF=="AC").First()` | |
| `Where(UF=="AC").FirstOrDefault()` | |

**3.6** Um método busca a empresa **pelo CNPJ**. `First` ou `Single`? Justifique pelo
domínio, não pela API.

> resposta:

---

## Demo 4 — os 5 bugs plantados

**Este é o exercício mais próximo do seu trabalho real.** Os cinco compilam com 0 avisos,
rodam sem exceção, e devolvem número plausível.

Abra [`../demos/Semana03.Console/Demos/Demo4BugsPlantados.cs`](../demos/Semana03.Console/Demos/Demo4BugsPlantados.cs)
e leia **só** os blocos marcados `// ERRADO`. Para cada um: o que está errado, e qual a
consequência **em reais ou em fato fiscal**.

**4.1** Bug 1 — ICMS de 18% sobre uma nota de 200 itens.

> o que está errado:
>
> consequência:

**4.2** Bug 2 — contagem de contribuintes.

> o que está errado:
>
> consequência:

**4.3** Bug 3 — "as 3 maiores notas autorizadas".

> o que está errado:
>
> consequência:

**4.4** Bug 4 — o acumulador de total.

> o que está errado:
>
> consequência:

**4.5** Bug 5 — `Math.Round(valor, 2)`.

> o que está errado:
>
> consequência:

**4.6** Dos cinco, quantos o **compilador** pegaria? E quantos um **teste automatizado**
escrito pela mesma IA que escreveu o código pegaria?

> resposta:

---

# BLOCO B — async (`dotnet run -- async`)

## Demo 5 — vazão

**5.1** Três consultas de 800 ms, uma independente da outra.
(a) três `await` em sequência levam quanto? (b) `Task.WhenAll` leva quanto?

> resposta:

**5.2** Em (b), o que ficou "mais rápido"? Responda com precisão — a palavra errada aqui é o
erro mais comum sobre async.

> resposta:

**5.3** 64 requisições, 250 ms de I/O cada, pool estrangulado em 2 threads mínimas.
Quanto leva cada versão, e qual o **pico de esperas simultâneas** de cada uma?

| | tempo | pico de esperas simultâneas |
|---|---|---|
| `Thread.Sleep` (bloqueante) | | |
| `await Task.Delay` | | |

**5.4** `async` cria thread? Justifique.

> resposta:

**5.5** Você precisa somar 2 milhões de linhas de um CSV já carregado em memória. `async`
ajuda? Por quê?

> resposta:

---

## Demo 6 — as 4 armadilhas

**6.1** Um método `async void` lança exceção. Ele é chamado dentro de um `try/catch`.
O `catch` pega?

> resposta, e para onde a exceção vai:

**6.2** O mesmo método falhando, chamado de três jeitos. Que **tipo** de exceção cada `catch`
recebe?

| chamada | tipo da exceção |
|---|---|
| `await FalharAsync()` | |
| `FalharAsync().Result` | |
| `FalharAsync().GetAwaiter().GetResult()` | |

**6.3** Um `async Task` que faz 400 ms de **cálculo** e não tem nenhum `await`. A linha
seguinte à chamada roda antes ou depois dos 400 ms?

> resposta:

**6.4** Cinco chamadas `_ = RegistrarAuditoriaAsync(i)` num laço, cada uma com 50 ms de
espera. Quantas auditorias estão gravadas **logo depois do laço**? E 300 ms depois?

> resposta:

**6.5 — a que vale a semana.** A IA te entrega um `PageModel` com `.Result` e escreve no
comentário: *"evitei `.Result` em outros pontos porque causa deadlock em ASP.NET Core"*.
A conclusão dela está certa. **O que está errado na frase, e por que o erro importa na
prática?**

> resposta:

---

## Depois de rodar

Não apague o que previu. Marque só onde **não** bateu.

| Item | Previ | Saiu | O que eu não sabia |
|---|---|---|---|
| | | | |
