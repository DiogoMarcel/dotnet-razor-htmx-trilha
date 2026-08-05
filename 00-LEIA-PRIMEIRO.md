# Guia de Estudos — Razor Pages + HTMX sobre .NET 10

**Para:** Diogo Marcel · **Base:** Delphi, sem experiência web/.NET · **Ritmo:** 10–15h/semana · **Duração:** 12 semanas

---

## Parte 1 — Como este guia funciona

### Arquivos de raiz

| Arquivo | O que é | Quando abrir |
|---|---|---|
| `PROGRESSO.md` | **Estado do estudo.** Onde você parou, o que está pendente. | Toda sessão, antes e depois |
| `CLAUDE.md` | Contexto compartilhado entre Cowork e Claude Code | Só se precisar ajustar como as IAs te tratam |
| `00-LEIA-PRIMEIRO.md` | Este. Como o guia funciona + mapa Delphi→Web | Uma vez, e revisitar quando se perder |
| `01-trilha-12-semanas.md` | O currículo completo, com recursos externos | Ao planejar a semana |
| `02-projeto-fiscallab.md` | O projeto prático, visão de ponta a ponta | Ao planejar a semana |

### Pastas por semana

Cada semana tem sua pasta com o material detalhado:

```
semana-NN/
├── README.md              <- comece por aqui: ordem, tempo, o que não entra
├── teoria-NN-*.md         <- 3 blocos de ~2h
├── projeto/ prototipo/    <- código, com GUIA-*.md dizendo o que construir
├── Exercícios/            <- suas respostas
└── Corrigir.txt           <- minha devolutiva depois da revisão
```

**Fluxo:** `PROGRESSO.md` → `semana-NN/README.md` → teoria → projeto → prova → traz para revisão → `Corrigir.txt` → aplica → próxima semana.

### Trabalhando em duas ferramentas

Você usa **Cowork** e **Claude Code**. Para não haver divergência:

- **`PROGRESSO.md` é a fonte única de verdade.** Sempre atualize ao fim da sessão, seja qual for a ferramenta.
- **Claude Code** — melhor para: gerar material das semanas, escrever/rodar código, refatorar, debugar erro de compilação.
- **Cowork (aqui)** — melhor para: revisão crítica, sabatinas, discutir arquitetura, pesquisar na web, questionar decisões de arquitetura.
- `CLAUDE.md` é lido automaticamente pelo Claude Code. Se você mudar a forma como quer ser ensinado, mude lá — vale para as duas.

**Regra de ouro:** nunca estude um módulo sem construir a parte correspondente do projeto na mesma semana. Leitura sem código não fixa nada — especialmente vindo de outra stack.

### O ciclo semanal sugerido (12h)

| Bloco | Tempo | O quê |
|---|---|---|
| Teoria | 3h | Curso/doc do módulo da semana |
| Prática guiada | 4h | Refazer os exemplos por conta própria, sem copiar |
| Projeto | 4h | Implementar a feature da semana no FiscalLab |
| Revisão comigo | 1h | Você me traz o código, eu reviso e te faço perguntas difíceis |

O bloco de revisão é o mais importante. Traga o código, não o resumo.

---

## Parte 2 — Mapa mental: Delphi → Web

Sua maior barreira não é sintaxe C#. É o modelo de execução. Traduções úteis:

| Delphi | Equivalente web / .NET | Diferença crítica |
|---|---|---|
| `TForm` | Página Razor (`.cshtml` + `PageModel`) | O form **não fica vivo**. É recriado a cada requisição. |
| Variável de campo do form | Nada equivalente | Estado morre no fim da requisição. Persistir = banco/cookie/hidden field |
| `OnClick` | Handler HTTP (`OnPostSalvar`) | Não é chamada direta — é uma requisição HTTP que atravessa a rede |
| `TDataSet` / `TQuery` | EF Core `DbSet` / Dapper | Sem cursor aberto. Você busca, materializa e a conexão fecha |
| `.dfm` | `.cshtml` (HTML + Razor) | Layout é CSS/fluxo de documento, não coordenadas absolutas |
| Unidade `uses` | `using` + injeção de dependência | Dependências vêm pelo construtor, não por variável global |
| DLL / BPL | NuGet package | |
| Compilar e rodar `.exe` | `dotnet run` → servidor HTTP | Sua app é um servidor, não um programa de janela |

Interiorize a linha 2 da tabela. **Ausência de estado entre requisições** é a única ideia que, se você não entender de verdade, vai te travar em tudo o resto.

---

## Parte 3 — O que você vai conseguir fazer ao final

Depois das 12 semanas você deve conseguir, sem ajuda:

- Criar um projeto ASP.NET Core do zero e explicar cada linha do `Program.cs`
- Construir formulários com validação server-side e feedback instantâneo via HTMX
- Modelar dados com EF Core, escrever migrations e queries de relatório
- Criar Tag Helpers, Partial Views e View Components e saber **quando usar cada um**
- Proteger a aplicação: autenticação, autorização, antiforgery com HTMX, Data Protection distribuído
- Escrever testes de unidade e de integração
- Empacotar em container e explicar o que muda ao rodar múltiplas réplicas
- Ler uma proposta de arquitetura para essa stack e discordar dela com argumentos

---

## Próximo passo

Abra `PROGRESSO.md` para ver onde você parou. Depois vá para o `README.md` da semana ativa.
