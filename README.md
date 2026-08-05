# De Delphi para .NET — trilha de 12 semanas

Diário público de estudo de **Diogo Marcel**, desenvolvedor Delphi migrando para desenvolvimento web com **ASP.NET Core 10 · Razor Pages · HTMX**.

Não é um curso. É o registro honesto de alguém aprendendo em público: a teoria que escrevi para mim mesmo, o código que produzi, e as **correções que recebi** — inclusive as que doeram.

---

## Por que esta stack

Sistema fiscal/contábil legado em Delphi sendo modernizado. Razor Pages + HTMX foi a escolha: renderização no servidor, sem SPA, sem circuito permanente. Menos peças móveis para uma equipe que nunca fez web.

**A barreira real não é a sintaxe do C#.** Delphi e C# têm o mesmo projetista (Anders Hejlsberg) — `class`, `interface`, propriedades, genéricos transferem quase de graça. A barreira é o **modelo de execução da web**: em Delphi um `TForm` fica vivo com seu estado; na web o servidor esquece tudo entre uma requisição e outra. Praticamente toda dificuldade minha até aqui desce até esse ponto.

| Delphi | Web / .NET | Diferença crítica |
|---|---|---|
| `TForm` | Página Razor (`.cshtml` + `PageModel`) | Não fica vivo. É recriado a cada requisição. |
| Campo do form | *nada equivalente* | Estado morre no fim da requisição. Persistir = banco/cookie/hidden field |
| `OnClick` | Handler HTTP (`OnPostSalvar`) | Não é chamada direta — é requisição que atravessa a rede |
| `TQuery` | EF Core `DbSet` / Dapper | Sem cursor aberto. Busca, materializa, fecha |
| `.dfm` | `.cshtml` | Fluxo de documento e CSS, não coordenadas absolutas |
| `.exe` de janela | `dotnet run` → servidor HTTP | Sua aplicação é um servidor |

Mapa completo em [`00-LEIA-PRIMEIRO.md`](00-LEIA-PRIMEIRO.md).

---

## Progresso

**Semana ativa: 02 — C# fundamentos.** Estado detalhado e sempre atualizado em [`PROGRESSO.md`](PROGRESSO.md).

| Fase | Semanas | Tema | Status |
|---|---|---|---|
| 1 — Fundação | 01–04 | Web, HTTP, HTML/CSS, C#, ASP.NET Core | 🔄 em andamento |
| 2 — Razor | 05–07 | Razor Pages, Tag Helpers, EF Core | ⬜ |
| 3 — Hipermídia | 08–09 | HTMX, Alpine.js, fronteira cliente/servidor | ⬜ |
| 4 — Produção | 10–12 | Segurança, testes, containers | ⬜ |

Currículo completo: [`01-trilha-12-semanas.md`](01-trilha-12-semanas.md)

---

## O projeto: FiscalLab

Aplicação construída em incrementos, uma feature por semana — nunca teoria sem código na mesma semana. Cadastro de empresas, entrada de notas fiscais, cálculo de ICMS, relatórios.

Começa como app de console (semana 2) e vira aplicação web completa. Visão de ponta a ponta em [`02-projeto-fiscallab.md`](02-projeto-fiscallab.md).

Decisões fixas: `decimal` para dinheiro, sempre — contexto fiscal não perdoa ponto flutuante. Nullable reference types ligado. Um tipo público por arquivo.

---

## Como navegar

```
semana-NN/
├── README.md            <- ordem de estudo, tempo estimado, o que NÃO entra
├── teoria-NN-*.md       <- 3 blocos de ~2h
├── projeto/             <- código, com GUIA-*.md do que construir
├── Exercícios/          <- minhas respostas
└── Corrigir.txt         <- devolutiva da revisão, item por item
```

Os `Corrigir.txt` são a parte mais útil do repo. Erro documentado com o porquê vale mais que acerto sem explicação.

---

## Dívidas de aprendizado em aberto

Coisas que identifiquei como fracas e ainda não resolvi:

- **Acessibilidade** — `aria-invalid` e `aria-describedby` faltando em validação de formulário. Apontado na Semana 1, volta a morder na Semana 8 (fragmentos HTMX de validação precisam exatamente desses atributos). Não é detalhe em sistema usado 8h/dia por operador fiscal.
- **Espaçamento é responsabilidade do container pai**, não do elemento filho. Princípio, não regra pontual.

---

## Ferramentas

Material e código produzidos com apoio de **Claude** (Claude Code e Cowork), configurado via [`CLAUDE.md`](CLAUDE.md) para atuar como revisor crítico — não como validador. A instrução mais importante ali: *nunca escrever o exercício por mim*.

**Stack:** .NET 10 (LTS até nov/2028) · ASP.NET Core Razor Pages · HTMX · Alpine.js · EF Core · VS Code
