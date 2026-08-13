# Prova de conhecimento — Semana 3

**Responda sem consultar as demos, a teoria ou a internet.** Se não souber, escreva "não
sei" — vale mais que chute certo, que vira dívida silenciosa.

**Regra em vigor desde 12/08/2026:** resposta que reformula o enunciado conta como
não-resposta. A pergunta a se fazer antes de dar cada resposta por pronta: *"outra pessoa
conseguiria implementar a coisa certa lendo só isto?"*

12 questões. Faça de uma vez.

---

## Parte A — LINQ

### Q1

Explique, para um colega que só conhece Delphi, **por que** `Where` recebe código como
parâmetro em vez de existirem `WhereAutorizadas`, `WhereDeJulho`, `WhereAcimaDe`.

Uma resposta que só diga "para ser genérico" não responde. Diga **o que fica no método** e
**o que vem de fora**.

> resposta:

### Q2

```csharp
var q = notas.Where(n => n.Valor > 1000m);
notas.Add(new NotaFiscal(9999, ..., 50_000m, SituacaoNota.Autorizada));
Console.WriteLine(q.Count());
```

A nota 9999 entra na contagem? **Explique o mecanismo**, não o resultado.

> resposta:

### Q3

Um método público de um serviço vai devolver as notas rejeitadas de um lote para a tela
montar a tabela.

**a)** Qual tipo de retorno, e por quê?
**b)** Se você devolver `IEnumerable<NotaFiscal>` sem materializar, o que pode dar errado
para quem chamar?

> resposta:

### Q4

Complete a tabela **de cabeça**:

| | 0 itens | 1 item | 2+ itens |
|---|---|---|---|
| `First` | | | |
| `FirstOrDefault` | | | |
| `Single` | | | |
| `SingleOrDefault` | | | |

E depois: um método `ObterEmpresaPorCnpj(string cnpj)`. Qual dos quatro, e o que a sua
escolha **declara** sobre o que você acredita a respeito do banco?

> resposta:

### Q5

`Sum` sobre lista vazia devolve `0`. `Max` sobre lista vazia estoura. **Não é
inconsistência.** Qual o critério que separa os dois?

> resposta:

### Q6

Este trecho compila, roda, e devolve menos linhas do que deveria:

```csharp
var top3 = notas
    .OrderByDescending(n => n.Valor)
    .Take(3)
    .Where(n => n.Situacao == SituacaoNota.Autorizada)
    .ToList();
```

**a)** Qual o defeito?
**b)** Escreva a versão certa.
**c)** Por que nenhuma ferramenta pega isso?

> resposta:

---

## Parte B — async

### Q7

Complete, e a palavra importa:

> `async` não deixa nada mais ______. Ele ______ enquanto espera.
> A métrica que melhora chama-se ______, não velocidade.

Depois explique, em duas frases, por que essa distinção quase não importa em Delphi desktop
e importa muito num servidor web.

> resposta:

### Q8

```csharp
var sp = await ConsultarAsync("SP");   // 800 ms
var mg = await ConsultarAsync("MG");   // 800 ms
var rj = await ConsultarAsync("RJ");   // 800 ms
```

**a)** Quanto tempo leva, e por quê?
**b)** Reescreva para levar ~800 ms.
**c)** Em que situação a versão sequencial é a **correta** e a paralela seria um bug?

> resposta:

### Q9

Um método `async void` lança uma exceção. Ele foi chamado de dentro de um `try/catch`
em ASP.NET Core.

**a)** O `catch` pega?
**b)** Para onde a exceção vai, e qual o efeito na aplicação?
**c)** Qual é a assinatura certa, e qual é a única exceção legítima à regra?

> resposta:

### Q10

Alguém escreve `var resultado = ConsultarAsync().Result;` num `PageModel` de ASP.NET Core.

**a)** Isso causa deadlock? Responda sim ou não, e justifique com o mecanismo.
**b)** O que acontece de fato, sob carga?
**c)** Além do problema de vazão, o que muda no **tratamento de erro**?

Esta é a questão que separa "repetiu o que leu na internet" de "entendeu".

> resposta:

### Q11

```csharp
public async Task<decimal> ApurarAsync(IReadOnlyList<NotaFiscal> notas)
{
    decimal total = 0m;
    foreach (var n in notas)
        total += Math.Round(n.Valor * 0.18m, 2, MidpointRounding.AwayFromZero);
    return total;
}
```

**a)** Este método é assíncrono na prática? Justifique.
**b)** O compilador diz alguma coisa? O quê?
**c)** O que fazer com ele.

> resposta:

---

## Parte C — juízo

### Q12

Um requisito fiscal: *"o relatório mostra o ICMS de cada item da nota e o total da nota"*.

A IA entrega:

```csharp
var totalIcms = Math.Round(itens.Sum(i => i.Valor * aliquota), 2);
```

**a)** Há **dois** defeitos nessa linha. Quais?
**b)** Para cada um, qual é a consequência **fiscal** — não a técnica?
**c)** Escreva a linha certa.

> resposta:

---

## Ao terminar

Traga esta prova e o `PREVISOES.md` preenchido. A correção vai em `semana-03/Corrigir.txt`,
numerada, cada item com **o que está errado + por quê + o certo**.
