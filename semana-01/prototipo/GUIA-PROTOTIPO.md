# Guia do protótipo — Semana 1

## O que tem aqui

| Arquivo | O que é |
|---|---|
| `index.html` | Listagem de empresas: menu lateral, cabeçalho, filtro, tabela |
| `cadastro.html` | Formulário de cadastro de empresa |
| `css/estilos.css` | Todo o estilo, comentado bloco a bloco |

Escrevi este código **comentado como material de estudo**, não como código de produção. Leia os comentários — eles são metade do conteúdo.

## Como abrir

Duplo clique no `index.html`. É HTML estático, não precisa de servidor.

Melhor: instale a extensão **Live Server** no VS Code, clique com o botão direito no `index.html` → *Open with Live Server*. Ele recarrega sozinho a cada save.

---

## Roteiro de exploração (faça na ordem, ~1h)

### 1. Ver o formulário virar requisição HTTP

1. Abra `cadastro.html`
2. F12 → aba **Network** → marque **Preserve log**
3. Preencha alguns campos e clique em **Salvar empresa**
4. Vai dar erro (404 ou "arquivo não encontrado) — **é o esperado**, não existe servidor
5. Clique na linha da requisição no Network e responda:
   - Qual o **método**? Qual o **caminho**?
   - Aba **Payload** (ou *Request*): quais campos foram enviados? Em que formato?
   - O campo `nomeFantasia`, deixado em branco, foi enviado? E o `id` hidden?
   - Desmarque **Empresa ativa** e envie de novo. O campo `ativa` aparece no payload? **Por que não?**
   - Qual o `Content-Type` da requisição?

### 2. Comparar com GET

1. Abra `index.html`, digite algo na busca, clique em **Filtrar**
2. Olhe a **URL** na barra de endereços. Onde foram parar os dados?
3. Por que o filtro usa GET e o cadastro usa POST? (dica: Teoria 1, item 4)

### 3. Furar a validação do cliente

1. Em `cadastro.html`, deixe **Razão social** vazia e tente enviar. O navegador barra (`required`).
2. F12 → **Elements** → ache o `<input id="razaoSocial">` → duplo clique → apague o `required` → Enter
3. Envie de novo. **Passou.**
4. Escreva com suas palavras por que "validação no cliente é UX, validação no servidor é segurança".

> Depois adicione `novalidate` na tag `<form>` e envie o formulário vazio. O navegador para de reclamar — um atributo só desliga toda a validação de cliente. Se sua única defesa estivesse ali, o banco já teria recebido lixo.

### 4. Ver o DOM (não o arquivo)

1. Em `index.html`, Ctrl+U (código-fonte) e F12 → Elements. Compare: iguais, porque não há JS.
2. Ainda no Elements, apague uma `<tr>` da tabela (Delete). Sumiu da tela.
3. F5. Voltou. **Você alterou o DOM, não o arquivo.** Guarde isso — é exatamente o que o HTMX faz na Semana 8, só que com HTML vindo do servidor.

### 5. Brincar com o CSS ao vivo

No painel **Styles** (Elements):

- `.pagina` → mude `grid-template-columns` de `240px 1fr` para `1fr 1fr`. Veja o menu comer metade da tela.
- `.cabecalho` → troque `justify-content: space-between` por `center`, depois `flex-end`.
- `.menu` → troque `flex-direction: column` por `row`. Note que `justify-content`/`align-items` **trocaram de eixo**.
- Desligue `box-sizing: border-box` no bloco do reset. Observe o layout inchar.
- Estreite a janela abaixo de 768px e veja o `@media` reorganizar o grid.

---

## Exercícios (aqui você escreve código)

Faça **sem copiar** dos meus arquivos. Consultar o CSS depois de tentar, pode.

1. **Item ativo do menu** — Crie `notas.html` (pode ser uma cópia enxuta do `index.html`) e faça o item "Notas fiscais" ficar ativo, com o item "Empresas" inativo. Reaproveite as classes existentes.

2. **Coluna nova** — Acrescente uma coluna "Regime tributário" à tabela do `index.html`, com os valores certos para as 4 empresas. Sem quebrar o alinhamento.

3. **Total no rodapé da tabela** — Use `<tfoot>` para somar a coluna "Notas no mês", alinhado à direita e em negrito. Estilize com um seletor novo no CSS (`.tabela tfoot td { … }`).

4. **Campos lado a lado** — No `cadastro.html`, faça CEP, Logradouro e Número caberem em **uma única linha** do grid de 12 colunas. Você escolhe a divisão — só precisa somar 12.

5. **Estado de erro** — Mostre a mensagem de erro no campo CNPJ: adicione `<span class="campo__erro">CNPJ inválido.</span>` e crie uma classe `.campo__entrada--erro` que deixe a borda vermelha. Aplique-a no input.

6. **Página de detalhe (mais difícil)** — Crie `detalhe.html`: dados da empresa em duas colunas (rótulo à esquerda, valor à direita) usando Grid, mais uma tabela com as últimas 5 notas fiscais dela. Sem inventar classe nova sem necessidade — reaproveite `.cartao`, `.tabela`, `.etiqueta`.

---

## Prova de conhecimento da Semana 1

Do `01-trilha-12-semanas.md`. Faça **sem consultar nada**:

1. Explique por escrito por que um servidor web não sabe que duas requisições vieram do mesmo usuário — e três formas de resolver isso, com o trade-off de cada uma.
2. Monte **do zero, em arquivo novo**, uma página com formulário de cadastro de empresa (razão social, CNPJ, UF, e-mail) estilizada com **Flexbox** (não Grid — é o exercício), sem framework CSS. Abra o DevTools e mostre a requisição gerada pelo submit.

Quando terminar, me chame com o código e eu reviso: aponto o que está errado **e** o que está apenas medíocre.
