# Projeto Prático — FiscalLab

Um mini-sistema fiscal construído incrementalmente, uma feature por semana, espelhando a trilha. Ao final você terá um sistema real no portfólio e — mais importante — terá cometido em casa os erros que você **não** quer cometer em produção.

## Por que este escopo

Escolhi um domínio deliberadamente próximo do sistema legado que motivou esta trilha (cadastro fiscal, emissão de documento, cálculo tributário, painel de obrigações). Assim cada aprendizado é transferível direto. Mas é uma **simulação** — nada de integração real com SEFAZ até você dominar o resto.

## Stack

Exatamente a stack alvo:

- .NET 10 (C#)
- Razor Pages
- HTMX
- Alpine.js
- Tag Helpers + Partial Views + View Components
- EF Core + PostgreSQL (em Docker) — troque por SQL Server se for esse o banco de destino
- Bootstrap 5
- xUnit + Testcontainers + Playwright
- Docker / docker-compose

## IDE

Comece com **VS Code + C# Dev Kit** (leve, gratuito) ou **Rider** (licença não comercial gratuita para estudo). Visual Studio 2026 Community serve para estudo individual — só não vale para uso comercial em equipe — aí a licença exige Professional ou Enterprise.

---

## Onde o código mora

O projeto **não** vive numa pasta única. Ele nasce dentro da semana em que é criado e migra quando muda de natureza:

| Semanas | Local | O que é |
|---|---|---|
| 01 | `semana-01/prototipo/` | HTML + CSS estático, sem .NET |
| 02–03 | `semana-02/projeto/FiscalLab.Console/` | Console app, regra de negócio pura |
| 04–12 | `FiscalLab/` (raiz) | A solução web de verdade, versionada em git |

Na Semana 4 você promove o domínio do console para uma class library e o console vira histórico. O guia detalhado de cada etapa está no `GUIA-*.md` da respectiva semana — este arquivo é a visão de ponta a ponta.

## Setup da solução web (Semana 4)

```bash
# conferir SDK
dotnet --version          # deve mostrar 10.x

# criar solução na raiz de D:\StudieWithAI
dotnet new sln -n FiscalLab
dotnet new razor -n FiscalLab.Web
dotnet new classlib -n FiscalLab.Domain
dotnet sln add FiscalLab.Web FiscalLab.Domain

# rodar com hot reload
cd FiscalLab.Web
dotnet watch
```

Banco em Docker:

```bash
docker run -d --name fiscallab-db \
  -e POSTGRES_PASSWORD=dev123 \
  -e POSTGRES_DB=fiscallab \
  -p 5432:5432 postgres:17
```

Inicialize git no dia 1. Um commit por sessão de estudo. Você vai querer ver a evolução.

---

## Roteiro por semana

### Semana 1 — Protótipo estático

Sem .NET ainda. HTML + CSS puro:

- Tela de listagem de empresas (tabela)
- Formulário de cadastro de empresa
- Layout com menu lateral usando Flexbox ou Grid

**Objetivo escondido:** você vai reconstruir isso em Razor na semana 5, e vai perceber sozinho o que Razor te dá.

### Semana 2 — Regras de negócio em console

Console app, sem web:

- `Empresa`, `NotaFiscal`, `ItemNota` como classes/records
- Validador de CNPJ (dígito verificador na mão)
- Calculadora de ICMS simplificada
- Carregar dados de um CSV

### Semana 3 — Refatorar com LINQ e async

- Substituir loops por LINQ
- Método async que consulta a [BrasilAPI](https://brasilapi.com.br/docs) por CNPJ (`https://brasilapi.com.br/api/cnpj/v1/{cnpj}`) — API pública gratuita, sem chave
- Testar consultas paralelas

### Semana 4 — Esqueleto web

- Criar o projeto Razor Pages
- Mover as classes de domínio para uma class library `FiscalLab.Domain`
- Registrar um `ICnpjService` no DI e injetá-lo numa página
- Middleware de log de requisições
- `appsettings` por ambiente + User Secrets para a connection string

### Semana 5 — CRUD de Empresas (sem JS)

- Listar / criar / editar / excluir empresas
- Validação com DataAnnotations + validação customizada de CNPJ
- Padrão PRG com mensagem de sucesso
- Dados ainda em memória (lista estática) — de propósito, para você sentir o problema

### Semana 6 — Componentização

- Tag Helper `<cnpj-input asp-for="Empresa.Cnpj" />`
- Partial View `_BlocoEndereco`
- View Component `ObrigacoesPendentes` (busca dados próprios, aparece no dashboard)
- Layout unificado com menu e breadcrumb

### Semana 7 — Persistência

- `FiscalLabDbContext` + EF Core + PostgreSQL
- Migrations
- Migrar o CRUD para o banco
- Seed de dados
- Página de relatório: empresas com contagem/soma de notas por período — **sem N+1** (prove com MiniProfiler)

### Semana 8 — HTMX

Aqui o projeto ganha vida:

- Busca de CNPJ: usuário digita, `hx-trigger="keyup changed delay:500ms"` → servidor consulta BrasilAPI → preenche razão social e endereço
- Tabela de notas com filtro e paginação via HTMX (só o `<tbody>` é trocado)
- Validação inline: `hx-post` no blur do campo, devolve a mensagem de erro
- Exclusão com confirmação via `hx-delete` + `hx-confirm`
- Antiforgery token global no `<body>` via `hx-headers`
- `hx-indicator` em todas as ações

**Padrão importante que você vai aprender aqui:** detectar `HX-Request` no `PageModel` e devolver `Partial(...)` em vez da página inteira.

### Semana 9 — Emissão de nota (a tela complexa)

- Formulário de NF-e com itens dinâmicos (adicionar/remover linha via HTMX)
- Máscaras de CNPJ, CEP e moeda com Alpine
- Recálculo de ICMS/IPI/total ao alterar quantidade ou valor (HTMX → servidor → OOB swap do bloco de totais)
- Modal de seleção de produto
- Toast de sucesso via `HX-Trigger` header
- **Autosave de rascunho** — "stateless no servidor" não devolve de graça o formulário longo pela metade; autosave é implementação, não propriedade da arquitetura

### Semana 10 — Segurança

- ASP.NET Core Identity com login e papéis (Admin / Operador / Consulta)
- Autorização por pasta de páginas
- Data Protection em Redis
- Assinar um XML de exemplo com certificado A1 autoassinado (gerado localmente)
- Headers de segurança + CSP

### Semana 11 — Testes

- Unitários: cálculo tributário, validação de CNPJ
- Integração: POST de emissão de nota com `WebApplicationFactory` + Testcontainers
- Teste que verifica se a resposta HTMX devolveu o fragmento correto
- E2E Playwright: login → cadastrar empresa → emitir nota
- Pipeline GitHub Actions

### Semana 12 — Produção

- Dockerfile multi-stage
- docker-compose: app (3 réplicas) + postgres + redis + nginx
- Health checks e graceful shutdown
- Serilog estruturado + OpenTelemetry
- **Teste de resiliência:** com um usuário logado preenchendo uma nota, `docker kill` num container. Nada pode se perder. Se algo se perder, você ainda não terminou.

---

## Como usar minha ajuda

A cada semana, me traga:

1. **O código** — cole o arquivo ou me aponte na pasta. Não me traga descrição, me traga código.
2. **A dúvida específica** — "não entendi por que X" vale mais que "me explica Razor Pages".
3. **O que você tentou** antes de perguntar.

O que eu vou fazer:

- Revisar criticamente, apontando o que está errado **e** o que está apenas medíocre
- Te fazer perguntas em vez de dar respostas quando isso ensinar mais
- Não deixar passar código que funciona por acidente

O que eu **não** vou fazer:

- Escrever o projeto por você. Você não aprende assim.
- Elogiar código ruim.

---

## Marcos de avaliação

Ao fim de cada fase, me peça uma **sabatina**: eu te faço 10 perguntas do módulo, sem consulta. Se você errar mais de 3, refaça a semana mais fraca antes de seguir.

| Fase | Semana | Tema da sabatina |
|---|---|---|
| 1 | 4 | HTTP, stateless, C#, DI, async |
| 2 | 7 | Razor Pages, componentização, EF Core |
| 3 | 9 | HTMX, hipermídia, fronteira cliente/servidor |
| 4 | 12 | Segurança, testes, operação em cluster |
