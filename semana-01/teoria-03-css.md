# Teoria 3 — CSS: box model, seletores, Flexbox e Grid

Leitura: ~1h30. Depois: Flexbox Froggy + Grid Garden (1h cada, jogos — valem mais que qualquer vídeo).

---

## 1. A mudança de mentalidade

No Delphi você posiciona por coordenada: `Left := 24; Top := 100;`. O componente fica ali, independente do resto.

CSS é **fluxo de documento**: elementos se empilham naturalmente e você ajusta o *comportamento* do fluxo. Posicionamento absoluto existe, mas usá-lo como padrão é o erro clássico de quem vem do desktop — quebra em qualquer resolução diferente da sua.

## 2. Sintaxe

```css
seletor {
  propriedade: valor;
}

.tabela-empresas th {
  background-color: #eef2f7;
  padding: 12px 16px;
}
```

Três formas de aplicar — use **sempre** a terceira:

```html
<p style="color: red">inline — evite, impossível de manter</p>
<style> p { color: red } </style>   <!-- no head — só para teste -->
<link rel="stylesheet" href="css/estilos.css">   <!-- arquivo externo — correto -->
```

## 3. Seletores

| Seletor | Alcança | Peso |
|---|---|---|
| `*` | tudo | 0 |
| `p` | toda tag `<p>` | 1 |
| `.cartao` | `class="cartao"` | 10 |
| `#menu` | `id="menu"` | 100 |
| `[type="text"]` | por atributo | 10 |
| `nav a` | `<a>` **dentro de** `<nav>` (qualquer nível) | soma |
| `nav > a` | `<a>` **filho direto** de `<nav>` | soma |
| `a:hover` | estado | 10 |
| `tr:nth-child(even)` | linhas pares (zebra da tabela) | 10 |

### Especificidade — a fonte de 90% dos "meu CSS não aplica"

Quando duas regras disputam o mesmo elemento, vence a de **maior peso**. Empate → vence a que aparece **por último** no arquivo.

```css
#form-empresa input { border: 2px solid blue; }  /* peso 101 */
.campo-erro        { border: 2px solid red;  }  /* peso 10  → PERDE */
```

A borda fica azul mesmo o erro sendo mais específico semanticamente. Por isso: **prefira classes, evite `id` no CSS**, e não use `!important` para vencer na força (ele só empurra o problema).

**Faça agora:** F12 → Elements → selecione um elemento → painel Styles. Regras riscadas são as que perderam. Esse painel resolve qualquer dúvida de especificidade em 5 segundos.

### Cascata e herança

- **Herança:** propriedades de texto (`color`, `font-family`, `font-size`) descem para os filhos. Por isso define-se a fonte uma vez no `body`. Já `border`, `padding`, `margin` **não** herdam.
- **Cascata:** ordem das folhas de estilo importa. A sua vem depois do framework para poder sobrescrever.

## 4. Box model

Todo elemento é uma caixa com quatro camadas, de dentro para fora:

```
┌─────────── margin (fora, transparente) ────────────┐
│ ┌───────── border ─────────────────────────────┐   │
│ │ ┌─────── padding (dentro, pinta o fundo) ──┐ │   │
│ │ │            content                       │ │   │
│ │ └──────────────────────────────────────────┘ │   │
│ └──────────────────────────────────────────────┘   │
└────────────────────────────────────────────────────┘
```

O ajuste obrigatório em todo projeto:

```css
*, *::before, *::after {
  box-sizing: border-box;
}
```

Sem ele (`content-box`, o padrão), `width: 300px` + `padding: 20px` + `border: 1px` = elemento de **342px** na tela. Com `border-box`, `width: 300px` significa 300px totais, padding e borda incluídos. É o comportamento que sua intuição espera.

**Colapso de margem:** margens verticais adjacentes se fundem — `margin-bottom: 20px` seguido de `margin-top: 30px` resulta em 30px, não 50px. Não é bug. Não acontece em containers flex/grid (mais um motivo para usá-los).

### Unidades

| Unidade | O que é | Quando usar |
|---|---|---|
| `px` | pixel absoluto | bordas, sombras |
| `rem` | múltiplo da fonte-raiz (16px padrão) | **espaçamentos e fontes** — respeita o zoom do usuário |
| `em` | múltiplo da fonte do próprio elemento | dentro de componentes |
| `%` | relativo ao pai | larguras |
| `fr` | fração do espaço livre | só em Grid |
| `vh`/`vw` | 1% da altura/largura da viewport | telas cheias |

Padrão razoável: `rem` para espaçamento e tipografia, `px` só para detalhes finos.

## 5. Flexbox — layout em **uma** dimensão

Use quando os itens correm numa linha *ou* numa coluna: barra de navegação, grupo de botões, cabeçalho com logo à esquerda e usuário à direita.

```css
.barra {
  display: flex;              /* ativa no CONTAINER, afeta os FILHOS diretos */
  flex-direction: row;        /* row (padrão) | column */
  justify-content: space-between;  /* alinha no eixo PRINCIPAL */
  align-items: center;             /* alinha no eixo CRUZADO */
  gap: 1rem;                       /* espaço entre itens — melhor que margin */
}
```

O conceito que trava todo mundo: **eixo principal e eixo cruzado trocam de lugar** conforme o `flex-direction`.

- `flex-direction: row` → principal = horizontal, cruzado = vertical
- `flex-direction: column` → principal = **vertical**, cruzado = horizontal

Ou seja, `justify-content: center` centraliza horizontalmente no `row` e **verticalmente** no `column`. Sabendo disso, Flexbox deixa de ser tentativa e erro.

Valores de `justify-content`: `flex-start`, `center`, `flex-end`, `space-between`, `space-around`, `space-evenly`.
Valores de `align-items`: `stretch` (padrão), `center`, `flex-start`, `flex-end`, `baseline`.

Nos **filhos**:

```css
.item { flex: 1; }        /* cresce e divide o espaço livre igualmente */
.fixo { flex: 0 0 240px; } /* não cresce, não encolhe, 240px fixos */
```

`flex: 1` no conteúdo + `flex: 0 0 240px` no menu lateral = layout de sistema administrativo resolvido.

## 6. Grid — layout em **duas** dimensões

Use quando há linhas **e** colunas: a estrutura geral da página, um formulário de dois campos por linha, um painel de cartões.

```css
.pagina {
  display: grid;
  grid-template-columns: 240px 1fr;   /* menu fixo | conteúdo elástico */
  grid-template-rows: auto 1fr auto;  /* header | main | footer */
  min-height: 100vh;
  gap: 0;
}
```

Com **áreas nomeadas** fica legível — e é o que uso no protótipo:

```css
.pagina {
  display: grid;
  grid-template-columns: 240px 1fr;
  grid-template-rows: auto 1fr;
  grid-template-areas:
    "menu cabecalho"
    "menu conteudo";
  min-height: 100vh;
}
.menu      { grid-area: menu; }
.cabecalho { grid-area: cabecalho; }
.conteudo  { grid-area: conteudo; }
```

Você literalmente desenha o layout em ASCII dentro do CSS.

Ferramentas úteis:

```css
/* colunas responsivas sem media query: quantas couberem, mínimo 260px */
grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
```

### Regra de decisão

| Situação | Use |
|---|---|
| itens numa linha ou numa coluna | **Flexbox** |
| linhas e colunas juntas | **Grid** |
| estrutura geral da página | **Grid** |
| barra de botões, navbar, campo+label | **Flexbox** |

Grid por fora, Flexbox por dentro. Não competem.

## 7. Responsivo

```css
/* mobile-first: escreva o estilo base para tela pequena… */
.pagina { grid-template-areas: "cabecalho" "conteudo"; }

/* …e adicione o que muda em telas maiores */
@media (min-width: 768px) {
  .pagina {
    grid-template-columns: 240px 1fr;
    grid-template-areas: "menu cabecalho" "menu conteudo";
  }
}
```

Sistema fiscal roda em desktop 99% do tempo, então aqui não é prioridade. Mas custa 5 linhas.

## 8. Variáveis CSS

```css
:root {
  --cor-primaria: #1b4b8f;
  --espaco: 1rem;
}
.botao { background: var(--cor-primaria); padding: var(--espaco); }
```

Nativas, sem build step. Trocar a cor do sistema inteiro = editar uma linha.

---

## Checklist de saída

- [ ] O que `box-sizing: border-box` muda e por que você sempre o usa
- [ ] Por que `#menu .item` vence `.item.destaque`
- [ ] Eixo principal vs cruzado no Flexbox, e o que muda com `flex-direction: column`
- [ ] Quando Grid e quando Flexbox
- [ ] Diferença entre `rem` e `px`, e por que `rem` para espaçamento

Próximo: construir o protótipo em [`prototipo/`](prototipo/) — abra o `GUIA-PROTOTIPO.md`.
