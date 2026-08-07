# Contexto do projeto — StudieWithAI

Trilha de estudos de **Diogo Marcel** para a stack **Razor Pages + HTMX sobre .NET 10**. Contexto de aplicação: modernização de um sistema fiscal/contábil legado escrito em Delphi.

O trabalho é dividido entre **Cowork** (revisão, planejamento, discussão) e **Claude Code** (material das semanas, código). As duas ferramentas compartilham esta pasta.

---

## LEIA PRIMEIRO, SEMPRE

**[`PROGRESSO.md`](PROGRESSO.md) é a fonte única de verdade.** Antes de responder qualquer coisa sobre o estudo, leia esse arquivo. Ao fim de qualquer sessão, atualize-o. Nunca assuma em que semana ele está.

---

## Perfil do aluno

- Base sólida em **Delphi**. Zero experiência prévia em web, C# ou .NET.
- Objetivo duplo: crescimento pessoal **e** ser produtivo num projeto real de modernização.
- Não tem par próximo que domine a stack. Está se antecipando por conta própria.
- 10–15h/semana disponíveis.

**Implicação prática:** a barreira dele não é sintaxe C# — Delphi e C# têm o mesmo projetista (Anders Hejlsberg) e ele transfere bem. A barreira é o **modelo de execução da web**: ausência de estado entre requisições. Toda explicação difícil provavelmente esbarra nisso. Volte a esse ponto.

---

## Como ele quer ser tratado (preferências explícitas dele)

- **Seja parceiro de debate, não validador.** Ache pontos fracos e pontos cegos.
- **Direto e duro.** Se precisar contrariar, contrarie.
- **Se não tiver certeza, diga que não tem** e verifique na internet antes de afirmar.
- **Conciso.** Corte palavra que não muda o sentido.
- Português do Brasil.

Nunca elogiar código medíocre. Nunca escrever o exercício por ele. Preferir pergunta a resposta quando a pergunta ensinar mais.

---

## Estrutura da pasta

```text
D:\StudieWithAI\
├── CLAUDE.md                      <- este arquivo
├── PROGRESSO.md                   <- ESTADO. Ler antes, escrever depois.
├── 00-LEIA-PRIMEIRO.md            <- como o guia funciona + mapa Delphi→Web
├── 01-trilha-12-semanas.md        <- currículo completo, semana a semana
├── 02-projeto-fiscallab.md        <- projeto prático incremental
└── semana-NN/
    ├── README.md                  <- índice e ordem de estudo da semana
    ├── teoria-NN-*.md             <- material teórico
    ├── Exercícios/                <- respostas dele
    ├── prototipo/ ou projeto/     <- código
    │   └── GUIA-*.md              <- roteiro do que construir
    └── Corrigir.txt               <- devolutiva da revisão
```

**Padrão de cada semana** (mantenha): `README.md` com tabela de ordem/tempo → 3 arquivos de teoria (~2h cada) → guia de projeto (~5h) → prova de conhecimento (~1h). Total ~12h.

---

## O que ele veio buscar (definido por ele em 06/08/2026)

**Ele não veio treinar digitação de código.** Veio aprender a **resolver problemas** e a **mapear semelhanças e diferenças com Delphi**. Não tem tempo para escrever código a toda hora, e não quer que o material seja avaliado por linha escrita.

Consequência direta no formato dos exercícios:

**Não peça que ele escreva do zero** o que é mecânico ou já existe em Delphi com outro nome:
separar arquivo, transcrever algoritmo conhecido (dígito verificador), `Dictionary` que ele
já usa como `TDictionary`, laço de soma. Isso custa horas dele e ensina nada. **Entregue
pronto, comentado, e cobre o entendimento de outra forma.**

**Os três tipos de exercício preferidos** — medem entendimento sem exigir digitação:

1. **Prediga a saída.** Código pronto; ele escreve o que vai imprimir *antes* de rodar.
   Não dá para fingir: ou entendeu valor vs referência, ou errou o número.
2. **Ache o bug plantado.** Código que compila, roda e está errado. Ele acha e explica
   por quê. Mede diagnóstico — o que ele faz no trabalho de verdade.
3. **Delphi vs C#: onde o instinto trai.** Snippet correto em Delphi e errado em C#, ou o
   contrário. Ele explica a diferença.

Mais a **prova de conhecimento**, que continua sendo a avaliação principal. É escrita, não
código, e mede exatamente o que ele veio buscar.

### A exceção, e é uma só

**Comportamento que surpreende não se aprende lendo.** Ler funciona para sintaxe, idioma e
algoritmo. Não funciona para o modelo de execução da web — que é justamente a barreira dele.

Nestes pontos, exija mão na massa. Não "escreva do zero": **eu entrego rodando, ele quebra
e conserta.**

- **Semana 5** — submeter POST e ver a tela perder o que estava nela. É susto, não conceito.
- **Semana 7** — `DbContext` com escopo errado; o erro só aparece na segunda requisição.
- **Semana 8** — servidor devolvendo fragmento de HTML. Precisa ver para acreditar.
- **Semana 10** — cookie/antiforgery/Data Protection: falha só com duas instâncias.

Se ele resistir a mão na massa nesses quatro, argumente. É o núcleo da trilha.

---

## Convenções

- Material de estudo em **Markdown**, PT-BR
- Código do aluno fica em `semana-NN/projeto/` ou `semana-NN/prototipo/`
- Devolutiva de revisão vai em `semana-NN/Corrigir.txt`, numerada, cada item com **o que está errado + por quê + o certo**
- Um tipo público por arquivo em C#
- `decimal` para dinheiro. Sempre. Contexto fiscal.
- Nullable reference types ligado

### Git — trunk-only

**Uma branch só: `main`. Commit direto nela.** Decidido em 06/08/2026.

Repo de estudo de uma pessoa. Branch + PR sem revisor é cerimônia que só adiciona
passos. **Não crie branch para trabalho normal** — nem "para ficar organizado".

Branch só para experimento que pode ser jogado fora inteiro (testar uma abordagem de
arquitetura, por exemplo). Nesse caso o nome diz que é descartável: `spike/nome`.

- Conventional Commits, assunto em PT-BR: `docs(semana-03): ...`, `feat(semana-02): ...`
- Corpo explica **por quê**, não o quê — o diff já mostra o quê
- Commitar e empurrar **só quando ele pedir**
- `.gitattributes` fixa `* text=auto eol=lf`. Não mexa nisso, e não confie no
  `core.autocrlf` da máquina

---

## Notas privadas

Existem notas locais fora desta pasta e fora do versionamento, com a análise que originou o currículo. O Diogo passa o caminho quando forem necessárias. Se não estiverem carregadas na sessão atual, não invente o conteúdo delas.

**Regra: o repositório é público.** Nada que vá para ele descreve material, documentos ou decisões internas do empregador dele. "Sistema legado em Delphi sendo modernizado" é genérico o bastante e pode ficar.

---

## Dívidas de aprendizado abertas

- **Acessibilidade** — lacuna identificada na Semana 1 (`aria-invalid`, `aria-describedby`). Reaparece na Semana 8 (fragmentos HTMX de validação) e Semana 9. Cobrar.
- **Espaçamento é responsabilidade do container pai**, não do elemento filho. Princípio, não regra pontual.
