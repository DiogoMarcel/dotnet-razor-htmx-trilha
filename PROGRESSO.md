# PROGRESSO — fonte única de verdade

> **Este arquivo é o estado do estudo.** Cowork e Claude Code leem daqui antes de responder qualquer coisa, e escrevem aqui ao fim de cada sessão. Se este arquivo estiver desatualizado, as duas ferramentas dão conselho errado.

**Aluno:** Diogo Marcel · **Início:** 26/07/2026 · **Ritmo alvo:** 10–15h/semana
**Última atualização:** 13/08/2026

---

## Situação atual

| | |
|---|---|
| **Bloco ativo** | **Semana 03 — LINQ e async.** ✅ material entregue em 13/08. Aguardando as previsões dele |
| **Fase** | 1 — Fundação |
| **Bloqueio** | Nenhum |
| **Pendência aberta** | Nenhuma dívida de compreensão. **As 5 fecharam em 12/08** |
| **Próximo checkpoint** | Sabatina da Fase 1, depois da Semana 04 |
| **Formato** | **Mudou em 06/08, mudou de novo em 07/08.** Ver `CLAUDE.md` › "O que ele veio buscar" |
| **Ritmo** | **Carga variável, ele não sabe quanto tem.** Blocos auto-contidos, sem orçamento de horas. Conteúdo inteiro, prazo elástico (16–18 blocos) |

---

## Semana 01 — Como a web funciona · ✅ concluída (com ressalvas)

**Entregue**

- `Exercícios/resposta_stateless.txt` — explicação de HTTP stateless + 3 soluções (cookies, tokens, sessão). **Aprovado.** Resposta correta e com trade-offs, não decorada.
- `Exercícios/cadastro_empresa.html` — formulário de cadastro de empresa
- `prototipo/` — index, cadastro, notas, detalhe + `css/estilos.css`

**Correções apontadas** (`semana-01/Corrigir.txt`) — 4 itens:

1. ✅ Estilo inline em `detalhe.html` linha 64 → aplicado no protótipo (`display:flex` + `gap`)
2. ✅ Erro de CNPJ não anunciado → `aria-invalid`/`aria-describedby` aplicados no protótipo **e** compreendidos, fechado em 12/08 no bloco de quitação
3. ✅ `.dados dd { text-align: right }` → removido
4. ✅ Exercício 4 (grid CEP 3 + Logradouro 6 + Número 3 = 12) → ele conferiu, e reconheceu o padrão de 12 colunas por experiência. `estilos.css:351` confirma `repeat(12, 1fr)`

> **Fechado em 12/08/2026.** O que a verificação achou: as 3 correções de código estavam
> aplicadas no `prototipo/`, mas o arquivo que **ele** escreveu
> (`Exercícios/cadastro_empresa.html`) seguia com zero `aria-*` — o protótipo estava certo
> porque a ferramenta o escreveu. O exercício da dívida 5 usou o código dele, não o meu.

**Avaliação:** base de HTTP sólida. HTML/CSS funcional, com lacuna em acessibilidade — que não é detalhe em sistema usado 8h/dia por operador fiscal.

---

## Semana 02 — C# fundamentos (parte 1) · ✅ concluída em 07/08/2026

**Material disponível**

| # | Arquivo | Tempo | Status |
|---|---|---|---|
| 0 | Instalar SDK .NET 10 (`winget install Microsoft.DotNet.SDK.10`) | 15min | ✅ |
| 1 | `teoria-01-tipos.md` | 2h | ✅ |
| 2 | `teoria-02-oo.md` | 2h30 | ✅ |
| 3 | `teoria-03-colecoes.md` | 2h | ✅ |
| 4 | `projeto/GUIA-PROJETO.md` — FiscalLab console | 5h | ✅ entregue pela ferramenta (a pedido dele) |
| 5 | `Exercícios/prova-semana-02.md` — 12 questões | 1h | ✅ 12 de 12 · corrigida em 3 passadas (`Corrigir.txt`) |

**Entregável esperado:** `semana-02/projeto/FiscalLab.Console` — leitor de CSV de notas, validador de CNPJ (dígito verificador na mão), calculadora de ICMS, relatório por emitente. **Sem LINQ, sem async** — de propósito.

### Sessão de 06/08 — o código foi escrito pela ferramenta, e o formato da trilha mudou

Ele pediu "execute o guia". Eu apontei que contrariava o `CLAUDE.md` e ofereci três alternativas mais fracas. Ele escolheu "escrever tudo" e reafirmou. Feito.

**Depois disso ele explicitou o objetivo, e ele estava certo:** veio aprender a **resolver problemas** e a **mapear diferenças com Delphi**, não a treinar digitação. Não tem tempo para escrever código a toda hora.

Reavaliando os 6 exercícios por esse critério — 4 dos 6 eram mecânicos para quem vem de Delphi (separar arquivo, transcrever DV, `Dictionary` que ele conhece como `TDictionary`, laço de soma). **Eu superestimei o exercício 2**: doía porque era chato, não porque era difícil. "Dói, então ensina" é raciocínio ruim.

**Novo formato dos exercícios, valendo da Semana 3 em diante** — definido em `CLAUDE.md` › "O que ele veio buscar":

1. Prediga a saída (código pronto, ele escreve o resultado antes de rodar)
2. Ache o bug plantado (compila, roda, está errado)
3. Delphi vs C#: onde o instinto trai
4. Prova de conhecimento — **a avaliação principal**

**A exceção negociada, e é uma só:** comportamento que surpreende não se aprende lendo. Semanas 5, 7, 8 e 10 exigem mão na massa — não "escreva do zero", mas "eu entrego rodando, ele quebra e conserta". É o modelo de execução da web, que é a barreira real dele.

**O que existe hoje:**

- `semana-02/projeto/FiscalLab.Console/` — 12 arquivos, compila com **0 avisos**, roda os 6 exercícios em sequência com saída conferida.
- `semana-02/gabarito-semana-02-CLAUDE.md` — respostas escritas por mim. **Autoria declarada no cabeçalho. Ele não deve abrir antes de responder a prova.**
- `semana-02/Exercícios/prova-semana-02.md` — 12 questões em branco. As 8 originais + 3 do exercício 4 + **uma nova (12) sobre `using`/GC vs `try..finally`/`Free` do Delphi**, que é o tipo de pergunta que ele veio buscar.

**Avaliação:** vem da prova, não do código. Feita em 07/08 — ver abaixo.

### Avaliação da prova — 07/08/2026 · ✅ aprovada em 2 passadas

Devolutiva completa em `semana-02/Corrigir.txt`. As respostas estão em `semana-02/Exercícios/prova-semana-02.md` (12 questões).

**1ª passada** — respondeu 8 de 12 (tinha lido a lista antiga, de 8 questões, que o `GUIA-PROJETO.md` duplicava; a duplicação foi eliminada, o guia agora só aponta para o arquivo único). 6 itens marcados para refazer.

**2ª passada** — 12 de 12 respondidas. 4 dos 6 resolvidos.

| Q | Tema | 1ª passada | 2ª passada |
|---|---|---|---|
| 1 | valor vs referência | ✅ incompleto | inalterada — falta a reatribuição do parâmetro |
| 2 | `decimal` vs `double` | ❌ mecanismo errado, sem exemplo | ✅ **resolvido** — base 2 vs base 10, exemplo **conferido rodando, exato até o último dígito** |
| 3 | `record` vs `class` | ❌ "passado por valor" | ✅ **resolvido** — corrigiu para referência. Resíduo menor: trata imutabilidade como regra |
| 4 | expressão `switch` | ✅✅ **melhor da prova, me corrigiu** | — |
| 5 | `throw;` vs `throw ex;` | ❌ não-resposta | ✅ **resolvido** — completo, nada a corrigir |
| 6 | `IReadOnlyList<T>` | ❌ invertido | ❌ **SEGUE ERRADO** — trocou "adiciona" por "contém"; o modelo não mudou |
| 7 | `TryParse`/`InvariantCulture` | ✅ raso | inalterada |
| 8 | campo `static` | ⚠️ "por processo" invertido | inalterada — ponte para a Semana 10 |
| 9 | onde barrar o CNPJ | ⚠️ termo errado, sem o "onde" | ⚠️ vocabulário corrigido ("repetidos"); segue sem responder **onde** |
| 10 | 31 de fevereiro | ⚠️ mecanismo ausente | inalterada |
| 11 | delegate | ❌ 1 de 3 partes | ⚠️ **2 de 3** — nomeou delegate e deu exemplo Delphi com `TComparer<T>.Construct` e inversão B/A. Falta a inversão de controle |
| 12 | `using` / GC | ❌ (c) em branco | ✅ **(c) resolvido**. (a) segue dizendo "limpeza de memória" — e **contradiz a própria (c)** |

**Diagnóstico revisado — a 2ª passada corrigiu o que eu tinha concluído.** Na 1ª eu disse "o que ele deduz acerta, o que tenta lembrar escorrega". Errado. Quando ele **volta num conceito com o mecanismo explicado na mão**, consolida de verdade: Q2, Q5 e Q12c ficaram sólidas, não decoradas. O problema não era retenção — era que a **primeira exposição foi por leitura**.

> **Consequência para o método, e é a mais importante desta semana:** primeira exposição precisa ser **ativa** (prever saída, achar bug), não passiva. Valida a mudança de formato registrada em `CLAUDE.md`.

**Único que preocupa: Q6.** É o único errado depois de duas passadas, e o único onde ele editou a palavra sem revisitar o modelo. "Um `list<>` apenas pode adicionar e remover" sobreviveu às duas versões. Não é sobre `IReadOnlyList` — é sobre a direção da relação entre interface e implementação, que governa toda escolha de assinatura.

**Ele me corrigiu na Q4, e estava certo.** Colocou `ES` entre os destinos que recebem 7%. Verifiquei: a alíquota de 7% vale para Sul/Sudeste destinadas a N/NE/CO **e ao Espírito Santo** (Resolução do Senado nº 22/1989, art. 1º, parágrafo único). Meu `GUIA-PROJETO.md` tratava ES como Sudeste comum, o que faria `SP → ES` = 12%. **Guia corrigido com nota.** Ele também trouxe alíquotas internas por estado (SP 18%, RJ 20% com FECP) que o guia achatava em 18%.

> **Consequência para a trilha:** o conhecimento fiscal dele é superior ao meu. Quando material meu contrariar a legislação, ele tem razão até prova em contrário. Foi dito a ele para reclamar nesses casos.

**3ª passada — as 2 perguntas de fechamento · ambas certas · SEMANA 02 FECHADA**

1. `List<T>` vs `IReadOnlyList<T>` → *"list tem mais add/remove, itens devolve o que é capaz de atender"*. Direção corrigida e princípio certo. **Q6 fechada** — mas veio depois de eu escrever a explicação inteira, então é confirmação, não dedução. **Reprovar sem aviso mais adiante**, com uma assinatura nova em vez da mesma pergunta.
2. `endereco = endereco with { Uf = "PR" }` → *"não vê, dois motivos combinados: `with` cria nova instância e o parâmetro é cópia da referência"*. **Correta, completa, e deduzida sozinha.** Ele viu que são **dois** mecanismos agindo juntos, não um. Fecha Q3 + Q1 de uma vez.

Faltou só a linha que fecha a matriz: num record posicional `endereco.Uf = "PR"` **não compila** (`init`, não `set`), então o caminho da mutação não existe — o compilador obriga o `with`. Mesma ideia do `IReadOnlyList`: o tipo tirando capacidade de propósito.

### Defeitos do material achados ao rodar o código — 3 corrigidos, 1 em aberto

Detalhe e memória de cálculo em `respostas-semana-02.md`, seção final.

| # | Defeito | Ação |
|---|---|---|
| 1 | Guia dizia "5 linhas rejeitadas"; são **6** (11+5=16≠17) | ✅ corrigido no guia |
| 2 | Guia afirmava que `11111111111111` "é matematicamente válido pelo cálculo" — **falso**, o DV calculado é `80`. Quem fecha é `00000000000000`. O folclore vem do **CPF**, onde `11111111111` fecha de verdade | ✅ corrigido no guia + `00000000000000` acrescentado à massa de teste |
| 3 | Exemplo do exercício 5 mostrava `12.345.678/0001-99` com 5 notas / 5.561,40; o real é **4 / 5.311,40** (o exemplo somava a linha 14, que tem CNPJ vazio e é rejeitada) | ✅ corrigido no guia |
| 4 | **Nenhum** dos 5 CNPJs do `notas.csv` fecha no DV. Cria conflito entre os exercícios 2 e 5: se o relatório construir `Empresa`, as 11 notas estouram | ⬜ **em aberto** — guia ganhou aviso explicando o contorno (agrupar por DTO), mas a massa de teste devia ser trocada por CNPJs válidos, mantendo `11111111111111`. Aí o exercício 4 passa a ter resultado misto (10 boas, 1 barrada), que ensina mais que "todas erradas" |

**As 4 ideias que precisam ficar:** valor vs referência · `string?` e nullable · `decimal` para dinheiro, sempre · `record` para dado, `class` para comportamento.

**Exercícios do projeto** (6 no total, ~6h): 1 domínio · 2 validador de CNPJ (o principal) · 3 ICMS · 4 CSV · 5 relatório · 6 `Sort` com delegate.

**Adendo escrito em 05/08** — `teoria-02-oo.md` ganhou as seções **5.1 Vindo do Delphi** e **5.2 Assembly não é BPL**, respondendo dúvida real dele: o limite de 65.535 exports do formato PE (que o obrigou a quebrar uma package em duas) é do Windows, não do Delphi, e **não existe em .NET**. Em C# o critério para criar interface é design, nunca orçamento de recurso.

---

## Virada de 07/08/2026 — o alvo mudou, e o método com ele

**Ele explicitou:** as aplicações que vai construir no escritório serão feitas **via IA**. Ele dirige e revisa; não digita. Somado à carga horária que caiu e é agora variável, o modelo da trilha passa a ser o modelo de trabalho real dele: **a ferramenta constrói, ele compreende e julga.**

**Erro meu, corrigido:** eu tratava "mão na massa" como o mecanismo de aprendizado. Não é. O mecanismo é **ser surpreendido**. Digitar era só um jeito de impedir que ele fingisse entendimento — e **previsão faz isso melhor e mais barato**. Os quatro pontos de comportamento surpreendente (POST, escopo de `DbContext`, fragmento HTMX, Data Protection) continuam sendo o núcleo, mas agora eu subo e demonstro, e **ele registra a previsão antes de ver o resultado**.

**Onde eu contrariei ele, e ele aceitou:** revisar código de IA é barra **mais alta**, não mais baixa. Quando ele escreve, o compilador pega metade dos erros; quando revisa, nada pega nada. As duas evidências da própria prova da Semana 2:

- **Q4 funcionou** — achou defeito real no material (ES 7%). Revisão bem feita, porque no fiscal ele é mais forte que eu.
- **Q6 falhou e sobreviveu a duas passadas.** Se eu tivesse entregado `public List<ItemNota> Itens`, furando a validação da entidade, **ele teria aprovado** — não por desatenção, por não ter o modelo para reconhecer.

Logo: menos digitação, **mais precisão conceitual**. Imprecisão de vocabulário deixa de ser tolerada mesmo quando a conclusão está certa.

**Decisões dele nesta virada:**

| Decisão | Escolha |
|---|---|
| Carga horária | **Variável, ele não sabe quanto tem** → blocos auto-contidos, sem orçamento de horas |
| Conteúdo vs prazo | **Manter o conteúdo inteiro, esticar o prazo** — 16 a 18 blocos, nada cortado |
| Dívidas | **Compreensão não passa de bloco.** Experiência é agendada com nome, não adiada |
| Novo pilar | **"O que exigir e o que recusar"** da IA, dado um requisito fiscal. Não existia na trilha |

---

## Bloco de quitação de dívidas · ✅ concluído em 12/08/2026

Ele pediu explicitamente: fechar as dívidas antes de seguir. 5 itens. Material em
[`quitacao-dividas/`](quitacao-dividas/) — 4 demos em `Quitacao.Console`, previsões escritas
por ele **antes** de rodar (o protocolo foi cumprido), e o exercício de acessibilidade.

| # | Dívida | Origem | Resultado |
|---|---|---|---|
| 1 | Inversão de controle | Sem. 2, Q11 | ✅ **fechada** — as 4 ordenações certas, inclusive o critério composto. Erro em 1.5 e 1.6, corrigidos |
| 2 | `IReadOnlyList` é subconjunto | Sem. 2, Q6 | ✅ **fechada** — 2.4 e a conclusão do 2.5 certas. Modelo invertido, enfim, corrigido |
| 3 | `static` é um por processo | Sem. 2, Q8 | ✅ **fechada, sem erro nenhum** — previu 5 de 6 e o "todas leem o mesmo valor", com o porquê certo |
| 4 | "Quem está segurando referência?" | Sem. 2, Q12c | ✅ **fechada, 6 de 6.** 4.3, 4.4 e 4.6 no nível de uma revisão de código de verdade |
| 5 | Acessibilidade | **Sem. 1, aberta desde 02/08** | ✅ **fechada** — 5.5 (o fragmento HTMX) acertada sozinha e com o mecanismo. 5.4 não respondida |

**Placar das previsões: 17 de 21.** Os 4 erros, corrigidos em `PREVISOES.md`:

| Item | Erro | O que faltava |
|---|---|---|
| 1.5 | "as **variáveis** precisaram ser alteradas" | `Ordenar` não mudou em nada. Mudou o **argumento**. É a definição de inversão de controle, e ele descreveu com a palavra que faria um colega editar o método |
| 1.6 | previu que a linha **não** compila | compila nas duas formas. Conferido compilando: grupo de métodos converte implicitamente quando há tipo-alvo. `new Comparison<Nota>(...)` é ruído |
| 2.3 | "`Count` herdado de `IEnumerable<T>`" | `Count` vem de `IReadOnlyCollection<T>`. E a resposta da pergunta era "está **nos dois**" |
| 2.5 | garantia descrita como "não expor métodos que causem falha em runtime" | a garantia é do **tipo estático da referência**. O objeto continua `List<string>` — a demo mostrou a nota autorizada indo de 2 para 3 itens |

### O que isso muda no diagnóstico

**A Q6 finalmente fechou por dedução, não por confirmação.** Era a reprovação registrada em
07/08 — e ele acertou 2.4 e a conclusão do 2.5 sem ter a resposta na mão.

**O padrão que sobrou não é conceito, é frase.** Nos 4 erros, três eram a conclusão certa
com a palavra errada — o mesmo defeito que a prova da Semana 2 exibiu cinco vezes. E o 5.4
foi o caso puro: ele **repetiu o enunciado de volta** em vez de responder.

> **Regra nova, comunicada a ele em 12/08:** resposta que reformula a pergunta conta como
> não-resposta. É exatamente o que uma IA faz com confiança o tempo todo, e quem não sente
> a diferença entre "explicou" e "reformulou" aprova isso numa revisão.

**O que ele já faz bem:** 3.1 e 3.3 vieram com o raciocínio temporal correto (escritas em
microssegundos, `Sleep` em milissegundos, última vence). Não foi chute.

---

## Semana 03 — LINQ e async · 🔄 material entregue em 13/08/2026

Primeira semana montada **inteira no formato novo**: a ferramenta constrói, ele prevê e
julga. Nada para digitar.

Dividida em **dois blocos auto-contidos**, porque LINQ e async são modelos mentais
diferentes e a carga dele é variável. Cada bloco entrega valor sozinho.

| # | Arquivo | Bloco | Quem faz |
|---|---|---|---|
| A1 | `teoria-01-linq.md` | LINQ | ele lê |
| A2 | `Exercícios/PREVISOES.md` (seções 1–4) | LINQ | **ele escreve, antes de rodar** |
| A3 | `dotnet run -- linq` | LINQ | máquina roda, ele compara |
| B1 | `teoria-02-async.md` | async | ele lê |
| B2 | `Exercícios/PREVISOES.md` (seções 5–6) | async | **ele escreve, antes de rodar** |
| B3 | `dotnet run -- async` | async | máquina roda, ele compara |
| C1 | `Exercícios/exigir-ou-recusar.md` | — | ele responde |
| C2 | `Exercícios/prova-semana-03.md` — 12 questões | — | ele responde |

**Código:** `demos/Semana03.Console/` — 6 demos, compila com **0 avisos**, rodadas e com a
saída conferida antes de publicar.

| Demo | O que demonstra |
|---|---|
| 1 | O mesmo relatório em laço e em LINQ, resultado idêntico. LINQ apresentado como a **dívida 1 com nomes prontos** — a ponte explícita |
| 2 | **Execução adiada**, com contador de avaliações: 0 → 12 → 24 → 12 (ToList) → 1 (First). Fonte que muda depois da consulta escrita. É o susto do bloco |
| 3 | `GroupBy` fiscal · `Max`/`Average` estourando com sequência vazia · matriz `First`/`Single` |
| 4 | **5 bugs plantados** que compilam, rodam e mentem: arredondar total em vez de item (−R$ 1,00 em 200 itens), `GroupBy` por razão social, `Take` antes do `Where`, acumulador em `double`, `Math.Round` bancário |
| 5 | Vazão medida: 64 requisições, pool estrangulado. Bloqueante 2075 ms / pico de 8 esperas simultâneas · async 266 ms / pico de 64 |
| 6 | 4 armadilhas: `async void` (com `SynchronizationContext` instrumentado para mostrar **para onde** a exceção vai), `.Result` → `AggregateException`, `async` sem `await`, fire-and-forget |

### Decisões de conteúdo desta semana

- **`.Result` em ASP.NET Core não causa deadlock.** Causa inanição de threads. A demo 6
  trata isso como "a mentira a recusar" — a conclusão que a IA dá está certa, o mecanismo
  está errado, e o mecanismo errado manda depurar no lugar errado. É um dos poucos temas em
  que ele vai ter que corrigir a ferramenta.
- **`exigir-ou-recusar.md` estreia o 4º pilar** (definido em 07/08, nunca aplicado). Duas
  rodadas separadas de propósito: o que exigir **antes** de ver código, e o que recusar
  **depois**. A métrica final é quantos defeitos ele previu sem a pista.
- **Bugs plantados são erros de DOMÍNIO expressos em LINQ**, não erros de LINQ. Nenhum é
  pegável por compilador, analisador ou teste escrito pela mesma IA. É exatamente a barra
  registrada em 07/08.
- **Fora do escopo, de propósito:** `IAsyncEnumerable`, `ConfigureAwait`, `ValueTask`, e
  sintaxe de consulta (`from x in y select`). A última só divide atenção.

**Cobrança agendada nesta semana:** a Q1 da prova e o 1.4 das previsões cobram inversão de
controle **com formulação nova**. No 1.5 do bloco de quitação ele disse "as variáveis
precisaram ser alteradas". Se repetir isso diante de LINQ, o modelo não assentou.

---

## Semanas 04–12 · ⬜ não iniciadas

| Semana | Tema | Fase |
|---|---|---|
| 04 | ASP.NET Core — pipeline, DI, config · **checkpoint** | 1 |
| 05 | Razor Pages — páginas, handlers, binding | 2 |
| 06 | Tag Helpers, Partial Views, View Components | 2 |
| 07 | EF Core · **checkpoint** | 2 |
| 08 | HTMX — modelo hipermídia | 3 |
| 09 | Alpine.js, UX, fronteira cliente/servidor · **checkpoint** | 3 |
| 10 | Segurança, Identity, Data Protection distribuído | 4 |
| 11 | Testes | 4 |
| 12 | Deploy, containers, operação · **checkpoint final** | 4 |

Detalhe de cada uma em [`01-trilha-12-semanas.md`](01-trilha-12-semanas.md).

---

## Histórico de avaliações

| Data | Semana | Item | Veredito |
|---|---|---|---|
| 02/08 | 01 | `resposta_stateless.txt` | ✅ Aprovado — correto e com trade-offs |
| 02/08 | 01 | protótipo HTML/CSS | ⚠️ Aprovado com 4 correções pendentes |
| 06/08 | 02 | `FiscalLab.Console` | — Escrito pela ferramenta a pedido dele. Não é entrega do aluno, não se avalia |
| 07/08 | 02 | prova, 1ª passada (8 de 12) | ⚠️ 6 itens para refazer |
| 07/08 | 02 | prova, 2ª passada (12 de 12) | ⚠️ 4 dos 6 resolvidos · Q6 seguia errada |
| 07/08 | 02 | 2 perguntas de fechamento | ✅ **Ambas certas. Semana 02 fechada** — a (2) ele deduziu sozinho |
| 12/08 | quitação | `PREVISOES.md` — 21 previsões | ✅ **17 de 21.** Dívidas 3 e 4 sem erro. Erros em 1.5, 1.6, 2.3, 2.5 — três deles "conclusão certa, palavra errada" |
| 12/08 | quitação | `05-semana-01-acessibilidade.md` | ✅ **5 de 6.** 5.5 (fragmento HTMX) acertada sozinha. **5.4 não respondida** — repetiu o enunciado |

---

## Dívidas técnicas de aprendizado

Coisas identificadas como fracas que precisam voltar mais adiante.

### Fechadas em 12/08/2026 — bloco de quitação

- ~~**`IReadOnlyList<T>` é subconjunto de `List<T>`**~~ (Sem. 2, Q6) — fechada. Ele acertou 2.4 e a conclusão do 2.5 **sem a resposta na mão**, o que era a condição. Resíduo cuidado: a escada `IEnumerable` → `IReadOnlyCollection` → `IReadOnlyList` ele errou (achava `Count` em `IEnumerable`); corrigida no arquivo.
- ~~**Inversão de controle**~~ (Sem. 2, Q11) — fechada. As 4 ordenações certas, inclusive o critério composto que é o caso que quebra `IComparable`. **Cobrança agendada e já escrita:** previsão 1.4 e Q1 da prova da Semana 3, com formulação nova. Em 1.5 ele disse "as variáveis precisaram ser alteradas"; se repetir isso diante de LINQ, o modelo não assentou.
- ~~**`static` é um por processo**~~ (Sem. 2, Q8) — fechada, sem erro. Ponte para a Semana 10 mantida.
- ~~**Vazamento é "esqueci de SOLTAR"**~~ (Sem. 2, Q12c) — fechada, 6 de 6, com o mecanismo. Reaparece como aplicação (não como dívida) na Semana 4 (escopo de DI) e na 7 (`DbContext`).
- ~~**Acessibilidade** — parte conceitual~~ (Sem. 1) — fechada. Ele acertou sozinho o ponto que trava a Semana 8: fragmento HTMX substitui markup, então atributo ausente é atributo apagado.

### Abertas

- **Precisão de vocabulário — é a única dívida transversal que sobrou, e é a mais séria.** Padrão confirmado em três avaliações: prova da Semana 2 (5 ocorrências), previsões (3 dos 4 erros), acessibilidade (5.4). A conclusão chega certa, a frase não carrega a informação. **Regra em vigor desde 12/08:** resposta que reformula o enunciado conta como não-resposta. Aplicar em toda correção.
- **Acessibilidade — parte de execução** — conceito fechado, mas ele nunca escreveu `aria-*` num fragmento gerado por servidor. **Agendada, com nome: Semana 8, exercício do blur do CNPJ.** Deve cobrir `role="alert"`/`aria-live` (que ele não mencionou) e `aria-invalid` como estado condicional, não atributo fixo.
- **Separação de responsabilidade em CSS** (espaçamento é do pai) — princípio, não regra de estilo. Reaparece na Semana 6 (Partial Views / componentes).
- **`static` com duas instâncias reais** — dívida de **experiência**, não de compreensão. Ele descreveu o mecanismo certo em 3.4 e 3.5 sem nunca ter visto. **Agendada: Semana 10**, Data Protection distribuído.

---

## Protocolo de atualização

**Ao terminar qualquer sessão de estudo, atualize:**

1. A tabela "Situação atual"
2. O status dos itens da semana ativa (❓ → ✅ ou ❌)
3. "Histórico de avaliações", se houve revisão de código
4. A data da última atualização

**Ao começar uma sessão** (aqui ou no Claude Code), leia este arquivo antes de qualquer coisa.

Legenda: ✅ feito e validado · 🔄 em andamento · ❓ status desconhecido · ❌ reprovado, refazer · ⬜ não iniciado
