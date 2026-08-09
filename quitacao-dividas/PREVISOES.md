# Previsões — bloco de quitação

**Escreva aqui ANTES de rodar `dotnet run`.** Esta folha é o exercício; as demos são só a
conferência.

Por que a ordem importa: reconhecer a resposta certa é fácil, produzi-la é que mede. Se você
rodar primeiro, vai concordar com tudo e não vai saber o que não sabia. Foi exatamente o que
a correção da Semana 2 mostrou — você consolidou os conceitos em que **voltou com o mecanismo
na mão**, não os que leu.

**"Não sei" é resposta válida e útil.** Chute certo por sorte vira dívida silenciosa.

---

## Demo 1 — inversão de controle

A massa, na ordem em que foi criada:

| NF | CNPJ | Valor |
|---|---|---|
| 131 | 11222333000181 | 95,90 |
| 1042 | 45612378000105 | 38.910,50 |
| 37 | 11222333000181 | 2.075,00 |
| 6 | 33445566000199 | 420,00 |

O mesmo método `Ordenar` é chamado 4 vezes, com critérios diferentes. Escreva a ordem dos
**números de nota** que sai em cada caso.

**1.1** `(x, y) => x.Valor.CompareTo(y.Valor)`

> resposta:

**1.2** `(x, y) => y.Valor.CompareTo(x.Valor)`

> resposta:

**1.3** `(x, y) => x.Numero.CompareTo(y.Numero)`

> resposta:

**1.4** critério composto: CNPJ crescente e, dentro do mesmo CNPJ, valor **decrescente**

> resposta:

**1.5** Sem rodar: o método `Ordenar` precisou ser alterado entre 1.1 e 1.4? Por quê?

> resposta:

**1.6** Na prova você escreveu que a lambda *"não é um valor de fato"*. Esta linha compila?

```csharp
var criterios = new List<(string, Comparison<Nota>)> { ("desc", porValorDesc) };
```

> resposta e por quê:

---

## Demo 2 — IReadOnlyList

**2.1** Quantos membros públicos tem `List<string>`? (ordem de grandeza serve: 5? 15? 45? 90?)

> resposta:

**2.2** Quantos tem `IReadOnlyList<string>`, contando os herdados?

> resposta:

**2.3** `Count` está em qual dos dois? E `foreach`?

> resposta:

**2.4** Esta linha compila, sendo `Itens` do tipo `IReadOnlyList<string>`?

```csharp
nota.Itens.Add("Contrabando");
```

> resposta:

**2.5** E esta?

```csharp
var furado = (List<string>)nota.Itens;
furado.Add("Contrabando via cast");
```

> resposta, e o que isso diz sobre a força da garantia:

---

## Demo 3 — static entre requisições

6 requisições simultâneas, cada uma de uma empresa diferente. Cada uma grava sua empresa no
contexto, trabalha por 5–40 ms, e depois lê o contexto de volta.

**3.1** Com `ContextoAtual.EmpresaLogada` sendo um campo `static`: quantas das 6 leem a
empresa **errada**?

> resposta:

**3.2** Com uma instância nova por requisição: quantas leem errado?

> resposta:

**3.3** Antes de ver: as 6 que leem errado leem valores **diferentes** entre si, ou
**o mesmo** valor? Por quê?

> resposta:

**3.4** Sua resposta na prova foi *"static tem apenas 1 instância, independente de quantos
processos"*. Se você subir **duas instâncias** da aplicação (dois processos), quantas cópias
de `ContextoAtual.EmpresaLogada` existem?

> resposta:

**3.5** Um cache `static` funciona corretamente com a aplicação escalada em 3 instâncias?
O que o usuário vê?

> resposta:

---

## Demo 4 — quem está segurando?

20 objetos de ~2 MB cada. Dois cenários. **Nenhum dos dois chama `Dispose` nem tem
`try..finally`.**

- **(a)** cria os 20, deixa as variáveis locais saírem de escopo, força `GC.Collect()`
- **(b)** cria os 20 e faz `_cache.Add(obj)` num `static List<>`, depois força `GC.Collect()`

**4.1** Em (a), depois do GC, a memória volta perto da linha de base?

> resposta:

**4.2** Em (b), depois do GC, quanto sobra aproximadamente?

> resposta:

**4.3** A única diferença entre (a) e (b) é uma linha `_cache.Add(...)`. Nenhum dos dois
libera nada explicitamente. **Por que só um vaza?**

> resposta:

**4.4** Depois de `Cache.Limpar()` e um novo GC, a memória volta? O que exatamente foi feito
— "liberar o objeto" ou outra coisa?

> resposta:

**4.5** Duas `WeakReference`, criadas no mesmo instante, para objetos do mesmo tipo: uma para
um objeto solto, outra para um objeto guardado no cache static. Depois de `GC.Collect()`,
`IsAlive` de cada uma:

> solta:
> no cache:

**4.6** Complete, e é o resumo da dívida:

> Em Delphi, vazamento é "esqueci de ______". Em C#, é "esqueci de ______".
> A primeira é fácil de achar porque ______. A segunda é difícil porque ______.

> resposta:

---

## Depois de rodar

Não apague o que você previu. Marque abaixo somente onde a previsão **não** bateu — é essa
lista que vale, e é dela que eu monto o que reforçar.

| Item | Previ | Saiu | O que eu não sabia |
|---|---|---|---|
| | | | |
