# Projeto da Semana 2 — FiscalLab console

Console app puro. Sem web, sem banco, sem LINQ. Só C# e regra de negócio.

## 1. Criar o projeto

```powershell
cd D:\StudieWithAI\semana-02\projeto
dotnet new console -n FiscalLab.Console
cd FiscalLab.Console
dotnet run
```

Deve imprimir `Hello, World!`. Se imprimiu, o SDK está funcionando.

Abra a pasta no VS Code (`code .`). Estrutura final que você vai montar:

```text
FiscalLab.Console/
├── Program.cs
├── FiscalLab.Console.csproj
├── Dominio/
│   ├── Empresa.cs
│   ├── ItemNota.cs
│   ├── NotaFiscal.cs
│   ├── Endereco.cs
│   └── Enums.cs                 <- exceção: enum não ganha arquivo próprio
├── Servicos/
│   ├── ValidadorCnpj.cs         <- SEU
│   ├── CalculadoraIcms.cs       <- SEU
│   ├── ResultadoIcms.cs         <- SEU (record de retorno)
│   ├── LeitorCsv.cs
│   ├── ResultadoLinha.cs        <- saiu de LeitorCsv.cs ao separar
│   ├── NotaFiscalCsv.cs         <- idem
│   ├── Relatorio.cs             <- SEU
│   └── LinhaRelatorio.cs        <- SEU (record de linha do relatório)
└── dados/
    └── notas.csv
```

**Regra:** um tipo público por arquivo. Os exemplos em [exemplos/](exemplos/) estão num arquivo só para você ler de uma vez — ao copiar, **separe**.

Para o CSV ser copiado junto do executável, acrescente ao `.csproj`:

```xml
<ItemGroup>
  <None Update="dados\notas.csv">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

Comando útil: `dotnet watch run` recompila e reexecuta a cada save — o `dotnet` equivalente ao Live Server.

## 2. Material que eu escrevi

| Arquivo | O que é |
|---|---|
| [exemplos/Dominio.cs](exemplos/Dominio.cs) | `Empresa`, `ItemNota`, `NotaFiscal`, `Endereco`, enums — comentado linha a linha |
| [exemplos/LeitorCsv.cs](exemplos/LeitorCsv.cs) | leitura de CSV com tratamento de erro, cultura e formato de data |
| [dados/notas.csv](dados/notas.csv) | 17 linhas, **com sujeira de propósito** |

Leia os comentários antes de copiar. Se copiar sem entender, você repete o `httpbin`.

---

## Exercícios

### 1. Montar o domínio (1h)

Separe o `Dominio.cs` em arquivos, um tipo por arquivo, e faça compilar. No `Program.cs`, crie uma empresa, uma nota com 3 itens, autorize e imprima.

Depois **quebre de propósito** e observe as mensagens:

- criar `ItemNota` com quantidade `0`
- chamar `AdicionarItem` numa nota já autorizada
- `Cancelar` uma nota que está `EmDigitacao`

Objeto que se recusa a entrar em estado inválido é o objetivo. Veja acontecendo.

### 2. Validador de CNPJ (1h30) — o principal da semana

`Servicos/ValidadorCnpj.cs`, classe `static`:

```csharp
public static bool EhValido(string cnpj)
public static string Formatar(string cnpj)      // 14 dígitos -> 00.000.000/0000-00
```

Algoritmo do dígito verificador — implemente **na mão**, sem copiar pronto:

1. Limpe tudo que não é dígito. Precisa sobrar exatamente **14**.
2. Rejeite os 14 dígitos iguais. Todos são inválidos por convenção da Receita. Dos dez, só `00000000000000` fecha no cálculo do DV (soma zero, resto zero) — `11111111111111` **não** fecha, o DV calculado dele é `80`. Se você já ouviu que "todos iguais passa na conta", isso é verdade para **CPF** (`11111111111` fecha), não para CNPJ: os pesos são outros. A regra existe mesmo assim, por convenção e para barrar `00000000000000`.
3. **Primeiro dígito:** multiplique os 12 primeiros dígitos pelos pesos `5,4,3,2,9,8,7,6,5,4,3,2`, na ordem. Some tudo. Calcule `resto = soma % 11`. Se `resto < 2`, o dígito é `0`; senão é `11 - resto`.
4. **Segundo dígito:** mesma coisa com os **13** primeiros dígitos (já incluindo o primeiro DV) e os pesos `6,5,4,3,2,9,8,7,6,5,4,3,2`.
5. Compare os dois dígitos calculados com os dois últimos do CNPJ.

Teste com:

| CNPJ | Esperado |
|---|---|
| `11222333000181` | válido |
| `11.222.333/0001-81` | válido (com pontuação) |
| `11222333000180` | inválido (DV errado) |
| `11111111111111` | inválido (todos iguais — e o DV também não fecha) |
| `00000000000000` | inválido (todos iguais — este **fecha** no DV; só a regra o barra) |
| `112223330001` | inválido (12 dígitos) |
| `""` e `null` | inválido, sem estourar exceção |

Depois ligue no construtor de `Empresa`, substituindo o `TODO`.

> Dica: os dois dígitos usam o mesmo cálculo com pesos diferentes. Se você escrever duas vezes o mesmo laço, refatore para um método privado `CalcularDigito(string base, int[] pesos)`. Duplicação aqui é sinal de que faltou abstrair.

### 3. Calculadora de ICMS (1h)

`Servicos/CalculadoraIcms.cs`. Regras **simplificadas** (não é a legislação real):

| Situação | Alíquota |
|---|---|
| origem = destino | 18% |
| Sul/Sudeste → Norte/Nordeste/Centro-Oeste | 7% |
| demais operações interestaduais | 12% |
| emitente é Simples Nacional | 0% (recolhe pelo DAS) |

Assinatura sugerida:

```csharp
public record ResultadoIcms(decimal BaseCalculo, decimal Aliquota, decimal Valor);

public static ResultadoIcms Calcular(ItemNota item, string ufOrigem, string ufDestino,
                                     RegimeTributario regime);
```

Requisitos:

- Use **expressão `switch`** com `when`, não uma escada de `if` (Teoria 3, item 2)
- `Math.Round(valor, 2, MidpointRounding.AwayFromZero)` — nunca o arredondamento padrão
- `record` para o resultado

Sul/Sudeste: PR, SC, RS, SP, RJ, MG, ES.

> **Correção registrada em 07/08/2026 — o aluno apontou e estava certo.** Na legislação
> real, **ES é destino privilegiado**: a alíquota de 7% vale para Sul/Sudeste destinadas ao
> Norte, Nordeste, Centro-Oeste **e ao Espírito Santo** (Resolução do Senado nº 22/1989,
> art. 1º, parágrafo único). Ou seja, `SP → ES` é 7%, não 12% como esta regra simplificada
> devolve. ES aparece nos **dois** lados: é Sudeste como origem, e privilegiado como destino.
>
> A regra acima fica simplificada de propósito — o objetivo do exercício é expressão
> `switch` e arredondamento, não tributar. Mas saiba que é simplificação, e não descubra
> isso numa autuação. A alíquota interna também varia por estado (SP 18%, RJ 20% com o
> FECP, MG 18%), o que esta tabela achata em 18% para todos.

### 4. Carregar o CSV (1h)

Copie o `LeitorCsv.cs` e rode sobre [dados/notas.csv](dados/notas.csv). O arquivo tem **17 linhas e sujeira plantada**.

Imprima:

```text
✓ 11 notas carregadas
✗ 6 linhas rejeitadas:
  Linha 13: número inválido 'abc'
  ...
```

11 + 6 = 17. Se a sua conta não fechar com o total de linhas de dados do arquivo, tem
linha sendo engolida em silêncio — e é justamente isso que um leitor de importação não
pode fazer.

Depois responda:

1. Quantas linhas foram rejeitadas e por qual motivo cada uma?
2. A linha `132;11111111111111;500.00;30/07/2026` **passou** no leitor. Deveria? Onde é o lugar certo de barrá-la?
3. A linha `135;...;300.00;31/02/2026` — por que 31 de fevereiro foi rejeitado sem você escrever nenhuma regra sobre isso?

### 5. Relatório por emitente (1h)

`Servicos/Relatorio.cs`. Agrupe as notas válidas por CNPJ e imprima:

```text
CNPJ                 Notas    Valor total    Ticket médio
12.345.678/0001-99       4      5.311,40         1.327,85
98.765.432/0001-10       3     61.631,55        20.543,85
```

> **Aviso sobre a massa de teste:** os CNPJs do `notas.csv` são fictícios e **nenhum
> deles fecha no dígito verificador**. Não é bug do seu validador. Consequência prática:
> se o seu relatório construir `Empresa` para cada linha, as 11 notas estouram — porque
> o construtor agora valida o CNPJ (exercício 2). Agrupe pelos DTOs `NotaFiscalCsv`, não
> pelas entidades. Isso não é contorno: DTO na fronteira e entidade no núcleo é a
> modelagem certa, e é o mesmo padrão que volta na Semana 5 com o binding do Razor Pages.

Regras:

- **`Dictionary<string, ...>`** para agrupar. Percorrer a lista inteira para cada CNPJ é O(n²) — não faça.
- **Sem LINQ.** `foreach` na mão. Na Semana 3 você reescreve isso com `GroupBy` e vai medir a diferença de linhas.
- Alinhamento de colunas com formatação (`{valor,15:N2}` alinha à direita em 15 caracteres).
- Total geral no rodapé.

### 6. Ordenar sem LINQ (30 min)

Ordene o relatório por valor total decrescente usando `List<T>.Sort` com uma comparação:

```csharp
lista.Sort((a, b) => b.ValorTotal.CompareTo(a.ValorTotal));
```

Explique por escrito o que é aquele `(a, b) => ...` — que tipo é, e por que um método aceita código como parâmetro. Isso é **delegate**, e é o fundamento do LINQ inteiro. Se entender aqui, a Semana 3 é fácil.

---

## Prova de conhecimento — Semana 2

**A prova mora em [`../Exercícios/prova-semana-02.md`](../Exercícios/prova-semana-02.md).**
São **12 questões** — as 8 originais mais 4 acrescentadas em 06/08/2026. Responda lá
mesmo, no arquivo, embaixo de cada pergunta.

> Antes esta seção repetia as 8 questões aqui. Duas fontes para a mesma prova fizeram
> exatamente o que era previsível: a lista daqui ficou desatualizada e as 4 novas passaram
> em branco. Fonte única agora.

Sem consultar nada — nem a teoria, nem o código, nem o
`gabarito-semana-02-CLAUDE.md`.

E a tarefa da trilha, do [01-trilha-12-semanas.md](../../01-trilha-12-semanas.md):

> Console app: leitor de CSV de notas fiscais que carrega em memória, valida CNPJ (dígito verificador na mão) e imprime totais por emitente. **Sem LINQ.**

Exercícios 2, 4 e 5 juntos já são isso.

---

## Ao terminar

Me traga:

1. O código (aponte a pasta)
2. As respostas da prova
3. A dúvida específica que sobrou

Reviso e libero a Semana 3 — LINQ e `async`/`await`, onde você reescreve metade deste projeto em um terço das linhas.
