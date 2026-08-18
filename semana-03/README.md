# Semana 3 — LINQ e async

**Objetivo:** LINQ e `async/await` são onipresentes em .NET. Sem eles você escreve C# com
sotaque de Delphi e vai apanhar em EF Core (Semana 7) e no Razor Pages (Semana 5).

São **dois modelos mentais diferentes**, então a semana vem em **dois blocos auto-contidos**.
Cada um entrega valor sozinho. Não precisa terminar os dois na mesma sessão.

---

## Ordem — e ela não é sugestão

| # | O que | Onde | Quem faz |
|---|---|---|---|
| **A1** | Teoria de LINQ | [teoria-01-linq.md](teoria-01-linq.md) | você lê |
| **A2** | **Escrever as previsões do bloco A** | [Exercícios/PREVISOES.md](Exercícios/PREVISOES.md) | **você escreve** |
| **A3** | Rodar as demos 1 a 4 | `dotnet run -- linq` | a máquina roda, você compara |
| **B1** | Teoria de async | [teoria-02-async.md](teoria-02-async.md) | você lê |
| **B2** | **Escrever as previsões do bloco B** | [Exercícios/PREVISOES.md](Exercícios/PREVISOES.md) | **você escreve** |
| **B3** | Rodar as demos 5 e 6 | `dotnet run -- async` | a máquina roda, você compara |
| **C1** | O que exigir e o que recusar | [Exercícios/exigir-ou-recusar.md](Exercícios/exigir-ou-recusar.md) | você responde |
| **C2** | Prova de conhecimento | [Exercícios/prova-semana-03.md](Exercícios/prova-semana-03.md) | você responde |

**Se você rodar antes de prever, a demo vira leitura.** Você concorda com tudo e sai sem
saber o que não sabia. O bloco de quitação provou o contrário: 17 de 21 acertos, e os 4
erros só existiram como aprendizado **porque** havia previsão escrita antes.

---

## Rodar

```powershell
cd D:\StudieWithAI\semana-03\demos\Semana03.Console
dotnet run              # todas as 6
dotnet run -- linq      # demos 1 a 4
dotnet run -- async     # demos 5 e 6
dotnet run -- 4         # uma específica
```

Compila com **0 avisos**. As 6 demos são comentadas linha a linha — o código é material
didático, não exercício de digitação.

Cada demo termina com um bloco **`GABARITO DA DEMO N`**, na mesma numeração do
[`PREVISOES.md`](Exercícios/PREVISOES.md). Serve para conferir item a item sem caçar a
resposta no meio da narração. Os números são **recalculados** a cada execução, não copiados —
se a massa mudar, o gabarito acompanha.

---

## As demos

| # | Bloco | O que mostra |
|---|---|---|
| 1 | linq | O mesmo relatório em laço e em LINQ, resultado idêntico. LINQ é a **dívida 1** com nomes prontos |
| 2 | linq | **Execução adiada** — o susto da semana. Contador de avaliações, fonte que muda, e a régua adia/executa |
| 3 | linq | `GroupBy` fiscal, os agregados que estouram com sequência vazia, e a matriz `First`/`Single` |
| 4 | linq | **5 bugs plantados.** Compilam, rodam, mentem. É o treino literal de revisar código de IA |
| 5 | async | Vazão vs velocidade, medido: 64 esperas simultâneas com async, 8 sem |
| 6 | async | As 4 armadilhas — `async void`, `.Result`, `async` sem `await`, fire-and-forget |

---

## As 5 ideias que precisam ficar

1. **LINQ é inversão de controle com nomes prontos.** `Where` implementa percorrer; você
   fornece o que significa "passar". É a dívida 1, fechada em 12/08.
2. **Execução adiada: a consulta é uma pergunta, não uma resposta.** Ela roda quando alguém
   percorre, toda vez, e enxerga a fonte daquele instante.
3. **`First` vs `Single` declara o que você acredita sobre o dado.** Chave única pede
   `Single`; código de IA usa `First` porque `First` nunca reclama de dado sujo.
4. **`async` não cria thread e não acelera nada.** Ele devolve a thread durante a espera. A
   palavra é **vazão**.
5. **`.Result` em ASP.NET Core não causa deadlock — causa inanição de threads.** A conclusão
   ("não use") é a mesma; o mecanismo é outro, e o mecanismo errado te faz depurar no lugar
   errado.

---

## Onde isto volta

| Semana | Como |
|---|---|
| 5 — Razor Pages | `OnGetAsync`/`OnPostAsync`; handler que bloqueia trava a requisição |
| 7 — EF Core | O `Where` adiado vira `WHERE` no SQL. `.ToList()` no lugar errado vira `SELECT *` e filtro em C# — 40 ms contra 40 segundos |
| 7 — EF Core | Fire-and-forget com `DbContext` já descartado: `ObjectDisposedException` sem stack trace de requisição |
| 10 — Segurança | Inanição de thread sob carga real, com duas instâncias |

---

## O que NÃO entra nesta semana

`IAsyncEnumerable`, `ConfigureAwait`, `ValueTask`, `Parallel.ForEachAsync`, e LINQ com
sintaxe de consulta (`from x in y select`). Todos são reais e nenhum é necessário agora —
sintaxe de consulta em particular só divide sua atenção: a de método é a que você vai ler em
99% do código.

---

## Ao terminar

Traga o `PREVISOES.md` preenchido, o `exigir-ou-recusar.md` e a prova. A correção vai em
`semana-03/Corrigir.txt`.
