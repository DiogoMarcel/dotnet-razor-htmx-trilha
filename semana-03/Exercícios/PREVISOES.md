# Previsões — Semana 3

**Escreva aqui ANTES de rodar `dotnet run`.** Esta folha é o exercício; as demos são a
conferência.

No bloco de quitação você acertou 17 de 21 escrevendo antes. Os 4 erros só apareceram
**porque** havia previsão escrita — sem ela, você teria lido a explicação, concordado, e
saído achando que sabia as quatro.

**"Não sei" é resposta válida e útil.** Chute certo por sorte vira dívida silenciosa.

**Regra em vigor desde 12/08:** resposta que reformula o enunciado conta como não-resposta.
Se a sua frase não permitiria a outra pessoa implementar a coisa, ela não está pronta.

## Como conferir

Cada demo termina com um bloco **`GABARITO DA DEMO N`**, na numeração desta folha. Você lê
de cima para baixo e marca certo/errado sem ter que caçar a resposta no meio da narração.

```powershell
dotnet run -- 3          # roda a demo 3 e imprime o gabarito 3.1 a 3.6
```

Os números do gabarito são **recalculados**, não copiados: se eu mexer na massa ou num
filtro, ele acompanha. Gabarito que mente é pior que gabarito nenhum.

Os itens conceituais (1.4, 3.4, 5.2, 6.5) trazem resposta **de referência**, curta de
propósito — ela diz o que é certo; o porquê está na narração da demo, logo acima. Se a sua
resposta chegou no mesmo lugar com outras palavras, **não marque como erro na hora**: traga
para eu julgar. Palavra diferente pode ser sinônimo ou pode ser modelo diferente, e essa
distinção é exatamente o que você ainda não faz sozinho.

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

**1.1** Notas **autorizadas de julho/2026**, ordenadas **por valor, do maior para o menor**.
Escreva os **números das notas** na ordem em que saem.

> resposta anterior (enunciado ambíguo, corrigido em 13/08): 1012, 1010, 1009, 1007, 1005, 1004, 1002, 1001
>
> resposta: 1010, 1004, 1012, 1005, 1009, 1007, 1001, 1002

> **Nota da correção.** O enunciado dizia só "da maior para a menor", sem dizer maior o quê.
> Você ordenou por número da nota — leitura legítima do que estava escrito, e defeito meu.
> **O conjunto você acertou:** as 8 são exatamente essas, com 1003 e 1011 fora por canceladas,
> 1008 por em digitação, e 1006 por ser de agosto. Essa era a parte que a pergunta media.
> Refaça só a ordem.

**1.2** O total dessas notas.

> resposta: 53.295,75  ✅ confere com a saída da demo

**1.3** `Func<NotaFiscal, decimal>` — o que ele recebe e o que devolve? E `Action<NotaFiscal>`?

> resposta: para eu fixar anotei desta forma: FUNC<o_que_recebe,ultimo_retorno>, portanto recebe uma NotaFiscal e devolve o decimal.
>           já o Action<Apenas_recebe>, não há retornos.

**1.4** Sem rodar: qual a relação entre `Where(Func<T, bool>)` e o `Ordenar(List<T>,
Comparison<T>)` do bloco de quitação? Responda em uma frase que **não** use a palavra "lambda".

> resposta: ambos usam delegate como parametros. where apenas filtra valores, ordenar altera o resultado conforme o critério. Alterar neste caso é ajustar a ordem com um critério conforme foi informado.
> resposta adicional: ambos utilizam a mesma mecânica de inversão de controle, o método implementa a ação fixa, depois recebe a variável passado por valor.

**1.4.1** Where e Ordenar fazem coisas diferentes — e fazem. Então por que os dois precisam receber código de fora? O que há de igual no problema que os dois resolvem?

> resposta: ambos tem a mesma questão/problema estrutural, precisam separar o mecanismo da biblioteca, que é escrito uma única vez e não varia, da regra específica que varia.

**1.4.2** Complete: "Ordenar implementa ______ e recebe de fora ______." Depois a mesma frase para Where.

> resposta: Ordenar implementa o que não varia e recebe de fora critério valor. < aqui percorre e troca
> resposta: Where implementa percorrer e recebe de fora critério variável. < aqui percorre e deixa passar

**1.4.3** O teste de verdade: Sum(n => n.Valor) também recebe delegate. O que Sum implementa, e o que você fornece? Se a sua resposta às três tiver a mesma forma, o modelo está lá.

> resposta: O sum percorre e soma. Preciso fornecer um delegate FUNC<T, decimal>. É a mesma forma de modelo, com laço fixo que recebe o comportamento variável que é a regra.

---

## Demo 2 — execução adiada

A consulta é sempre a mesma:

```csharp
notas.Where(n => n.Situacao == SituacaoNota.Autorizada)
```

O filtro está instrumentado: **cada vez que a lambda é avaliada, um contador sobe.** A massa
tem **12** notas, na ordem da tabela acima.

**2.1** `var q = notas.Where(...);` e mais nada. Contador?

> resposta: 0

**2.2** Depois de **um** `foreach` sobre `q`. Contador?

> resposta: 12

**2.3** Depois de um **segundo** `foreach` sobre a **mesma** variável `q`. Contador?

> resposta: 24

**2.4** `notas.Where(...).ToList()` e depois **dois** `foreach` sobre a lista. Contador?

> resposta: 12

**2.5** `notas.Where(...).First()`. Contador?

> resposta, e por quê: 1. A rotina FIRST ao encontrar o primeiro registro da massa ele para e não avalia as demais.

**2.5.1** Se a NF 1001 fosse Cancelada, qual seria o contador?

> resposta: Continuaria 1.

> **Enunciado ambíguo, corrigido em 13/08.** Não dizia *quando* a 1001 seria cancelada.
> Você leu como "cancelar depois de rodar o First" — leitura legítima, ainda mais vindo
> logo depois do 2.6. E sob essa leitura a sua resposta está **certa**, pelo motivo certo:
> `First` executa na hora e devolve valor concreto, então mudar a fonte depois não altera
> nem o contador nem o resultado. Refeita abaixo.

**2.5.2** Massa **diferente**, mesma consulta. Imagine que a NF 1001 já nasceu **Cancelada**
— as outras 11 permanecem exatamente como na tabela. Roda-se, nessa massa:

```csharp
notas.Where(n => n.Situacao == SituacaoNota.Autorizada).First()
```

Qual o valor do contador, e qual nota é devolvida?

> resposta: o contador retorna 2, pois first é o executor do where, que irá falhar na primeira consulta da nota, somando 1 no contador, o where executa novamente para validar a segunda nota e incrementa o contador. A nota retornada será a 1002.

**2.6** A consulta `notas.Where(n => n.Valor > 10_000m)` é escrita. **Depois** disso alguém
faz `notas.Add(...)` de uma nota de 99.000,00. A nota nova aparece no resultado?
E se a consulta tivesse `.ToList()` no fim?

> resposta: a consulta adiada a nova nota irá aparecer. Em .ToList() não, a lista de notas foi fixada ao executar o ToList() antes de adicionar a nova nota.

**2.7** Sem consultar e sem rodar nada: você está revisando um código e precisa saber, de
cada linha abaixo, se ela **já executou a consulta** ou se ela **só montou a pergunta e não
rodou nada ainda**.

```csharp
notas.Where(n => n.Valor > 1_000m)         // (a)
notas.OrderByDescending(n => n.Valor)      // (b)
notas.GroupBy(n => n.Cnpj)                 // (c)
notas.Count()                              // (d)
notas.Any(n => n.Valor > 10_000m)          // (e)
notas.Where(n => ...).ToList()             // (f)
```

**a)** Classifique as seis.

> resposta:
 (a) adiado
 (b) adiado
 (c) adiado
 (d) executou
 (e) executou
 (f) executou


**b)** Agora escreva **a regra geral** que você usou — uma frase que classifique qualquer
operador de LINQ, inclusive um que você nunca viu. Diga **o que você olha** na linha e **o
que nesse lugar** separa os dois casos.

> resposta: não entendi a questão.

**b.1) — refeita em 13/08, com o dado na mesa.** Você acertou as seis de (a). Esta é a tabela
do que cada um desses métodos **declara devolver**:

| | linha | o método devolve |
|---|---|---|
| (a) | `Where(...)` | `IEnumerable<NotaFiscal>` |
| (b) | `OrderByDescending(...)` | `IOrderedEnumerable<NotaFiscal>` |
| (c) | `GroupBy(...)` | `IEnumerable<IGrouping<string, NotaFiscal>>` |
| (d) | `Count()` | `int` |
| (e) | `Any(...)` | `bool` |
| (f) | `ToList()` | `List<NotaFiscal>` |

Olhando **só a coluna da direita**: o que as três de cima têm que as três de baixo não têm?

Responda em uma frase, e ela é a regra. Depois teste nestes dois, que não estão na tabela:

```csharp
notas.Take(5)                       // devolve IEnumerable<NotaFiscal>
notas.ToDictionary(n => n.Numero)   // devolve Dictionary<int, NotaFiscal>
```

> resposta: as 3 acima retornam a interface. as 3 abaixo retornam o valor concreto.
take irá executar adiado
todictionary irá executar imediatamente.

> resposta: sem executar avaliando apenas o código é possível identificar o tipo do retorno. Se é notas.Where(Contando) é adiado. Se for notas.Where(Contando).ToList() - materializado.

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

> Esta demo usa uma massa **ampliada**: as 12 da tabela acima mais duas, e as duas existem
> só para os bugs 2 e 3 morderem. A NF **2001** é uma filial — mesma razão social do
> `11222333000181`, CNPJ `11222333000272`. A NF **2002** é uma cancelada de valor alto. Não
> se assuste ao vê-las no arquivo; não estão na tabela porque não pertencem às demos 1 a 3.

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

O tempo depende da máquina — para ele, **ordem de grandeza basta** (250 ms? 1 s? 10 s?).
O **pico de esperas simultâneas** não depende: é consequência do mecanismo, e é o número
que a pergunta realmente cobra.

| | tempo (ordem de grandeza) | pico de esperas simultâneas |
|---|---|---|
| `Thread.Sleep` (bloqueante) | | |
| `await Task.Delay` | | |

E depois, em uma frase: **por que** o pico de uma é o que é, e o da outra é 64?

> resposta:

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
