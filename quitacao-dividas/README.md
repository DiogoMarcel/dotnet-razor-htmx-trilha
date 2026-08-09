# Bloco de quitação de dívidas — Semanas 1 e 2

Fecha as 5 dívidas abertas antes de começar a Semana 3. Você pediu isso em 07/08/2026, e a
regra que combinamos ficou assim:

> **Dívida de compreensão não passa de bloco.** Fecha antes de avançar.
> **Dívida de experiência é agendada, não adiada** — com bloco e exercício nomeados.

Este bloco fecha as de **compreensão**. As de experiência (`static` vazando com duas
instâncias reais, `DbContext` com escopo errado) ficam agendadas para as Semanas 7 e 10, onde
são demonstráveis.

---

## Ordem

| # | O que | Onde | Formato |
|---|---|---|---|
| 0 | **Escrever as previsões** | [PREVISOES.md](PREVISOES.md) | você escreve |
| 1 | Rodar as 4 demos | `Quitacao.Console` | eu rodo, você compara |
| 2 | Acessibilidade da Semana 1 | [05-semana-01-acessibilidade.md](05-semana-01-acessibilidade.md) | você responde |

**A ordem não é sugestão.** Se você rodar as demos antes de escrever as previsões, o programa
imprime as respostas, você concorda com todas, e o bloco não mede nada. Foi exatamente isso
que a correção da Semana 2 provou: você consolidou os conceitos em que **voltou com o
mecanismo na mão** (Q2, Q5, Q12c), e não consolidou nada na primeira leitura da teoria.

Previsão escrita é o que substitui a mão na massa. O mecanismo do aprendizado não é digitar —
é **ser surpreendido**. Digitar era só o meu jeito antigo de garantir que você não fingisse
entendimento; previsão faz isso melhor e em 5 minutos.

---

## Rodar

```powershell
cd D:\StudieWithAI\quitacao-dividas\Quitacao.Console
dotnet run
```

Uma demo específica:

```powershell
dotnet run -- 3
```

---

## As 5 dívidas

| # | Dívida | Origem | Por que não passa |
|---|---|---|---|
| 1 | **Inversão de controle** | Sem. 2, Q11 | A Semana 3 é LINQ, e LINQ é isto quatro vezes. Sem ele, LINQ vira decoreba de nome de método |
| 2 | **`IReadOnlyList` é subconjunto** | Sem. 2, Q6 | Única que sobreviveu a duas passadas. Governa toda escolha de tipo de retorno |
| 3 | **`static` é um por processo** | Sem. 2, Q8 | Ponte para a Semana 10. Você acertou a consequência, inverteu o mecanismo |
| 4 | **"Quem está segurando?"** | Sem. 2, Q12c | Reaparece na Semana 4 (escopo de DI) e na 7 (`DbContext`) |
| 5 | **Acessibilidade** | **Sem. 1, aberta desde 02/08** | **A única que trava algo.** `aria-invalid`/`aria-describedby` voltam na Semana 8, no fragmento HTMX |

### Sobre a dívida 5, e é a única com surpresa

Fui verificar os arquivos antes de escrever o exercício. As 3 correções de código **estão
aplicadas** no `semana-01/prototipo/`. Mas o `git log` mostra um commit só, e os comentários
dentro do arquivo são meus, com a redação do meu próprio `Corrigir.txt`.

Não consigo distinguir "ele aplicou e entendeu" de "a ferramenta aplicou e ele leu".

O que resolve a dúvida: **`semana-01/Exercícios/cadastro_empresa.html`, o arquivo que você
escreveu, tem zero `aria-invalid` e zero `aria-describedby`.** O protótipo de referência está
certo; o seu arquivo não. O exercício da dívida 5 usa o seu próprio código.

---

## O que este bloco não é

Não é para você escrever código. As 4 demos estão prontas, comentadas, compilam com 0 avisos
e rodam. Seu trabalho é **prever, comparar e explicar** — que é o trabalho que você vai fazer
no escritório dirigindo IA.

Uma coisa que vale dizer com clareza: essa mudança de formato **aumenta** a exigência sobre
precisão de vocabulário, não diminui. Quando você escreve o código, o compilador pega metade
dos seus erros. Quando você revisa código que a IA escreveu, nada pega nada — compila, roda,
e está errado.

Na prova da Semana 2 você chegou na resposta certa com a palavra errada cinco vezes
(`record` "por valor", `IReadOnlyList` "adiciona", CNPJ "sequencial", `Dispose` "limpa
memória", `static` "independe de processos"). Nas cinco, um colega que lesse a sua
especificação implementaria a coisa errada.

Daqui pra frente eu não deixo passar imprecisão de vocabulário só porque a conclusão está
certa.
