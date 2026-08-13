# Teoria 2 — async/await

Leia antes de rodar as demos 5 e 6.

---

## A frase que precisa ficar certa

**`async` não deixa nada mais rápido. Ele devolve a thread enquanto espera.**

Uma consulta de 800 ms leva 800 ms com ou sem `async`. O que muda é que, durante esses
800 ms, **ninguém fica parado segurando um recurso caro só para olhar a rede**.

Em Delphi desktop isso quase não importa: uma thread principal, um usuário. Num servidor
web, o pool de threads é o recurso finito que separa "atende 500 usuários" de "atende 50" —
no mesmo hardware, com o mesmo banco, com a mesma latência.

A palavra certa é **vazão** (throughput), não velocidade. Se você disser "async deixa mais
rápido" numa especificação, quem ler vai usar async para acelerar cálculo — onde não
adianta nada.

---

## As quatro afirmações que a maioria erra

1. **`async` NÃO cria thread.** Nenhuma, nunca. Ele reescreve o método numa máquina de
   estados que sabe pausar e continuar. Quem continua é uma thread do pool que já existia —
   e pode não ser a mesma que começou.

2. **`await` NÃO bloqueia.** Ele devolve a thread e registra "quando terminar, continue
   daqui". Quem bloqueia é `.Result`, `.Wait()` e `Thread.Sleep`.

3. **Uma `Task` já está rodando quando nasce.** Não existe `Start` a chamar. É por isso que
   dá para disparar três e esperar as três depois.

4. **async serve para I/O** — rede, disco, banco. Para cálculo puro não há espera a
   devolver; ali é `Task.Run`, que é paralelismo, outra conversa.

Delphi: você conhece `TThread` + `Synchronize` — "rode noutra thread e volte à principal
para tocar na UI". `await` faz a volta sozinho **e sem thread nova**. O modelo que não tem
equivalente no seu Delphi é este: **esperar sem ocupar ninguém**.

---

## Sequencial vs paralelo — um `await` no lugar errado

```csharp
// SEQUENCIAL — 2400 ms. Cada await espera o anterior terminar.
var sp = await ConsultarAsync("SP");
var mg = await ConsultarAsync("MG");
var rj = await ConsultarAsync("RJ");

// PARALELO — 800 ms. Dispara as três, DEPOIS espera.
var tSp = ConsultarAsync("SP");
var tMg = ConsultarAsync("MG");
var tRj = ConsultarAsync("RJ");
var resultados = await Task.WhenAll(tSp, tMg, tRj);
```

A diferença é onde o `await` está. O compilador não avisa, o código continua **correto** —
só fica 3x mais lento.

**Quando NÃO paralelizar:** se `b` precisa do resultado de `a`, sequencial é o certo.
`Task.WhenAll` só para trabalho independente.

---

## As quatro armadilhas

### 1. `async void`

Um `async Task` guarda a exceção **dentro da Task**, e ela salta quando você faz `await`.
Um `async void` não tem Task — não há onde guardar. O runtime joga a exceção no
`SynchronizationContext` capturado no início do método.

**Em ASP.NET Core não existe `SynchronizationContext`.** A exceção vai direto ao thread pool
e **derruba o processo**. Não é erro 500 numa requisição — é a aplicação inteira caindo.

`async void` só em event handler de UI. Em servidor, nunca.

### 2. `.Result` e `.Wait()` — e a mentira que você vai ter que recusar

```csharp
await MetodoAsync();                       // InvalidOperationException
MetodoAsync().Result;                      // AggregateException  <- seu catch não pega
MetodoAsync().GetAwaiter().GetResult();    // InvalidOperationException
```

`.Result` embrulha em `AggregateException` porque uma Task pode ter falhado por vários
motivos. Consequência: seu `catch (InvalidOperationException)` **não pega**, e a causa fica
escondida em `.InnerException`.

**A mentira:** *"nunca use `.Result` em ASP.NET Core, causa deadlock"*. A conclusão está
certa. **O motivo está errado**, e o motivo errado te faz procurar no lugar errado.

- O deadlock clássico exigia um `SynchronizationContext` de thread única: ASP.NET
  **Framework**, WinForms, WPF. A thread bloqueava no `.Result` esperando a continuação, e a
  continuação esperava a thread.
- **ASP.NET Core não tem `SynchronizationContext`.** Esse deadlock não acontece.
- O que acontece é pior de diagnosticar: cada `.Result` prende uma thread do pool, e sob
  carga a aplicação morre de **inanição de threads**. Sem travar, sem erro, só ficando lenta
  até cair.

Quem acredita em "deadlock" procura contenção de lock, não acha nada, e conclui que é o
banco. Quem sabe que é inanição olha a contagem de threads do pool e acha em cinco minutos.

### 3. `async` sem `await`

Um método `async` roda **síncrono** na thread do chamador até o primeiro `await` que
realmente precise esperar. Sem `await` nenhum, roda inteiro, síncrono, e devolve uma Task já
pronta. O nome `OnGetAsync` mente; o perfil de carga não.

É a única das quatro que o compilador avisa — **CS1998**. Se a IA te entregar código com
CS1998 suprimido, pergunte por quê.

### 4. Fire-and-forget (`_ = MetodoAsync()`)

A Task existe, roda, e ninguém guarda referência nem observa a falha. Num PageModel, quando
a resposta HTTP é enviada o **escopo da requisição** encerra: o `DbContext` que a Task ia
usar já foi descartado, e você recebe `ObjectDisposedException` num log sem stack trace de
requisição nenhuma. (Semana 7.)

`_ =` na frente de uma chamada async é bandeira vermelha: alguém calou um aviso do
compilador em vez de responder à pergunta que ele fazia.

---

## Checklist de revisão — código async gerado por IA

| Ver | Exigir |
|---|---|
| `async void` | `async Task` (exceto handler de UI) |
| `.Result` / `.Wait()` | `await` até a raiz |
| `_ = MetodoAsync()` | quem observa a falha? |
| `async` sem `await` | tirar o `async`, ou justificar |
| `await` em fila, tarefas independentes | `Task.WhenAll` |
| sem `CancellationToken` | exigir; requisição abortada deve **parar** o trabalho |

**A regra que resume:** async é contagioso de baixo para cima. Se o repositório é async, o
serviço é async e o handler é async. O lugar onde alguém bloqueia é exatamente o lugar onde
a cadeia foi quebrada por preguiça.

---

## Por que a IA erra async com frequência alta

O corpus de treino dela está cheio de ASP.NET **Framework** (2012–2018), onde as regras eram
outras: havia `SynchronizationContext`, `ConfigureAwait(false)` era obrigatório em
biblioteca, e `.Result` travava de verdade. Muito desse conselho ainda circula como se fosse
atual.

Este é um dos poucos temas em que **você vai precisar corrigir a ferramenta com fonte na
mão**, e a fonte é a de sempre: [Stephen Cleary — Async/Await Best
Practices](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming)
mais a documentação atual do ASP.NET Core.

---

## Agora rode

```powershell
cd D:\StudieWithAI\semana-03\demos\Semana03.Console
dotnet run -- async
```

**Antes:** preencha as seções 5 e 6 de [`Exercícios/PREVISOES.md`](Exercícios/PREVISOES.md).
