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

> resposta: 131 - 6 - 37 - 1042

**1.2** `(x, y) => y.Valor.CompareTo(x.Valor)`

> resposta: 1042 - 37 - 6 - 131

**1.3** `(x, y) => x.Numero.CompareTo(y.Numero)`

> resposta: 6 - 37 - 131 - 1042

**1.4** critério composto: CNPJ crescente e, dentro do mesmo CNPJ, valor **decrescente**

> resposta: 37 - 131 - 6 - 1042

**1.5** Sem rodar: o método `Ordenar` precisou ser alterado entre 1.1 e 1.4? Por quê?

> resposta: a assinatura do método não foi alterada "(a, b) => a.var.compareTo(b.var)", isso não mudou, mas as variáveis precisaram ser alteradas por que o resultado esperado é outro.

**1.6** Na prova você escreveu que a lambda *"não é um valor de fato"*. Esta linha compila?

```csharp
var criterios = new List<(string, Comparison<Nota>)> { ("desc", porValorDesc) };
```

> resposta e por quê: Não. Por que porValorDesc não será considerado uma função, aparentemente o C# não consegue fazer a conversão explícita de método para delegate sem que o delegate seja explícito. 
> Poderia funcionar se estivesse assim: 
```csharp
var criterios = new List<(string, Comparison<Nota>)> { ("desc", new Comparison<Nota>(PorValorDesc)) };
```

---

## Demo 2 — IReadOnlyList

**2.1** Quantos membros públicos tem `List<string>`? (ordem de grandeza serve: 5? 15? 45? 90?)

> resposta: 45

**2.2** Quantos tem `IReadOnlyList<string>`, contando os herdados?

> resposta: 5

**2.3** `Count` está em qual dos dois? E `foreach`?

> resposta: Ambos são herdados de IEnumerable<T>. Se tivesse que escolher entre List ou IReadOnlyList o count seria o único.

**2.4** Esta linha compila, sendo `Itens` do tipo `IReadOnlyList<string>`?

```csharp
nota.Itens.Add("Contrabando");
```

> resposta: Não. IReadOnlyList<T> não tem o método Add.

**2.5** E esta?

```csharp
var furado = (List<string>)nota.Itens;
furado.Add("Contrabando via cast");
```

> resposta, e o que isso diz sobre a força da garantia: Sim, compila. IReadOnlyList é um contrato de visualização em tempo de compilação, não expondo métodos que possam causar falhas em tempo de execução.

---

## Demo 3 — static entre requisições

6 requisições simultâneas, cada uma de uma empresa diferente. Cada uma grava sua empresa no
contexto, trabalha por 5–40 ms, e depois lê o contexto de volta.

**3.1** Com `ContextoAtual.EmpresaLogada` sendo um campo `static`: quantas das 6 leem a
empresa **errada**?

> resposta: 5. static irá registrar a última empresa, das 6 a última retornará para as demais 5.

**3.2** Com uma instância nova por requisição: quantas leem errado?

> resposta: Nenhuma. Cada objeto é independente.

**3.3** Antes de ver: as 6 que leem errado leem valores **diferentes** entre si, ou
**o mesmo** valor? Por quê?

> resposta: todas leem o mesmo valor errado, com vazamento de dados de uma empresa para todas as demais.

**3.4** Sua resposta na prova foi *"static tem apenas 1 instância, independente de quantos
processos"*. Se você subir **duas instâncias** da aplicação (dois processos), quantas cópias
de `ContextoAtual.EmpresaLogada` existem?

> resposta: 2 instâncias, pois serão processos separados, duas cópias de contexto.empresa.

**3.5** Um cache `static` funciona corretamente com a aplicação escalada em 3 instâncias?
O que o usuário vê?

> resposta: Não. Devido o load balancer cada clique será uma roleta e o sistema vai parecer mágico ao usuário, uma hora ele vê uma coisa e depois outra.

---

## Demo 4 — quem está segurando?

20 objetos de ~2 MB cada. Dois cenários. **Nenhum dos dois chama `Dispose` nem tem
`try..finally`.**

- **(a)** cria os 20, deixa as variáveis locais saírem de escopo, força `GC.Collect()`
- **(b)** cria os 20 e faz `_cache.Add(obj)` num `static List<>`, depois força `GC.Collect()`

**4.1** Em (a), depois do GC, a memória volta perto da linha de base?

> resposta: Sim. Todos objetos saem do escopo sem referência mantida, o GC irá limpar sozinho eles.

**4.2** Em (b), depois do GC, quanto sobra aproximadamente?

> resposta: Praticamente os ~40MB adicionados. Sendo static eles ficam presos na memória.

**4.3** A única diferença entre (a) e (b) é uma linha `_cache.Add(...)`. Nenhum dos dois
libera nada explicitamente. **Por que só um vaza?**

> resposta: (b) vaza porque a referência é mantida como static, o GC não falha, só não coleta o que ainda seria considerado vivo.

**4.4** Depois de `Cache.Limpar()` e um novo GC, a memória volta? O que exatamente foi feito
— "liberar o objeto" ou outra coisa?

> resposta: Sim, volta. A liberação foi da referência e não o objeto, portanto nada foi destruído explicitamente, apenas a lista foi liberada.

**4.5** Duas `WeakReference`, criadas no mesmo instante, para objetos do mesmo tipo: uma para
um objeto solto, outra para um objeto guardado no cache static. Depois de `GC.Collect()`,
`IsAlive` de cada uma:

> solta: Falso - Coletado
> no cache: Verdadeiro - Preso

**4.6** Complete, e é o resumo da dívida:

> Em Delphi, vazamento é "esqueci de ______". Em C#, é "esqueci de ______".
> A primeira é fácil de achar porque ______. A segunda é difícil porque ______.

> resposta: LIBERAR / SOLTAR.
>           Falta código em algum lugar / está tudo certo mas algo está preso.

---

## Depois de rodar

Não apague o que você previu. Marque abaixo somente onde a previsão **não** bateu — é essa
lista que vale, e é dela que eu monto o que reforçar.

Preenchido por mim em 12/08/2026, depois de rodar `dotnet run` e de compilar o caso do
1.6 num projeto separado. **Não editei nenhuma resposta sua** — só marquei o que não bateu.

| Item | Previ | Saiu | O que eu não sabia |
|---|---|---|---|
| 1.5 | "a assinatura não mudou, mas **as variáveis** precisaram ser alteradas" | `Ordenar` não mudou em **nada** — nem assinatura, nem corpo. Mudou o **argumento** | que o que varia é o valor passado, não o método. É a definição de inversão de controle |
| 1.6 | não compila; precisaria de `new Comparison<Nota>(...)` | **compila**, nas duas formas — com variável e com grupo de métodos | conversão implícita de grupo de métodos para delegate quando o tipo-alvo é conhecido |
| 2.3 | `Count` e `foreach` "ambos herdados de `IEnumerable<T>`" | `foreach` vem de `IEnumerable<T>`; `Count` vem de `IReadOnlyCollection<T>` | que são dois degraus diferentes da escada. E que a pergunta "em qual dos dois" tem resposta "nos dois" |
| 2.5 | cast compila porque `IReadOnlyList` "não expõe métodos que possam causar falhas em tempo de execução" | cast compila **e roda**: nota autorizada foi de 2 para 3 itens | que a garantia é do **tipo estático da referência**, não do objeto. O objeto continua sendo `List<string>` |

Sem erro, mas vale registrar: 2.1 você previu 45, saiu **46**. A pergunta pedia ordem de
grandeza e você acertou a faixa. Não conta.

---

## Correções — leia antes de fechar o bloco

### 1.5 — "as variáveis precisaram ser alteradas"

Nenhuma variável foi alterada. Entre 1.1 e 1.4 mudou **um argumento**: o valor que você
passa no segundo parâmetro de `Ordenar`. O método é literalmente o mesmo código executando.

A palavra certa importa aqui mais que na média, porque ela **é** a dívida 1. Se você
dissesse a um colega "as variáveis precisaram ser alteradas", ele iria alterar o `Ordenar`.
O certo é: *"`Ordenar` implementa o que não varia — percorrer e trocar. O critério de
comparação é passado de fora, como valor, a cada chamada."*

O caso (d) é a prova: critério composto, CNPJ e depois valor decrescente. Nenhum parâmetro
novo, nenhuma sobrecarga, nenhuma linha tocada em `Ordenar`.

### 1.6 — compila, e por dois motivos independentes

Sua resposta foi "não", e a justificativa citou conversão de grupo de métodos. Duas coisas
erradas de uma vez:

**Primeiro:** `porValorDesc` no enunciado não é um método. É uma **variável local do tipo
`Comparison<Nota>`** — exatamente como está em `Demo1InversaoDeControle.cs:112`. Uma
variável de tipo delegate é um valor comum: cabe em tupla, em lista, em campo, em retorno.
A linha do enunciado é praticamente a que já roda na demo (linhas 116-120), e ela compilou.

**Segundo, e é o que interessa:** mesmo que fosse um método, compilaria. Compilei isto:

```csharp
public static int PorValorDesc(Nota x, Nota y) => y.Valor.CompareTo(x.Valor);

var b = new List<(string, Comparison<Nota>)> { ("desc", PorValorDesc) };   // 0 erros, 0 avisos
```

C# converte grupo de métodos para delegate **implicitamente** quando o tipo-alvo é
conhecido. Aqui é: o inicializador de coleção sabe que o elemento é
`(string, Comparison<Nota>)`, a conversão de tupla se aplica elemento a elemento, e
`PorValorDesc` → `Comparison<Nota>` é uma dessas conversões implícitas.

Seu `new Comparison<Nota>(PorValorDesc)` funciona, mas é ruído. Em Delphi o `@` e o
`TComparer<T>.Construct` são obrigatórios; em C# não.

**Onde a conversão implícita realmente falha** — e é o que valia a pena saber: quando o
tipo-alvo **não** é conhecido. `var f = PorValorDesc;` compila em C# 10+, mas infere o tipo
sintetizado `Comparison<Nota>`... e `var f = Console.WriteLine;` não compila, porque há
sobrecarga e o compilador não tem como escolher. Sem tipo-alvo, não há conversão.

### 2.3 — `Count` não vem de `IEnumerable<T>`

A escada, e vale decorar porque governa toda escolha de tipo de retorno:

```text
IEnumerable<T>              -> GetEnumerator, e só. É o que o foreach usa.
  IReadOnlyCollection<T>    -> + Count
    IReadOnlyList<T>        -> + indexador [i]     (fim)
```

`List<T>` implementa as três, **e mais** `IList<T>`/`ICollection<T>` — que é onde moram
`Add`, `Remove`, `Insert`, `Clear`.

E a segunda metade da sua resposta — *"se tivesse que escolher entre `List` ou
`IReadOnlyList`, o `Count` seria o único"* — não responde a pergunta. `Count` está nos
**dois**. `foreach` está nos **dois**. Nada que `IReadOnlyList<T>` tem falta em `List<T>`;
essa é a definição de subconjunto.

Números que a demo imprimiu: `List<string>` = **46** membros públicos.
`IReadOnlyList<string>` = **5** (`Count`, `get_Count`, `Item`, `get_Item`, `GetEnumerator`).

Você previu 45 e 5. A faixa está certa, o modelo por trás não estava — foi a Q6 da prova
reaparecendo com outra roupa. **Agora fecha:** não é que a interface adiciona nada. Ela
subtrai mutação.

### 2.5 — a conclusão estava certa, a frase inviabiliza a conclusão

*"não expondo métodos que possam causar falhas em tempo de execução"* — não é isso.
`IReadOnlyList<T>` não previne falha nenhuma em tempo de execução. Ela remove `Add` do
**tipo estático da referência**, e é só isso que ela faz.

O objeto por trás continua sendo `List<string>`. O cast recupera o tipo real e devolve
mutação total — a demo imprimiu a nota **autorizada** indo de 2 para 3 itens, em runtime,
sem exceção nenhuma.

O que dizer no lugar: *"a garantia é de compilação e vale só para quem enxerga a referência
pelo tipo da interface. Impede acidente, não sabotagem. Garantia real custa alocação —
`ImmutableList<T>`, ou `.ToArray()` em cada leitura."*

### 3.1 e 3.3 — certos, e o raciocínio também. Uma ressalva

Rodei 5 vezes: **5 de 6 erraram, todas lendo `Zeta`, nas 5 execuções.** Bateu com a sua
previsão e com o seu porquê — as 6 escritas acontecem em microssegundos, o `Sleep` é de
5–40 ms, então todas as threads escrevem antes de qualquer uma ler, e a última vence.

A ressalva: **5 não é lei.** Depende de a máquina ter núcleos para as 6 threads
começarem juntas. Com menos núcleos o `Parallel.ForEach` lotearia em levas e sairiam
valores diferentes entre si — o que reforça o ponto real: o número é **não-determinístico**.
Isso é pior que errado sempre, não melhor. Errado sempre você acha em desenvolvimento;
errado às vezes chega em produção.

O resto da demo 3 e a demo 4 inteira: **todas certas, com o mecanismo certo.** 4.3, 4.4 e
4.6 estão escritas do jeito que eu usaria numa revisão de código. A dívida 4 está fechada.
