# PROGRESSO — fonte única de verdade

> **Este arquivo é o estado do estudo.** Cowork e Claude Code leem daqui antes de responder qualquer coisa, e escrevem aqui ao fim de cada sessão. Se este arquivo estiver desatualizado, as duas ferramentas dão conselho errado.

**Aluno:** Diogo Marcel · **Início:** 26/07/2026 · **Ritmo alvo:** 10–15h/semana
**Última atualização:** 04/08/2026

---

## Situação atual

| | |
|---|---|
| **Semana ativa** | 02 — C# fundamentos (parte 1) |
| **Fase** | 1 — Fundação (semanas 1–4) |
| **Bloqueio** | Nenhum reportado |
| **Pendência aberta** | Correções da Semana 1 (`semana-01/Corrigir.txt`) — **não confirmadas como aplicadas** |
| **Próximo checkpoint** | Sabatina da Fase 1, ao fim da Semana 4 |

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

## Semana 02 — C# fundamentos (parte 1) · 🔄 em andamento

**Material disponível**

| # | Arquivo | Tempo | Status |
|---|---|---|---|
| 0 | Instalar SDK .NET 10 (`winget install Microsoft.DotNet.SDK.10`) | 15min | ❓ |
| 1 | `teoria-01-tipos.md` | 2h | ❓ |
| 2 | `teoria-02-oo.md` | 2h30 | ❓ |
| 3 | `teoria-03-colecoes.md` | 2h | ❓ |
| 4 | `projeto/GUIA-PROJETO.md` — FiscalLab console | 5h | ❓ |
| 5 | Prova de conhecimento | 1h | ❓ |

**Entregável esperado:** `semana-02/projeto/FiscalLab.Console` — leitor de CSV de notas, validador de CNPJ (dígito verificador na mão), calculadora de ICMS, relatório por emitente. **Sem LINQ, sem async** — de propósito.

**As 4 ideias que precisam ficar:** valor vs referência · `string?` e nullable · `decimal` para dinheiro, sempre · `record` para dado, `class` para comportamento.

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

---

## Dívidas técnicas de aprendizado

Coisas identificadas como fracas que precisam voltar mais adiante:

- **Acessibilidade** (`aria-*`, foco, navegação por teclado) — apareceu na Semana 1, reaparece nas Semanas 8 e 9. Não deixar acumular.
- **Separação de responsabilidade em CSS** (espaçamento é do pai) — princípio, não regra de estilo.

---

## Protocolo de atualização

**Ao terminar qualquer sessão de estudo, atualize:**

1. A tabela "Situação atual"
2. O status dos itens da semana ativa (❓ → ✅ ou ❌)
3. "Histórico de avaliações", se houve revisão de código
4. A data da última atualização

**Ao começar uma sessão** (aqui ou no Claude Code), leia este arquivo antes de qualquer coisa.

Legenda: ✅ feito e validado · 🔄 em andamento · ❓ status desconhecido · ❌ reprovado, refazer · ⬜ não iniciado
