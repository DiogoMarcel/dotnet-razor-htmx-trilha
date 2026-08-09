# PROGRESSO — fonte única de verdade

> **Este arquivo é o estado do estudo.** Cowork e Claude Code leem daqui antes de responder qualquer coisa, e escrevem aqui ao fim de cada sessão. Se este arquivo estiver desatualizado, as duas ferramentas dão conselho errado.

**Aluno:** Diogo Marcel · **Início:** 26/07/2026 · **Ritmo alvo:** 10–15h/semana
**Última atualização:** 07/08/2026

---

## Situação atual

| | |
|---|---|
| **Bloco ativo** | **Quitação de dívidas** — 5 itens, antes da Semana 03. A montar |
| **Fase** | 1 — Fundação |
| **Bloqueio** | Nenhum reportado |
| **Pendência aberta** | As 5 dívidas abaixo. **Regra dele: dívida de compreensão não passa de bloco** |
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

1. ❓ Estilo inline em `detalhe.html` linha 64 → resolver espaçamento no container pai, não no elemento
2. ❓ Erro de CNPJ não é anunciado a leitor de tela → falta `aria-invalid="true"` e `aria-describedby` com lista. **Reaparece na Semana 8** — o fragmento devolvido por HTMX no blur precisa exatamente destes atributos.
3. ❓ `.dados dd { text-align: right }` → alinhar à esquerda; direita só para coluna numérica
4. ❓ Exercício 4 (grid CEP 3 + Logradouro 6 + Número 3 = 12) já estava correto — **pergunta em aberto:** você conferiu e viu que somava, ou passou batido?

> **Ação:** confirmar se os 4 foram aplicados. O item 2 é o único que trava a Semana 8.

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

## Bloco de quitação de dívidas · 🔄 ativo, antes da Semana 03

Ele pediu explicitamente: fechar as dívidas antes de seguir. 5 itens.

| # | Dívida | Origem | Como fecha |
|---|---|---|---|
| 1 | Inversão de controle | Sem. 2, Q11 | previsão de saída |
| 2 | `IReadOnlyList` — reprovação | Sem. 2, Q6 | assinatura nova, ele escolhe e justifica |
| 3 | "Quem está segurando referência?" | Sem. 2, Q12c | demonstração + previsão |
| 4 | `static` é um por processo | Sem. 2, Q8 | vazamento entre requisições, em número |
| 5 | **4 correções de acessibilidade** | **Sem. 1, abertas desde 02/08** | ele confirma se aplicou; se não, antes/depois |

O item 5 é o mais antigo e o que mais preocupa: `aria-invalid`/`aria-describedby` voltam na Semana 8, no fragmento que o HTMX devolve no blur do CNPJ. Se não fechar, a Semana 8 tropeça.

---

## Semanas 03–12 · ⬜ não iniciadas

| Semana | Tema | Fase |
|---|---|---|
| 03 | C# parte 2 — LINQ e async | 1 |
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

---

## Dívidas técnicas de aprendizado

Coisas identificadas como fracas que precisam voltar mais adiante:

- **Acessibilidade** (`aria-*`, foco, navegação por teclado) — apareceu na Semana 1, reaparece nas Semanas 8 e 9. Não deixar acumular.
- **Separação de responsabilidade em CSS** (espaçamento é do pai) — princípio, não regra de estilo.
- **`IReadOnlyList<T>` é subconjunto de `List<T>`** (Semana 2, Q6) — **resolvido na 3ª passada, mas com a resposta na mão.** Foi o único que sobreviveu a duas passadas antes de eu explicar por extenso. É confirmação, não dedução. **Reprovar sem aviso**, com uma assinatura nova: *"este método devolve as notas rejeitadas para a tela montar a tabela — qual tipo de retorno, e por quê?"*. Se sair `IReadOnlyList` com justificativa, assentou.
- **Inversão de controle** (Semana 2, Q11) — ele já nomeou `delegate` e mapeou para `reference to function`, mas não explicou **por que** um método aceita código como parâmetro. É o que falta para o LINQ não virar decoreba. Cobrar no **primeiro exercício da Semana 3**.
- **`static` é um por processo, não global entre processos** (Semana 2, Q8) — uma palavra, mas é a **ponte para a Semana 10**: cache static diverge entre instâncias, e é a mesma família do Data Protection distribuído.
- **Vazamento em .NET é "esqueci de SOLTAR", não "esqueci de LIBERAR"** (Semana 2, Q12c) — ele acertou que o GC troca a pergunta, mas só pegou a pergunta dos recursos do SO. Falta a segunda: *quem ainda está segurando referência?* Não há linha errada, há referência que sobra — precisa de profiler. **Reaparece na Semana 4** (escopo de DI) e na **Semana 7** (`DbContext`).

---

## Protocolo de atualização

**Ao terminar qualquer sessão de estudo, atualize:**

1. A tabela "Situação atual"
2. O status dos itens da semana ativa (❓ → ✅ ou ❌)
3. "Histórico de avaliações", se houve revisão de código
4. A data da última atualização

**Ao começar uma sessão** (aqui ou no Claude Code), leia este arquivo antes de qualquer coisa.

Legenda: ✅ feito e validado · 🔄 em andamento · ❓ status desconhecido · ❌ reprovado, refazer · ⬜ não iniciado
