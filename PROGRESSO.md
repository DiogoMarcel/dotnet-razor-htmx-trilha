# PROGRESSO — fonte única de verdade

> **Este arquivo é o estado do estudo.** Cowork e Claude Code leem daqui antes de responder qualquer coisa, e escrevem aqui ao fim de cada sessão. Se este arquivo estiver desatualizado, as duas ferramentas dão conselho errado.

**Aluno:** Diogo Marcel · **Início:** 26/07/2026 · **Ritmo alvo:** 10–15h/semana
**Última atualização:** 06/08/2026

---

## Situação atual

| | |
|---|---|
| **Semana ativa** | 02 — C# fundamentos (parte 1) · código entregue pela ferramenta · **prova pendente com ele** |
| **Fase** | 1 — Fundação (semanas 1–4) |
| **Bloqueio** | Nenhum reportado |
| **Pendência aberta** | Prova da Semana 2 (`semana-02/Exercícios/prova-semana-02.md`) — 12 questões, ele vai responder · Correções da Semana 1 seguem **não confirmadas** |
| **Próximo checkpoint** | Sabatina da Fase 1, ao fim da Semana 4 |
| **Formato dos exercícios** | **Mudou em 06/08.** Ver `CLAUDE.md` › "O que ele veio buscar" |

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

## Semana 02 — C# fundamentos (parte 1) · 🔄 código entregue, prova pendente

**Material disponível**

| # | Arquivo | Tempo | Status |
|---|---|---|---|
| 0 | Instalar SDK .NET 10 (`winget install Microsoft.DotNet.SDK.10`) | 15min | ✅ |
| 1 | `teoria-01-tipos.md` | 2h | ✅ |
| 2 | `teoria-02-oo.md` | 2h30 | ✅ |
| 3 | `teoria-03-colecoes.md` | 2h | ✅ |
| 4 | `projeto/GUIA-PROJETO.md` — FiscalLab console | 5h | ✅ entregue pela ferramenta (a pedido dele) |
| 5 | `Exercícios/prova-semana-02.md` — 12 questões | 1h | 🔄 **com ele** |

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

**Avaliação:** vem da prova, não do código. Corrigir quando ele entregar.

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
| — | 02 | `Exercícios/prova-semana-02.md` | 🔄 Aguardando resposta dele. **É a avaliação da semana** |

---

## Dívidas técnicas de aprendizado

Coisas identificadas como fracas que precisam voltar mais adiante:

- **Acessibilidade** (`aria-*`, foco, navegação por teclado) — apareceu na Semana 1, reaparece nas Semanas 8 e 9. Não deixar acumular.
- **Separação de responsabilidade em CSS** (espaçamento é do pai) — princípio, não regra de estilo.
- **C# fundamentos (Semana 2) — a verificar pela prova.** Valor vs referência, `string?`, `decimal`, `record` vs `class`, delegate. O código foi escrito pela ferramenta; a evidência de aprendizado vem de `Exercícios/prova-semana-02.md`. Corrigir com rigor: questão 1 (valor vs referência) e 8 (`static` na web) são as que preveem se a Fase 2 vai doer.

---

## Protocolo de atualização

**Ao terminar qualquer sessão de estudo, atualize:**

1. A tabela "Situação atual"
2. O status dos itens da semana ativa (❓ → ✅ ou ❌)
3. "Histórico de avaliações", se houve revisão de código
4. A data da última atualização

**Ao começar uma sessão** (aqui ou no Claude Code), leia este arquivo antes de qualquer coisa.

Legenda: ✅ feito e validado · 🔄 em andamento · ❓ status desconhecido · ❌ reprovado, refazer · ⬜ não iniciado
