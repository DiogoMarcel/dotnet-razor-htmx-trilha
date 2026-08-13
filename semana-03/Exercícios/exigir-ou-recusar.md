# Exercício — o que exigir e o que recusar

Este é o pilar novo, definido em 07/08. É o que mais se parece com o seu trabalho no
escritório e não existia na trilha antes.

**Formato:** duas rodadas. A ordem importa e não é sugestão.

- **Rodada 1** — você recebe só o requisito fiscal. Escreve o que vai **exigir** da IA
  **antes** de ver uma linha de código.
- **Rodada 2** — você recebe o código que a IA entregou. Marca o que **recusa**.

Fazer a rodada 1 depois de ler o código não mede nada: você vai listar exatamente os
defeitos que acabou de ver. A rodada 1 mede se você sabe o que pedir **sem** a pista.

---

## Rodada 1 — antes de ver o código

### O requisito, como ele chegaria de um contador

> *"Preciso de uma tela que mostre, para um mês escolhido, o total de ICMS por
> estabelecimento. Só nota autorizada entra. Quero ver quantas notas, o total da base, o
> ICMS, e a maior nota do estabelecimento. Ordenado do maior ICMS para o menor. Os dados vêm
> de um serviço externo do escritório, uma consulta por estabelecimento, e são uns 40
> estabelecimentos."*

Você vai pedir isso a uma IA. **Escreva agora, antes de ver qualquer código:**

### 1.1 — As decisões técnicas que você IMPÕE no pedido

Não "boas práticas em geral". Decisões concretas que, se a IA escolher sozinha, ela tem
chance real de escolher errado — e que você só descobre depois em produção.

Mínimo de 6. Para cada uma, **uma frase dizendo o que quebra se ela for ignorada.**

> 1.
> 2.
> 3.
> 4.
> 5.
> 6.

### 1.2 — As perguntas que você faz ao CONTADOR antes de pedir à IA

O requisito acima tem pelo menos três ambiguidades que nenhuma IA vai resolver — porque a
resposta não está no código, está na legislação ou na prática do escritório.

> 1.
> 2.
> 3.

### 1.3 — O que você aceita que a IA decida sozinha

Também é parte do trabalho: exigir tudo é tão ruim quanto não exigir nada, porque ninguém
revisa 40 exigências. Liste 3 decisões que você **entrega** a ela sem revisar.

> 1.
> 2.
> 3.

---

## Rodada 2 — o código que veio

**Só leia depois de responder a rodada 1.**

A IA devolveu isto. Compila, roda, e a tela mostra números.

```csharp
public class ApuracaoService
{
    private readonly IConsultaExterna _consulta;

    public ApuracaoService(IConsultaExterna consulta) => _consulta = consulta;

    public async Task<List<LinhaApuracao>> ApurarAsync(int ano, int mes)
    {
        var todas = new List<NotaDto>();

        foreach (var cnpj in await _consulta.ListarEstabelecimentosAsync())
        {
            var notas = await _consulta.ObterNotasAsync(cnpj, ano, mes);
            todas.AddRange(notas);
        }

        var validas = todas.Where(n => n.Situacao == "Autorizada");

        if (validas.Count() == 0)
            return new List<LinhaApuracao>();

        var linhas = validas
            .GroupBy(n => n.RazaoSocial)
            .Select(g => new LinhaApuracao
            {
                Estabelecimento = g.Key,
                Quantidade      = g.Count(),
                Base            = g.Sum(n => n.Valor),
                Icms            = Math.Round(g.Sum(n => n.Valor) * 0.18m, 2),
                MaiorNota       = g.Max(n => n.Valor),
            })
            .OrderByDescending(l => l.Icms)
            .ToList();

        _ = GravarLogDeApuracaoAsync(ano, mes, linhas.Count);

        return linhas;
    }

    private async void GravarLogDeApuracaoAsync(int ano, int mes, int quantidade)
    {
        await _consulta.GravarLogAsync($"apuracao {mes:D2}/{ano}: {quantidade} linhas");
    }

    public decimal TotalGeral(int ano, int mes)
    {
        var linhas = ApurarAsync(ano, mes).Result;
        return linhas.Sum(l => l.Icms);
    }
}
```

### 2.1 — O que você RECUSA

Liste cada defeito. Para cada um: **o que está errado · por que · o que exigir no lugar.**

Não pare no primeiro que achar. Há mais de seis, e eles são de três famílias diferentes
(LINQ, async, domínio fiscal).

> 1.
> 2.
> 3.
> 4.
> 5.
> 6.
> ...

### 2.2 — Qual é o defeito mais CARO

Dos que você achou, qual custa mais dinheiro ou mais credibilidade — e por quê? Não é
necessariamente o mais grave tecnicamente.

> resposta:

### 2.3 — Qual defeito NÃO aparece em desenvolvimento

Um deles funciona perfeitamente na sua máquina, com um usuário, e só quebra com carga ou
com dado real. Qual, e o que exatamente muda?

> resposta:

### 2.4 — O que está certo neste código

Também é revisão. Se você só sabe apontar erro, seus pedidos viram lista de proibições e a
IA piora. Cite duas escolhas corretas.

> 1.
> 2.

### 2.5 — A comparação que fecha o exercício

Volte na sua rodada 1. **Quantos dos defeitos que você listou em 2.1 estavam previstos em
1.1?** Escreva o número, honestamente.

> previstos: ___ de ___

Esse número é o exercício inteiro. Achar defeito olhando código é revisão. Saber o que
exigir **antes** é o que faz a IA não escrever o defeito.
