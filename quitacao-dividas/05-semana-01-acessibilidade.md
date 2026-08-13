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

Fala ou anuncia um campo de texto e que é obrigatório.
Um label: CNPJ
Um editor: Caixa de texto
Um estado obrigatório: Com o atributo '*'.
E a configuração da máscara: "00.000.000/0000-00".

> **Correção (12/08).** As quatro peças estão certas. Duas imprecisões:
>
> **1. "Com o atributo `*`" está errado.** Não há asterisco nenhum no seu HTML. O que faz o
> leitor dizer "obrigatório" é o atributo **`required`**, que o navegador mapeia para
> `aria-required="true"` na árvore de acessibilidade. Asterisco é convenção **visual**, para
> quem enxerga — e sozinho ele não anuncia nada, porque leitor de tela lê a árvore, não o
> desenho.
>
> **2. "A configuração da máscara" não é máscara — é o `placeholder`.** Seu campo não tem
> máscara alguma; é um `input type="text"` puro. E o comportamento dele é o motivo de
> `placeholder` nunca substituir dica:
>
> - o leitor o anuncia como **descrição** do campo, e só porque não existe `aria-describedby`
>   — quando existir, o `describedby` vence e **o `placeholder` deixa de ser lido**;
> - ele some da tela no instante em que o operador digita o primeiro caractere. A instrução
>   de formato evapora exatamente quando ele está tentando acertar o formato.
>
> **Ordem real do anúncio**, que é o que a pergunta pediu: nome (`CNPJ`, do `<label for>`) →
> papel (`edição` / `caixa de texto`) → valor (vazio) → estado (`obrigatório`) → descrição
> (o `placeholder`, na falta de coisa melhor).

### 5.2

O que ele **não** fala, e deveria?

**Resposta:**

Não fala que o campo está inválido e não lê a mensagem de erro que deveria ser lida.

### 5.3

Escreva o campo corrigido. Você pode consultar a teoria, mas não o `prototipo/`.

**Resposta:**

```html
    <div class="form-group">
        <label for="cnpj">CNPJ</label>
        <input type="text" id="cnpj" name="cnpj" required placeholder="00.000.000/0000-00" aria-invalid="true" aria-describedby="ajuda-cnpj erro-cnpj">
        <span class="campo__ajuda" id="ajuda-cnpj">Somente números ou com pontuação.</span>
        <span class="campo__erro" id="erro-cnpj">CNPJ inválido.</span>
    </div>
```

> **Correção (12/08).** Aprovado para o cenário do enunciado. Duas coisas a exigir de
> qualquer IA que gere isso pra você:
>
> **1. `aria-invalid="true"` fixo no HTML é bug.** Aqui está certo porque o enunciado diz
> que o servidor devolveu a página **com erro**. Mas o atributo é **estado**, não decoração:
> no primeiro carregamento, com o campo vazio e nada validado ainda, `aria-invalid="true"`
> faz o leitor anunciar "inválido" antes de o operador ter digitado qualquer coisa. Num
> Razor Page é condicional — `aria-invalid="@(temErro ? "true" : "false")"` ou o atributo
> some. Mesma lógica para o `<span class="campo__erro">`: se não há erro, ele não existe.
>
> **2. Cuidado com `id` órfão.** Se o `erro-cnpj` sumir do DOM mas o `aria-describedby`
> continuar apontando pra ele, o leitor simplesmente **não lê nada** — sem aviso, sem erro
> no console, sem nada quebrado visualmente. É o tipo de defeito que passa em revisão e é a
> razão de a Semana 8 ser onde isso morde.
>
> Um detalhe que você acertou sem comentar: a **ordem** dentro do `aria-describedby` é a
> ordem de leitura. `"ajuda-cnpj erro-cnpj"` faz o leitor dizer o formato esperado e **depois**
> o erro. É a ordem útil.

### 5.4

`aria-describedby` aceita uma lista separada por espaço. **Por que isso importa neste caso
específico**, e o que se perde se você apontar para um `id` só?

**Resposta:**

Porque o atributo aceita lista com espaço, permitindo ler ajuda e erro em conjunto.

> **Correção (12/08). É a resposta mais fraca do arquivo, e é a única que eu não aceito.**
>
> Você repetiu o enunciado. "Aceita lista separada por espaço" foi o que **eu** escrevi na
> pergunta; a pergunta era **por que isso importa neste caso** e **o que se perde apontando
> para um `id` só**. A segunda metade você não respondeu.
>
> Isso é exatamente o padrão que a prova da Semana 2 mostrou cinco vezes: a conclusão está
> na direção certa, mas a frase não carrega informação suficiente para outra pessoa
> implementar. Se você entregasse isso como especificação, o dev do outro lado não saberia o
> que fazer.
>
> **A resposta.** Os dois textos têm naturezas diferentes e ciclos de vida diferentes:
>
> | | `ajuda-cnpj` | `erro-cnpj` |
> |---|---|---|
> | O que é | instrução de formato | resultado da validação |
> | Quando existe | sempre | só depois de uma tentativa falha |
> | Muda? | nunca | a cada submissão |
>
> **Apontando só para `erro-cnpj`:** o operador que acabou de errar o formato perde a
> instrução de formato — no exato momento em que ela é mais útil. Ele ouve "CNPJ inválido" e
> nada sobre o que se espera dele.
>
> **Apontando só para `ajuda-cnpj`:** o `aria-invalid` diz "inválido" e o motivo nunca é
> lido. Pior que não ter erro nenhum, porque ele sabe que falhou e não sabe no quê.
>
> **Fundindo os dois num `<span>` só** — que é o atalho que uma IA vai propor pra você — o
> texto de ajuda passa a aparecer e desaparecer junto com o erro, você perde o CSS separado
> (`campo__ajuda` cinza vs `campo__erro` vermelho), e o servidor passa a ter que reconstruir
> a frase inteira em vez de trocar só o pedaço que mudou. **Recuse.**
>
> É esse último ponto que liga na 5.5: o fragmento que o HTMX substitui deve ser o **erro**,
> não o bloco todo. Dois `id` = dois pedaços de ciclo de vida independente.

### 5.5 — a que liga com a Semana 8

Na Semana 8, o HTMX vai disparar uma requisição no `blur` do campo de CNPJ, o servidor valida
o dígito verificador, e devolve **um fragmento de HTML** que substitui só aquele pedaço da
página.

Esse fragmento é gerado pelo servidor, em C#, num Razor Pages.

**Pergunta:** se o `aria-invalid` e o `aria-describedby` estiverem no HTML da página mas
**não** no fragmento que o servidor devolve, o que acontece na prática para o operador que
usa leitor de tela? Ele fica melhor, igual ou pior do que estava antes do HTMX?

**Resposta:**

O operador fica pior.
Se o HTML no servidor não possuir estes atributos, vai sobrescrever o antigo e o campo passará a ser anunciado novamente como "válido e sem erro", escondendo a falha.

> **✅ Correta, e é a melhor resposta do arquivo.** Você pegou o mecanismo exato: o fragmento
> **substitui** o markup, não o complementa, então atributo ausente no fragmento é atributo
> apagado da página. E pegou o "pior": antes do HTMX o campo era silencioso; depois do HTMX
> ele **afirma que está válido** enquanto a tela mostra vermelho. Mentira ativa é pior que
> omissão.
>
> Uma peça a acrescentar, que só existe por causa do HTMX: **a troca acontece sem recarregar
> a página, então o leitor de tela nem sabe que algo mudou.** Foco continua no campo, nada é
> anunciado. Por isso o `<span>` de erro na Semana 8 vai precisar de `role="alert"` (ou
> `aria-live="assertive"`) — é o que faz o leitor interromper e ler o texto novo sem o
> operador ter que sair e voltar no campo. Guarde isto; vai ser cobrado lá.

---

## 5.6 — Item 4 da Semana 1, que era uma pergunta e não uma correção

Do `semana-01/Corrigir.txt`:

> Exercício 4 já estava pronto. CEP `campo--3` + Logradouro `campo--6` + Número `campo--3` =
> 12. Era assim no meu arquivo original. **Ou você conferiu e viu que já somava, ou passou
> batido — me diga qual, porque muda o que eu explico.**

Está aberta desde 02/08. Responda honestamente — "passei batido" é uma resposta útil e
"conferi" sem ter conferido não é.

**Resposta:**

Ao conferir os códigos estava claro que a soma era 12, por experiência eu deduzi que seriam 12 colunas.

> **Aceito, e o "por experiência" é a parte que interessa.** Você não conferiu porque
> alguém mandou; você reconheceu 3+6+3 como um padrão de grid de 12 colunas. Isso é
> transferência de Delphi/layout funcionando, não sorte.
>
> A ressalva: o número 12 não é lei da natureza, é a **escolha do `estilos.css` deste
> protótipo**. Bootstrap usa 12, Foundation usa 12, mas CSS Grid puro usa o que você
> declarar. Numa revisão de código de IA, a pergunta certa não é "soma 12?" — é
> **"soma o que o `grid-template-columns` deste projeto declara?"**. Se o container tiver
> `repeat(16, 1fr)`, um `campo--3 + campo--6 + campo--3` deixa 4 colunas de buraco e o
> layout parece certo até alguém abrir em outra largura.
>
> **Item 4 fechado.**

---

## Veredito — 12/08/2026

| # | Item | Resultado |
|---|---|---|
| 5.1 | o que o leitor anuncia | ⚠️ peças certas, dois termos errados (`*` em vez de `required`; "máscara" em vez de `placeholder`) |
| 5.2 | o que ele não fala | ✅ correta |
| 5.3 | campo corrigido | ✅ correta para o cenário · ressalva sobre `aria-invalid` fixo |
| 5.4 | por que a lista de `id` | ❌ **não respondeu** — repetiu o enunciado |
| 5.5 | fragmento HTMX | ✅ **melhor resposta do arquivo** |
| 5.6 | grid de 12 | ✅ aceito |

**Dívida 5 fechada, com uma condição.** O 5.4 não foi respondido — mas a 5.5, que é a
consequência prática do 5.4, você acertou sozinho e com o mecanismo certo. Isso me diz que
o modelo está lá e a **frase** é que não estava. Que é precisamente o defeito que a Semana 2
diagnosticou cinco vezes.

Então não te reprovo por isso, mas registro: **a partir daqui, resposta que repete o
enunciado conta como não-resposta.** Você vai revisar código de IA — e a IA repete o
enunciado de volta pra você o tempo todo, com confiança. Se você não sente a diferença entre
"explicou" e "reformulou a pergunta", você vai aprovar isso.

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
