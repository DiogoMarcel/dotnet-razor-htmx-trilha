# Trilha de 12 Semanas — .NET 10 / Razor Pages / HTMX

**Carga:** ~12h/semana · **Total:** ~145h

Cada semana tem: **Objetivo** (por que importa) · **Conteúdo** · **Recursos** · **Prova de conhecimento** (como saber que você aprendeu de verdade).

> **Prova de conhecimento** não é "assisti o vídeo". É uma tarefa que você faz sem consultar. Se não conseguir, a semana não acabou.

---

# FASE 1 — FUNDAÇÃO (semanas 1–4)

## Semana 1 — Como a web funciona (o módulo que quase todo mundo pula)

**Objetivo:** Você vem de desktop. Sem este módulo, todo o resto vira decoreba. HTMX em particular é *incompreensível* sem entender HTTP.

**Conteúdo**

- Cliente/servidor, DNS, TCP, TLS
- HTTP: métodos (GET/POST/PUT/DELETE), status codes, headers, corpo
- Por que HTTP é **stateless** e o que isso implica
- Cookies e sessões — como se simula estado sobre um protocolo sem estado
- HTML semântico: `form`, `input`, `label`, `table`, `div`/`span`, atributos
- CSS: box model, flexbox, grid, seletores, especificidade
- DOM: o que é, como o navegador o constrói, como JS o altera
- DevTools do navegador: aba Network e Elements (você vai viver aqui)

**Recursos**

- [MDN — Primeiros passos na web](https://developer.mozilla.org/pt-BR/docs/Learn/Getting_started_with_the_web) (PT-BR, gratuito) — leitura base
- [MDN — Visão geral do HTTP](https://developer.mozilla.org/pt-BR/docs/Web/HTTP/Overview) (PT-BR)
- [MDN — Aprender CSS: layout](https://developer.mozilla.org/pt-BR/docs/Learn/CSS/CSS_layout) — foco em Flexbox e Grid
- [Flexbox Froggy](https://flexboxfroggy.com/#pt-br) e [Grid Garden](https://cssgridgarden.com/#pt-br) — jogos, 1h cada, valem muito
- Opcional: curso "Desenvolvimento Web" da [Curso em Vídeo](https://www.cursoemvideo.com/) (Gustavo Guanabara, gratuito, PT-BR) — só os módulos de HTML/CSS

**Prova de conhecimento**

1. Explique, sem consultar, por que um servidor web não sabe que duas requisições vieram do mesmo usuário — e três formas de resolver isso.
2. Monte uma página HTML estática com um formulário de cadastro de empresa (razão social, CNPJ, UF, e-mail), estilizada com Flexbox, sem framework CSS. Abra o DevTools e mostre a requisição que o `submit` gera.

---

## Semana 2 — C# fundamentos (parte 1)

**Objetivo:** Sair do zero em C#. Vindo de Delphi você tem vantagem real aqui — OO, tipagem estática e Pascal/Delphi e C# têm parentesco (Anders Hejlsberg projetou os dois).

**Conteúdo**

- Tipos, `var`, tipos por valor vs referência, `nullable reference types` (`string?`)
- Controle de fluxo, `switch` expressions, pattern matching
- Classes, structs, `record`, interfaces, herança, `abstract`, `sealed`
- Propriedades, `init`, construtores primários
- Exceções e `try/catch/finally`
- Namespaces, `using`, organização de projeto
- Coleções: `List<T>`, `Dictionary<K,V>`, `IEnumerable<T>`

**Recursos**

- [Microsoft Learn — C# for beginners](https://learn.microsoft.com/en-us/training/paths/get-started-c-sharp-part-1/) (gratuito, com sandbox no navegador)
- [Balta.io — Fundamentos do C#](https://balta.io/cursos/fundamentos-csharp) (PT-BR, gratuito no plano free)
- Referência: [C# language reference](https://learn.microsoft.com/en-us/dotnet/csharp/)

**Prova de conhecimento**

Console app: leitor de arquivo CSV de notas fiscais (número, CNPJ emitente, valor, data) que carrega em memória, valida CNPJ (dígito verificador — implemente na mão) e imprime totais por emitente. Sem LINQ ainda.

---

## Semana 3 — C# fundamentos (parte 2): LINQ e async

**Objetivo:** LINQ e `async/await` são onipresentes em .NET. Sem eles você escreve C# com sotaque de Delphi e vai apanhar em EF Core.

**Conteúdo**

- LINQ: `Where`, `Select`, `OrderBy`, `GroupBy`, `Any`, `All`, `First/Single/…OrDefault`, `Sum/Count/Aggregate`
- Execução adiada (lazy) — pegadinha clássica
- Delegates, `Func<>`, `Action<>`, lambdas
- `async`/`await`, `Task`, `Task<T>`
- Por que async importa em servidor web (throughput, não velocidade)
- Armadilhas: `async void`, `.Result` e `.Wait()` (deadlock)
- `IDisposable`, `using`, `IAsyncDisposable`

**Recursos**

- [Microsoft Learn — Write your first C# LINQ queries](https://learn.microsoft.com/en-us/training/modules/csharp-linq/)
- [Microsoft Learn — Asynchronous programming with async and await](https://learn.microsoft.com/en-us/dotnet/csharp/asynchronous-programming/)
- Artigo: [Async/await best practices (Stephen Cleary)](https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/march/async-await-best-practices-in-asynchronous-programming) — antigo mas ainda o melhor

**Prova de conhecimento**

1. Reescreva o programa da semana 2 usando LINQ, e explique em uma frase por que `.Where()` não executa até você iterar.
2. Escreva um método que consulta 3 URLs em paralelo e retorna o resultado das três. Explique a diferença entre fazer isso e chamar `await` sequencialmente três vezes.

---

## Semana 4 — ASP.NET Core: o que acontece quando chega uma requisição

**Objetivo:** Entender o pipeline. É o `Program.cs` que assusta todo iniciante — e é onde tudo se conecta.

**Conteúdo**

- Instalar .NET 10 SDK; `dotnet new`, `dotnet run`, `dotnet watch`
- Estrutura de um projeto ASP.NET Core; o `.csproj`
- Anatomia do `Program.cs`: `WebApplicationBuilder` → `WebApplication`
- **Middleware pipeline** — ordem importa, e por quê
- **Injeção de dependência** — `AddSingleton` / `AddScoped` / `AddTransient` e quando cada um explode na sua mão
- Configuração: `appsettings.json`, ambientes, User Secrets, variáveis de ambiente
- Logging (`ILogger<T>`)
- Minimal APIs (para você entender o contraste com Razor Pages)

**Recursos**

- [Microsoft Learn — Build web apps with ASP.NET Core for beginners](https://learn.microsoft.com/en-us/training/paths/aspnet-core-web-app/) (gratuito)
- [Docs — ASP.NET Core Middleware](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-10.0)
- [Docs — Dependency injection](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-10.0)
- Blog de referência para o resto da trilha: [andrewlock.net](https://andrewlock.net/) — o melhor conteúdo profundo de ASP.NET Core em inglês

**Prova de conhecimento**

Escreva um middleware customizado que loga método, caminho e tempo de resposta de cada requisição. Depois, coloque-o **depois** do `UseStaticFiles` e explique o que muda no log. Explique também o que acontece se você registrar um `DbContext` como Singleton.

> **Checkpoint fase 1** — Me chame aqui. Vou te fazer perguntas sobre stateless, DI scopes e async. Se você passar, seguimos.

---

# FASE 2 — RAZOR PAGES (semanas 5–7)

## Semana 5 — Razor Pages: páginas, handlers e binding

**Objetivo:** O núcleo da stack.

**Conteúdo**

- Razor Pages vs MVC — quando cada um; por que a stack escolheu Pages
- Sintaxe Razor: `@`, `@{ }`, `@model`, `@foreach`, escaping automático
- `PageModel`: `OnGet`, `OnPost`, `OnGetAsync`, handlers nomeados (`OnPostSalvar`)
- Roteamento: `@page`, rotas com parâmetro, `asp-page`, `asp-route-*`
- **Model binding**: `[BindProperty]`, `[BindProperty(SupportsGet = true)]`, binding de coleções
- Validação: DataAnnotations, `ModelState`, `asp-validation-for`, validação customizada (`IValidatableObject`)
- `_Layout.cshtml`, `_ViewStart`, `_ViewImports`, seções
- `TempData` e `PRG` (Post/Redirect/Get) — e por que TempData é uma armadilha em k8s

**Recursos**

- [Learn Razor Pages](https://www.learnrazorpages.com/) — **o melhor recurso gratuito da web sobre o tema**. Leia inteiro.
- [Docs — Tutorial: Get started with Razor Pages](https://learn.microsoft.com/en-us/aspnet/core/tutorials/razor-pages/razor-pages-start?view=aspnetcore-10.0)
- [Docs — Razor Pages architecture and concepts](https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-10.0)

**Prova de conhecimento**

CRUD completo de "Empresas" com validação server-side, mensagens de erro por campo e padrão PRG. Sem JavaScript nenhum. Depois explique: quando o usuário envia o form e a validação falha, quantas requisições HTTP acontecem, e onde os dados digitados ficaram guardados?

---

## Semana 6 — Componentização: Tag Helpers, Partial Views, View Components

**Objetivo:** Escolher errado entre os três é o erro mais comum de time novo em Razor.

**Conteúdo**

- **Tag Helpers built-in**: `asp-for`, `asp-page`, `asp-validation-summary`, `asp-items`, `environment`
- **Tag Helper customizado**: `TagHelper`, `TagHelperOutput`, `[HtmlTargetElement]` — criar um `<cnpj-input>`
- **Partial Views**: `<partial name="…" model="…" />`, quando o modelo vem de fora
- **View Components**: `ViewComponent`, `InvokeAsync`, tem lógica e busca dados próprios
- **A regra de decisão**:
  - precisa gerar HTML a partir de um atributo em uma tag → **Tag Helper**
  - é HTML repetido que recebe dados prontos da página → **Partial View**
  - precisa buscar dados por conta própria e ser independente da página → **View Component**
- Bônus: `Razor Class Library` para compartilhar componentes entre projetos

**Recursos**

- [Docs — Tag Helpers in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/intro?view=aspnetcore-10.0)
- [Docs — Author Tag Helpers](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/tag-helpers/authoring?view=aspnetcore-10.0)
- [Docs — View components](https://learn.microsoft.com/en-us/aspnet/core/mvc/views/view-components?view=aspnetcore-10.0)
- [Learn Razor Pages — Partial Pages](https://www.learnrazorpages.com/razor-pages/partial-pages)

**Prova de conhecimento**

Implemente os três exemplos: um Tag Helper `<cnpj-input>`, uma Partial View de "bloco de endereço", e um View Component "obrigações pendentes" que consulta o banco sozinho. Justifique por escrito por que cada um é do tipo que é.

---

## Semana 7 — Dados: EF Core

**Objetivo:** Sem persistência não há sistema. E EF Core é onde a maioria dos times novos gera lentidão de produção.

**Conteúdo**

- `DbContext`, `DbSet<T>`, configuração por Fluent API vs Data Annotations
- Migrations: `dotnet ef migrations add`, `database update`, e por que **não** usar `EnsureCreated` em produção
- Consultas: `AsNoTracking`, `Include`, projeção com `Select`
- **Problema N+1** — como detectar e resolver
- Change tracking, `SaveChangesAsync`, transações, concorrência otimista (`RowVersion`)
- Quando abandonar EF e usar **Dapper** ou SQL puro (relatórios fiscais pesados)
- Repositório vs `DbContext` direto — o debate, e minha posição: não crie repositório genérico sem motivo

**Recursos**

- [Docs — EF Core](https://learn.microsoft.com/en-us/ef/core/)
- [Docs — Razor Pages with EF Core tutorial](https://learn.microsoft.com/en-us/aspnet/core/data/ef-rp/intro?view=aspnetcore-10.0)
- [EF Core performance docs](https://learn.microsoft.com/en-us/ef/core/performance/) — leia antes de escrever query de relatório
- Ferramenta: [MiniProfiler](https://miniprofiler.com/dotnet/) para ver o SQL gerado

**Prova de conhecimento**

Migre o CRUD da semana 5 para EF Core com SQL Server ou PostgreSQL local (Docker). Escreva uma query que lista empresas com contagem de notas emitidas no mês, e prove — mostrando o SQL gerado — que ela não faz N+1.

> **Checkpoint fase 2** — Traga o projeto. Vou revisar o código e cutucar nas decisões.

---

# FASE 3 — HTMX E INTERATIVIDADE (semanas 8–9)

## Semana 8 — HTMX: o modelo hipermídia

**Objetivo:** A peça central da aposta desta stack. Conceito simples, mas o modelo mental é diferente de tudo que você conhece.

**Conteúdo**

- A ideia central: **o servidor devolve HTML, não JSON**. O cliente troca pedaços do DOM.
- Atributos: `hx-get`, `hx-post`, `hx-target`, `hx-swap` (`innerHTML`, `outerHTML`, `beforeend`, `delete`…)
- `hx-trigger`: eventos, `changed`, `delay:500ms` (debounce), `every 2s` (polling), `load`, `revealed`
- `hx-indicator` — feedback de carregamento
- `hx-swap-oob` (out-of-band) — atualizar mais de uma região numa resposta só
- `hx-boost` — transformar links/forms normais em requisições parciais
- Headers do HTMX: `HX-Request`, `HX-Trigger`, `HX-Redirect`, `HX-Retarget`
- **Detectar no PageModel se a requisição veio do HTMX** e devolver partial em vez de página inteira
- **Antiforgery + HTMX** — o ponto que quebra todo mundo. Configurar token global via `hx-headers` no `<body>`.
- Histórico do navegador (`hx-push-url`) e acessibilidade
- Limites: quando HTMX não basta e você precisa de JS de verdade

**Recursos**

- [Documentação oficial do HTMX](https://htmx.org/docs/) — curta, leia inteira em uma sessão
- [ASP.NET Core Reimagined with htmx (livro gratuito, Chris Woodruff)](https://cwoodruff.github.io/book-aspnet-htmx/) — **o recurso mais completo para esta stack específica**
- [Workshop htmx + Razor Pages (repositório aberto)](https://github.com/cwoodruff/htmx-razor-workshop) — labs práticos
- [JetBrains Guide — HTMX Tag Helpers para ASP.NET Core](https://www.jetbrains.com/guide/dotnet/tutorials/htmx-aspnetcore/razor-taghelpers/)
- [Exemplo CRUD htmx + Razor Pages](https://github.com/mryderie/aspnet-core-htmx-example)
- Ensaio: [Hypermedia Systems (livro online gratuito)](https://hypermedia.systems/) — pelos criadores do HTMX; a filosofia por trás

**Prova de conhecimento**

No projeto: busca de CNPJ que preenche razão social e endereço sem recarregar; tabela de notas com paginação e filtro via HTMX; validação de campo em tempo real (blur → servidor → mensagem). Tudo com antiforgery funcionando. Explique o que trafega na rede em cada caso (abra o Network e mostre).

---

## Semana 9 — Alpine.js, UX e o que fica no cliente

**Objetivo:** HTMX resolve servidor↔cliente. Sobra a lógica puramente visual. É onde entra o Alpine (opcional na stack).

**Conteúdo**

- Alpine.js: `x-data`, `x-show`, `x-model`, `x-on`, `x-init`
- **Onde traçar a linha**: máscaras, mostrar/esconder, modais, abas → Alpine. Regra de negócio, cálculo de imposto, validação real → **servidor, sempre**.
- Máscaras de CPF/CNPJ/CEP/moeda sem lib pesada
- Modais e toasts integrados com HTMX
- CSS: escolher entre Bootstrap 5 (default do template Razor) e Tailwind
- Acessibilidade básica: labels, foco, navegação por teclado — obrigatório em sistema usado 8h/dia
- Otimização: bundling/minificação, cache de estáticos

**Recursos**

- [Alpine.js docs](https://alpinejs.dev/start-here) — curtíssima
- [Livro htmx+ASP.NET, capítulos de UX](https://cwoodruff.github.io/book-aspnet-htmx/)
- [MDN — Acessibilidade](https://developer.mozilla.org/pt-BR/docs/Learn/Accessibility)

**Prova de conhecimento**

Formulário de emissão de NF-e com: máscara de CNPJ (Alpine), cálculo de ICMS/IPI atualizado ao digitar (HTMX → servidor), linhas de item adicionáveis dinamicamente, e total recalculado. Navegável 100% por teclado.

---

# FASE 4 — PRODUÇÃO (semanas 10–12)

## Semana 10 — Segurança e identidade

**Objetivo:** Sistema fiscal. Segurança não é módulo opcional.

**Conteúdo**

- ASP.NET Core Identity: usuários, papéis, claims, cookies
- Autorização: `[Authorize]`, políticas, `AuthorizeFolder` em Razor Pages
- **Data Protection distribuído** — a armadilha que quase toda apresentação da stack omite. Configurar key ring em Redis ou Blob.
- Antiforgery (CSRF) — mecânica completa, e a integração com HTMX
- XSS: por que Razor escapa por padrão e como você quebra isso com `@Html.Raw`
- SQL injection e por que EF/Dapper parametrizado te protege
- HTTPS, HSTS, headers de segurança (CSP, X-Frame-Options)
- Certificados digitais ICP-Brasil em .NET: `X509Certificate2`, A1 vs A3, assinatura XML de NF-e
- Segredos: nunca em `appsettings.json` versionado

**Recursos**

- [Docs — Segurança em ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-10.0)
- [Docs — Configure Data Protection](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-10.0)
- [Andrew Lock — Deploying ASP.NET Core to Kubernetes: tips e edge cases](https://andrewlock.net/deploying-asp-net-core-applications-to-kubernetes-part-12-tips-tricks-and-edge-cases/)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)

**Prova de conhecimento**

Adicione login ao projeto, restrinja páginas por papel, configure Data Protection em Redis via Docker, suba **duas instâncias** da aplicação atrás de um proxy e prove que o login sobrevive ao balanceamento entre elas. Este exercício sozinho já te coloca à frente da maioria das discussões sobre deploy dessa stack.

---

## Semana 11 — Testes e qualidade

**Objetivo:** Sistema fiscal sem teste é passivo, não ativo.

**Conteúdo**

- xUnit: fatos, teorias, fixtures
- Testar `PageModel` isoladamente (mock com NSubstitute ou Moq)
- **Testes de integração** com `WebApplicationFactory` — testar a requisição HTTP de ponta a ponta
- Testar respostas HTMX (verificar que o fragmento certo voltou)
- Banco em teste: SQLite in-memory vs Testcontainers (prefira Testcontainers)
- Testes E2E com Playwright .NET
- Estrutura de solução: separar Domain / Infrastructure / Web
- Análise estática, `.editorconfig`, CI básico com GitHub Actions

**Recursos**

- [Docs — Integration tests in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-10.0)
- [Testcontainers for .NET](https://dotnet.testcontainers.org/)
- [Playwright for .NET](https://playwright.dev/dotnet/)

**Prova de conhecimento**

Cobrir o fluxo de emissão de nota com: testes unitários das regras de cálculo, teste de integração do POST do formulário, e um E2E Playwright do caminho feliz. Rodando em GitHub Actions.

---

## Semana 12 — Deploy, containers e operação

**Objetivo:** Fechar o ciclo. O destino da aplicação é Kubernetes — você precisa saber o que isso implica no código.

**Conteúdo**

- `dotnet publish`, runtime vs SDK image, multi-stage Dockerfile
- Variáveis de ambiente e configuração em container
- Health checks (`/healthz`, readiness vs liveness) — o k8s depende disso
- Graceful shutdown e `IHostApplicationLifetime` (drenar requisições ao encerrar pod)
- Kubernetes: Deployment, Service, Ingress, ConfigMap, Secret, HPA — conceitos, sem virar sysadmin
- Por que "sem afinidade de sessão" só funciona se Data Protection estiver resolvido
- Observabilidade: logging estruturado (Serilog), OpenTelemetry, métricas
- Performance: response caching, output caching, compressão

**Recursos**

- [Docs — .NET em containers](https://learn.microsoft.com/en-us/dotnet/core/docker/introduction)
- [Série completa: Deploying ASP.NET Core to Kubernetes (Andrew Lock)](https://andrewlock.net/series/deploying-asp-net-core-applications-to-kubernetes/) — 12 partes, gratuito, excelente
- [Docs — Health checks](https://learn.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-10.0)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)

**Prova de conhecimento**

Containerizar o FiscalLab, rodar 3 réplicas com docker-compose + nginx, provar que sessão e antiforgery funcionam em qualquer réplica, e que derrubar um container no meio de um uso não perde nada.

---

# Depois das 12 semanas

Não é o fim. Próximos passos por ordem de valor para o seu contexto:

1. **Domínio fiscal brasileiro** — NF-e (layout 4.00), SPED Fiscal/Contribuições, eSocial, Reforma Tributária (CBS/IBS, em transição a partir de 2026). Bibliotecas: `Zeus.Net.NFe.NFCe`, `ACBr` (via wrapper). Esse conhecimento vale mais no mercado que a stack em si.
2. **Arquitetura** — DDD tático, Clean Architecture, CQRS. Cuidado: overengineering é a doença crônica dessa comunidade. Aprenda para saber quando **não** usar.
3. **Migração de legado Delphi** — Strangler Fig pattern, integração gradual, coexistência de sistemas.
4. **Mensageria e background jobs** — Hangfire/Quartz, filas para processar lotes de SPED.

---

# Recursos pagos que valem o dinheiro (opcional)

Nenhum é necessário — a trilha acima é 100% cobrível com material gratuito. Mas se você quiser estrutura de curso guiado em português:

| Recurso | Idioma | Nota |
|---|---|---|
| [Balta.io](https://balta.io/) — trilha .NET | PT-BR | Bom para C# e ASP.NET base. Não cobre HTMX. |
| [Udemy — ASP.NET Core](https://www.udemy.com/pt/topic/aspnet-core/) | PT-BR/EN | Qualidade varia muito. **Verifique a data de atualização** — muito curso ainda em .NET 6/8. Nunca pague preço cheio. |
| [Dometrain](https://dometrain.com/) | EN | Qualidade alta, autores conhecidos da comunidade .NET |

**Alerta honesto:** até hoje não existe curso pago em português cobrindo Razor Pages + HTMX + .NET 10 de forma decente. O livro gratuito do Woodruff é o melhor material que existe para essa combinação, em qualquer idioma. Não gaste dinheiro procurando algo melhor.

---

## Fontes consultadas

- [.NET and .NET Core official support policy](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core) — confirmação de .NET 10 LTS até nov/2028
- [Configure ASP.NET Core Data Protection — Microsoft Learn](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/configuration/overview?view=aspnetcore-10.0)
- [ASP.NET Core Reimagined with htmx — Chris Woodruff](https://cwoodruff.github.io/book-aspnet-htmx/)
- [htmx-razor-workshop — GitHub](https://github.com/cwoodruff/htmx-razor-workshop)
- [Build web apps with ASP.NET Core for beginners — Microsoft Learn](https://learn.microsoft.com/en-us/training/paths/aspnet-core-web-app/)
- [Learn Razor Pages](https://www.learnrazorpages.com/)
- [WebStorm and Rider Are Now Free for Non-Commercial Use — JetBrains](https://blog.jetbrains.com/blog/2024/10/24/webstorm-and-rider-are-now-free-for-non-commercial-use/)
