# Dívida 5 — acessibilidade (Semana 1, aberta desde 02/08/2026)

Esta é a dívida mais antiga da trilha. Não é código para rodar — é leitura e julgamento.

**Antes de escrever este arquivo eu fui verificar o estado real dos arquivos, e o que
encontrei mudou o exercício.** Vale te contar primeiro, porque é o tipo de descoberta que
você vai ter que fazer sozinho revisando código de IA.

---

## O que eu encontrei

As 3 correções de código **estão aplicadas** em `semana-01/prototipo/`:

| # | Correção | Estado em `prototipo/` |
|---|---|---|
| 1 | `style="margin-top"` inline em `detalhe.html` | ✅ removido. `.conteudo` tem `display:flex` + `gap: var(--espaco-lg)` |
| 2 | `aria-invalid` + `aria-describedby` no CNPJ | ✅ presentes em `cadastro.html:98-99`, com `id="ajuda-cnpj"` e `id="erro-cnpj"` ligados |
| 3 | `.dados dd { text-align: right }` | ✅ removido. Sobrou só `margin: 0` |

**Mas há um problema, e é o exercício.**

O `git log` mostra que `prototipo/cadastro.html` foi tocado por **um commit só** — o inicial
da trilha. E os comentários que estão dentro dele são meus, com a redação do meu próprio
`Corrigir.txt` quase palavra por palavra:

```html
<!-- Sem os dois, o campo continua "válido e sem erro" no leitor.
     Na Semana 8 o HTMX devolve este fragmento no blur do campo — -->
```

Ou seja: **eu não consigo distinguir "ele aplicou e entendeu" de "a ferramenta aplicou e ele
leu".** O arquivo estar correto não é evidência de que você sabe.

E tem um dado que resolve a dúvida:

> `semana-01/Exercícios/cadastro_empresa.html` — **o arquivo que VOCÊ escreveu** — tem
> **zero** `aria-invalid` e **zero** `aria-describedby`.

Linha 116, o seu campo de CNPJ, inteiro:

```html
<input type="text" id="cnpj" name="cnpj" required placeholder="00.000.000/0000-00">
```

O protótipo de referência está certo. O seu arquivo não. Isso é a resposta.

**Nota de método:** o que eu acabei de fazer — desconfiar de um arquivo correto, olhar o
histórico, achar a fonte, e cruzar com outro arquivo — é exatamente o trabalho que você vai
fazer revisando código de IA. O código estava certo; a pergunta era *quem o escreveu e quem
entendeu*. Guarde o movimento.

---

## Exercício — ache o que falta no seu próprio código

Sem consultar o `prototipo/cadastro.html`. Este é o seu campo de CNPJ, como você escreveu:

```html
<div class="form-group">
    <label for="cnpj">CNPJ</label>
    <input type="text" id="cnpj" name="cnpj" required placeholder="00.000.000/0000-00">
</div>
```

Cenário: o operador digitou um CNPJ com dígito verificador errado. O servidor devolveu a
página com o campo marcado em vermelho e a mensagem "CNPJ inválido." abaixo dele.

### 5.1

Um operador que usa leitor de tela (NVDA, JAWS) chega neste campo com Tab. **O que exatamente
o leitor anuncia?** Liste o que ele fala, em ordem.

**Resposta:**

<!-- escreva aqui -->

### 5.2

O que ele **não** fala, e deveria?

**Resposta:**

<!-- escreva aqui -->

### 5.3

Escreva o campo corrigido. Você pode consultar a teoria, mas não o `prototipo/`.

**Resposta:**

```html
<!-- escreva aqui -->
```

### 5.4

`aria-describedby` aceita uma lista separada por espaço. **Por que isso importa neste caso
específico**, e o que se perde se você apontar para um `id` só?

**Resposta:**

<!-- escreva aqui -->

### 5.5 — a que liga com a Semana 8

Na Semana 8, o HTMX vai disparar uma requisição no `blur` do campo de CNPJ, o servidor valida
o dígito verificador, e devolve **um fragmento de HTML** que substitui só aquele pedaço da
página.

Esse fragmento é gerado pelo servidor, em C#, num Razor Pages.

**Pergunta:** se o `aria-invalid` e o `aria-describedby` estiverem no HTML da página mas
**não** no fragmento que o servidor devolve, o que acontece na prática para o operador que
usa leitor de tela? Ele fica melhor, igual ou pior do que estava antes do HTMX?

**Resposta:**

<!-- escreva aqui -->

---

## 5.6 — Item 4 da Semana 1, que era uma pergunta e não uma correção

Do `semana-01/Corrigir.txt`:

> Exercício 4 já estava pronto. CEP `campo--3` + Logradouro `campo--6` + Número `campo--3` =
> 12. Era assim no meu arquivo original. **Ou você conferiu e viu que já somava, ou passou
> batido — me diga qual, porque muda o que eu explico.**

Está aberta desde 02/08. Responda honestamente — "passei batido" é uma resposta útil e
"conferi" sem ter conferido não é.

**Resposta:**

<!-- escreva aqui -->

---

## Por que eu não deixo esta dívida passar

Você vai construir telas para operador fiscal usar **8 horas por dia**. Nesse público,
acessibilidade não é conformidade legal — é gente com baixa visão, gente que navega só por
teclado porque é mais rápido que mouse, e gente que vai ficar nessa tela até se aposentar.

E tem o lado técnico: `aria-invalid`/`aria-describedby` num HTML estático é fácil. Num
**fragmento gerado pelo servidor** — que é o modelo da Semana 8 inteira — é fácil esquecer,
porque o fragmento é montado em outro arquivo, longe do formulário original. Quem não sabe
que os atributos precisam estar lá não os coloca.

É por isso que esta dívida trava a Semana 8, e é a única das cinco que trava algo.
