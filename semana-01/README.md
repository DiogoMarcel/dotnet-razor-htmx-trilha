# Semana 1 — Como a web funciona

**Objetivo:** você vem de desktop. Sem este módulo, todo o resto vira decoreba — e HTMX (Semana 8) fica literalmente incompreensível.

## Ordem de estudo

| # | Arquivo | Tempo | O que é |
|---|---|---|---|
| 1 | [teoria-01-http.md](teoria-01-http.md) | 2h | Cliente/servidor, HTTP, métodos, status, **stateless**, cookies, DevTools |
| 2 | [teoria-02-html.md](teoria-02-html.md) | 1h30 | HTML semântico, formulários, `name` vs `id`, tabelas, DOM |
| 3 | [teoria-03-css.md](teoria-03-css.md) | 1h30 | Box model, seletores, especificidade, Flexbox, Grid |
| 4 | Flexbox Froggy + Grid Garden | 2h | Jogos. Valem mais que qualquer vídeo sobre o assunto |
| 5 | [prototipo/GUIA-PROTOTIPO.md](prototipo/GUIA-PROTOTIPO.md) | 1h | Explorar o código que escrevi, no DevTools |
| 6 | Exercícios do guia | 3h | **Você escrevendo código** |
| 7 | Prova de conhecimento | 1h | Sem consultar nada |

Total ~12h.

## Links externos da semana

- [MDN — Primeiros passos na web](https://developer.mozilla.org/pt-BR/docs/Learn/Getting_started_with_the_web) (PT-BR)
- [MDN — Visão geral do HTTP](https://developer.mozilla.org/pt-BR/docs/Web/HTTP/Overview) (PT-BR)
- [Flexbox Froggy](https://flexboxfroggy.com/#pt-br) · [Grid Garden](https://cssgridgarden.com/#pt-br)
- [MDN — Aprender CSS: layout](https://developer.mozilla.org/pt-BR/docs/Learn/CSS/CSS_layout)

## As 3 ideias que precisam ficar

Se você esquecer tudo o resto, guarde estas:

1. **HTTP não tem memória.** Cada requisição chega ao servidor como a primeira da vida. Estado é sempre *carregado junto* (cookie, sessão, hidden field) ou está *no banco*.
2. **`name` é o que vai para o servidor.** `id` só existe dentro da página. Na Semana 5 o model binding do ASP.NET Core usa o `name` para preencher sua classe C#.
3. **Validação de cliente é conveniência; validação de servidor é a real.** Sem exceção — e menos ainda em cálculo de imposto.

## Ambiente

Nesta semana você não precisa de nada além do navegador e do VS Code. Duas extensões que valem instalar agora:

- **Live Server** (Ritwick Dey) — recarrega a página a cada save
- **Portuguese (Brazil) Language Pack** — opcional

O .NET SDK só entra na Semana 4 — na sua máquina hoje há só o runtime, não o SDK. Docker 29.4.1 já está instalado (usado a partir da Semana 7).

## Ao terminar

Me chame com:

1. O código dos exercícios e da prova de conhecimento
2. A dúvida específica que sobrou
3. O que você tentou antes de perguntar

Eu reviso, aponto o que está errado **e** o que está apenas medíocre, e libero a Semana 2 (C# fundamentos).
