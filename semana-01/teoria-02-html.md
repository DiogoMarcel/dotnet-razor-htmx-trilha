# Teoria 2 — HTML semântico, formulários e DOM

Leitura: ~1h30. Depois você constrói o protótipo.

---

## 1. HTML não é layout, é significado

Erro nº 1 de quem vem do Delphi: tratar HTML como o `.dfm`, onde você posiciona componentes. HTML **descreve o que a coisa é**; CSS decide como aparece.

```html
<!-- ruim: só um retângulo sem significado -->
<div class="titulo-grande">Empresas cadastradas</div>

<!-- bom: é um cabeçalho de seção, e o navegador/leitor de tela sabe disso -->
<h1>Empresas cadastradas</h1>
```

Por que importa em sistema fiscal usado 8h/dia: leitor de tela, navegação por teclado, e o próprio HTMX (que troca pedaços do DOM) dependem de estrutura correta.

## 2. Anatomia de um elemento

```
<input type="text" name="cnpj" required>
 └─┬─┘ └────┬────┘ └───┬────┘ └──┬───┘
 tag    atributo    atributo   atributo booleano
```

- **Elemento com conteúdo:** `<p>texto</p>` — abre e fecha
- **Elemento vazio:** `<input>`, `<img>`, `<br>` — não fecha
- **Atributo booleano:** presença = verdadeiro. `required` == `required="required"`

## 3. Esqueleto de página

```html
<!DOCTYPE html>
<html lang="pt-BR">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>FiscalLab</title>
  <link rel="stylesheet" href="css/estilos.css">
</head>
<body>
  <!-- conteúdo visível -->
</body>
</html>
```

- `<!DOCTYPE html>` — sem isso o navegador entra em "quirks mode" e o CSS se comporta diferente. Sempre presente.
- `charset="UTF-8"` — sem isso, "Ação" vira "AÃ§Ã£o". Não negociável em PT-BR.
- `viewport` — sem isso o celular renderiza a página em 980px e encolhe tudo.
- `<head>` = metadados (não aparece). `<body>` = o que se vê.

## 4. Tags estruturais que você vai usar

| Tag | Papel |
|---|---|
| `<header>` | topo da página ou de uma seção |
| `<nav>` | bloco de navegação (o menu lateral) |
| `<main>` | o conteúdo principal — **um só por página** |
| `<section>` | agrupamento temático, normalmente com título |
| `<article>` | conteúdo que faz sentido isolado |
| `<aside>` | conteúdo lateral/complementar |
| `<footer>` | rodapé |
| `<div>` | **sem significado** — só para agrupar e estilizar |
| `<span>` | `<div>` em versão inline (dentro de um texto) |

Regra: use `<div>` quando nenhuma tag semântica se aplica. Não o contrário.

**Bloco vs inline:** `<div>`, `<p>`, `<h1>` ocupam a linha toda (bloco). `<span>`, `<a>`, `<strong>` ficam no fluxo do texto (inline). Isso muda como o CSS os trata.

## 5. Formulários — o coração de sistema fiscal

```html
<form action="/empresas/criar" method="post">
  <label for="razaoSocial">Razão social</label>
  <input type="text" id="razaoSocial" name="razaoSocial" required>

  <button type="submit">Salvar</button>
</form>
```

Três atributos do `<form>` decidem tudo:

- `action` — para qual URL vai
- `method` — `get` (dados na query string, `?razaoSocial=x`) ou `post` (dados no corpo). **Sempre `post` quando altera algo.**
- `enctype` — só importa com upload: `multipart/form-data`

### `name` é o que importa, não `id`

Esta é a confusão nº 1 de iniciante:

- **`name`** → é a chave que **vai para o servidor**. Sem `name`, o campo não é enviado. Ponto.
- **`id`** → identificador único **na página**, usado pelo `<label for="...">` e pelo CSS/JS.

Na Semana 5 o Tag Helper `asp-for="Empresa.Cnpj"` vai gerar `name="Empresa.Cnpj"` automaticamente, e o model binding do ASP.NET Core usa esse `name` para preencher sua classe C#. Entender isso agora economiza uma semana de confusão depois.

### `<label>` não é enfeite

`<label for="cnpj">` conectado a `<input id="cnpj">` faz:
- clicar no texto foca o campo (área de clique maior)
- leitor de tela anuncia o nome do campo
- é requisito de acessibilidade

### Tipos de input úteis

| `type` | Uso |
|---|---|
| `text` | genérico |
| `email` | valida formato, teclado com `@` no celular |
| `number` | numérico; aceita `min`, `max`, `step` |
| `date` | seletor de data nativo |
| `checkbox` | sim/não — **cuidado: se desmarcado, não é enviado** |
| `radio` | escolha única — mesmo `name` no grupo |
| `hidden` | invisível, mas enviado. É estado carregado no HTML (Teoria 1, item 6) |
| `password` | mascarado |

Seleção:

```html
<label for="uf">UF</label>
<select id="uf" name="uf">
  <option value="">Selecione…</option>
  <option value="SP">São Paulo</option>
  <option value="MG" selected>Minas Gerais</option>
</select>
```

`value` é o que vai para o servidor; o texto é o que o usuário vê.

### Validação nativa do HTML — e por que ela não basta

`required`, `minlength`, `pattern`, `type="email"` fazem o navegador barrar o envio. Ótimo para UX.

**Mas é validação de conveniência, não de segurança.** Qualquer pessoa abre o DevTools, remove o `required`, e envia. Ou envia direto por `curl`, sem navegador nenhum.

> **Regra sem exceção: toda validação acontece no servidor. A do cliente é bônus.**

Vale ainda mais para cálculo de imposto — nunca no cliente.

## 6. Tabelas

Tabela é para **dado tabular**. Listagem de empresas, notas, itens: correto usar `<table>`. Para layout de página: nunca (isso morreu em 2005, use CSS Grid).

```html
<table>
  <caption>Empresas cadastradas</caption>
  <thead>
    <tr>
      <th scope="col">Razão social</th>
      <th scope="col">CNPJ</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td>Padaria do João</td>
      <td>12.345.678/0001-99</td>
    </tr>
  </tbody>
</table>
```

`<thead>`/`<tbody>` não são decoração: na Semana 8 o HTMX vai trocar **só o `<tbody>`** ao paginar ou filtrar. Estrutura correta hoje = HTMX fácil depois.

## 7. O DOM

Você escreve um arquivo `.html` — texto morto. O navegador lê esse texto e constrói uma **árvore de objetos em memória**: o **DOM** (Document Object Model).

```
document
└── html
    ├── head
    │   └── title → "FiscalLab"
    └── body
        ├── nav
        └── main
            └── table
                └── tbody
                    └── tr → td, td
```

Três consequências:

1. **O arquivo é o ponto de partida, não a verdade corrente.** "Ver código-fonte" (Ctrl+U) mostra o arquivo original; a aba Elements mostra o DOM **agora**. Se JS alterou algo, os dois diferem.
2. **JavaScript manipula a árvore**, não o texto. `document.querySelector('#cnpj').value = '123'` muda o objeto, e a tela reflete.
3. **HTMX opera no DOM.** `hx-target="#tabela"` + `hx-swap="innerHTML"` significa: "pegue o HTML que o servidor devolveu e substitua os filhos deste nó da árvore". Sem entender DOM, HTMX é mágica — e mágica você não depura.

Não precisa aprender JavaScript agora. Precisa entender que **a página é uma árvore viva de objetos**.

---

## Checklist de saída

- [ ] Diferença entre `name` e `id` num input, e qual chega ao servidor
- [ ] Por que validação de cliente não substitui a de servidor
- [ ] Diferença entre elemento bloco e inline
- [ ] O que é o DOM e por que difere do arquivo `.html`
- [ ] Para que serve `<thead>`/`<tbody>` além de estética

Próximo: [`teoria-03-css.md`](teoria-03-css.md)
