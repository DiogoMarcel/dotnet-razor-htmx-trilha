# Teoria 1 — LINQ

Leia isto antes de rodar as demos 1 a 4. É curto de propósito: o material que
ensina são as demos, e a sua previsão escrita antes delas.

---

## A frase que resume

**LINQ não é capacidade nova. É a dívida 1 do bloco de quitação, com nomes prontos.**

No bloco de quitação, `Ordenar` implementava o que não varia — percorrer e trocar — e
recebia o critério de fora, como valor. `Where` faz o mesmo:

```csharp
// aquilo
private static void Ordenar<T>(List<T> lista, Comparison<T> comparar)

// isto — mesma ideia, outro formato de delegate
public static IEnumerable<T> Where<T>(this IEnumerable<T> fonte, Func<T, bool> filtro)
```

Se você já entendeu `Comparison<T>`, você já entendeu `Func<T, bool>`. Muda o formato,
não o conceito.

### Vocabulário: `Func<>` e `Action<>`

| Tipo | Recebe | Devolve | Onde aparece |
|---|---|---|---|
| `Func<T, bool>` | um `T` | `bool` | `Where`, `Any`, `All`, `First` |
| `Func<T, TResult>` | um `T` | um `TResult` | `Select`, `OrderBy`, `Sum`, `GroupBy` |
| `Action<T>` | um `T` | **nada** | `List<T>.ForEach` |
| `Comparison<T>` | dois `T` | `int` | `List<T>.Sort` |

**A regra do `Func<>`:** o **último** parâmetro genérico é o retorno.
`Func<A, B, C>` recebe `A` e `B`, devolve `C`. `Action<>` nunca devolve.

Delphi: `Func<T, bool>` é `reference to function(const Arg: T): Boolean`. `Action<T>` é
`reference to procedure`. A separação função/procedimento que o Delphi faz na palavra-chave,
o C# faz no nome do tipo.

---

## Execução adiada — é aqui que você vai errar

**`Where` não filtra.** Ele monta o objeto que sabe filtrar e devolve. A filtragem só
acontece quando alguém percorre. **Toda vez** que alguém percorrer.

Isto não tem equivalente no Delphi que você usa. Lá, uma função que filtra uma lista
devolve a lista filtrada, pronta, ali. Aqui ela devolve uma **pergunta**.

Três consequências, e as três mordem em produção:

1. **Zero execuções** se ninguém iterar. O código parece ter rodado. Não rodou.
2. **Execução repetida.** `if (q.Any())` seguido de `foreach (var x in q)` percorre **duas
   vezes**. Com EF Core, são duas idas ao banco.
3. **A consulta enxerga a fonte no momento da ITERAÇÃO**, não no da escrita. Se a lista
   mudou entre as duas, a consulta usa a lista de agora.

A (3) é a que estraga número fiscal em silêncio: o rodapé do relatório não bate com a soma
das linhas, porque as linhas foram enumeradas num instante e o total noutro.

### A régua — olhe o tipo de retorno

| Devolve `IEnumerable<T>` → **ADIA** | Devolve valor ou coleção → **EXECUTA** |
|---|---|
| `Where` `Select` `OrderBy` `GroupBy` | `ToList` `ToArray` `ToDictionary` |
| `Take` `Skip` `Distinct` `SelectMany` | `Count` `Sum` `Min` `Max` `Average` |
| `Reverse` `Concat` `Union` | `First` `Single` `Any` `All` `Contains` |

**Uma frase:** se o retorno ainda é `IEnumerable<T>`, nada rodou.

### A decisão prática

- Vai percorrer mais de uma vez? → `.ToList()`
- Vai **devolver de um método público**? → `.ToList()`, senão quem chamar executa a consulta
  sem saber que está executando
- É passo intermediário de uma cadeia? → deixe adiado

O segundo caso é o que mais importa para você: o tipo de retorno de um método público é
contrato. `IEnumerable<T>` num retorno público diz "eu te devolvo uma promessa, execute você
quando quiser" — e quase nunca é o que se quis dizer.

---

## `GroupBy` devolve grupos, e cada grupo é uma sequência

```csharp
var relatorio = notas
    .Where(n => n.Situacao == SituacaoNota.Autorizada)
    .GroupBy(n => n.Cnpj)                       // <- a chave
    .Select(g => new LinhaRelatorio(
        Cnpj:        g.Key,                     // <- o valor agrupado
        RazaoSocial: g.First().RazaoSocial,
        Quantidade:  g.Count(),                 // <- g É uma sequência
        Total:       g.Sum(n => n.Valor),
        Maior:       g.Max(n => n.Valor)))
    .OrderByDescending(l => l.Total)
    .ToList();
```

Um grupo é um `IGrouping<TChave, TItem>` — um `IEnumerable<TItem>` com uma propriedade
`Key` colada. É por isso que `g.Sum()` e `g.Count()` funcionam dentro dele.

Delphi: você faria com `TDictionary<string, TList<TNota>>` e dois laços. Mesma estrutura;
o `GroupBy` monta o dicionário e devolve já percorrível.

---

## Sequência vazia: o que devolve, o que estoura

| Sobre lista vazia | Resultado |
|---|---|
| `Count()` | `0` |
| `Sum(...)` | `0` |
| `Max(...)` / `Min(...)` | **`InvalidOperationException`** |
| `Average(...)` | **`InvalidOperationException`** |

Não é inconsistência. `Sum` tem elemento neutro (zero); "o maior de nenhum" não tem
resposta, e devolver `0` seria **mentir**.

Em relatório fiscal isso importa: `DefaultIfEmpty(0m).Max()` faz o erro sumir e imprime
*"maior nota do mês: R$ 0,00"*. **"Não houve movimento" é um fato diferente.** A IA vai
sugerir o `DefaultIfEmpty` porque ele faz a exceção desaparecer. Recuse.

---

## `First` vs `Single` — a escolha declara o que você acredita

| | 0 itens | 1 item | 2+ itens |
|---|---|---|---|
| `First` | **estoura** | devolve | devolve o 1º |
| `FirstOrDefault` | `null` | devolve | devolve o 1º |
| `Single` | **estoura** | devolve | **estoura** |
| `SingleOrDefault` | `null` | devolve | **estoura** |

- **Busca por chave única** (CNPJ, ID) → `Single`/`SingleOrDefault`. Se vier 2, o dado está
  corrompido e você **quer** saber agora.
- **Primeiro de uma lista ordenada** → `First`/`FirstOrDefault`. Ter vários é normal.

O erro caro é `First` numa busca por chave única: no dia em que dois cadastros duplicarem o
CNPJ, o sistema escolhe um em silêncio e emite a nota no CNPJ errado.

**Código gerado por IA usa `.First()` por padrão**, porque `.First()` nunca reclama de dado
sujo. Você quer que reclame.

---

## O que cobrar numa revisão de LINQ

1. `IEnumerable<T>` devolvido de método público → exigir `.ToList()` ou justificativa
2. Consulta percorrida duas vezes (`Any()` + `foreach`) → materializar
3. `.First()` em busca por chave única → exigir `Single`
4. `Max`/`Average` sem tratar sequência vazia → perguntar o que a tela mostra sem movimento
5. `Take`/`Skip` **antes** do `Where` → ordem dos operadores é semântica
6. `GroupBy` por campo de apresentação (razão social) em vez de identificador (CNPJ)
7. `Math.Round(x, 2)` sem o terceiro argumento → o padrão do .NET é bancário, não fiscal
8. Qualquer `(double)` no caminho de dinheiro

Os itens 3 a 8 **não são erros de LINQ.** São erros de domínio escritos em LINQ, e é por
isso que nenhuma ferramenta os pega: o compilador vê tipos certos, o analisador vê sintaxe
idiomática, o revisor vê código bonito.

Quem pega é quem sabe que apuração é por estabelecimento e que imposto arredonda por item.

---

## Agora rode

```powershell
cd D:\StudieWithAI\semana-03\demos\Semana03.Console
dotnet run -- linq
```

**Antes:** preencha as seções 1 a 4 de [`Exercícios/PREVISOES.md`](Exercícios/PREVISOES.md).
Sem a previsão escrita, a demo vira leitura e você concorda com tudo.
