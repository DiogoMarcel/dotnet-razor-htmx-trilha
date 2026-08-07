# Respostas — Semana 2

> ⚠️ **AUTORIA: Claude Code, não Diogo.** Estas respostas foram escritas pela ferramenta
> em 06/08/2026, a pedido explícito dele, depois de eu ter apontado que isso contraria
> o `CLAUDE.md` ("nunca escrever o exercício por ele") e ele ter reafirmado.
>
> **Isto não é avaliação de aprendizado.** Não conta como prova feita. Se a intenção
> era medir o que ele sabe, a prova precisa ser refeita de cabeça, num arquivo separado,
> sem consultar este.

---

## Exercício 4 — perguntas sobre o CSV

### 1. Quantas linhas foram rejeitadas e por qual motivo?

**Seis**, não cinco (o `GUIA-PROJETO.md` diz cinco — está errado; ver "Defeitos no material" no fim).

| Linha | Conteúdo | Motivo | Camada que barrou |
|---|---|---|---|
| 13 | `abc;12345678000199;100.00;01/07/2026` | número inválido `'abc'` | `int.TryParse` |
| 14 | `133;;250.00;02/07/2026` | CNPJ vazio | `IsNullOrWhiteSpace` |
| 15 | `134;12345678000199;abc;03/07/2026` | valor inválido `'abc'` | `decimal.TryParse` |
| 16 | `135;12345678000199;300.00;31/02/2026` | data inválida `'31/02/2026'` | `DateTime.TryParseExact` |
| 17 | `136;12345678000199;-50.00;04/07/2026` | valor negativo | regra explícita `valor < 0` |
| 18 | `137;12345678000199;180.00` | esperava 4 campos, veio 3 | contagem de campos |

17 linhas de dados − 6 rejeitadas = **11 carregadas**. Fecha.

Repare que cinco dos seis motivos são **formato** — o tipo de destino simplesmente não
aceita o texto. Só um (`valor < 0`) é regra que alguém escreveu. Quanto mais o sistema
de tipos barra por você, menos código de validação você mantém.

### 2. A linha `132;11111111111111;500.00;30/07/2026` passou. Deveria? Onde é o lugar certo de barrá-la?

**Sim, deveria passar no leitor.** E deveria ser barrada depois.

`11111111111111` é sintaticamente um CNPJ: 14 dígitos, nada além de dígito. Do ponto de
vista do **formato do arquivo**, a linha está perfeita. O que está errado é o **conteúdo**:
CNPJ com todos os dígitos iguais é inválido por convenção da Receita.

Isso é uma **regra de negócio**, e o lugar dela não é o parser:

- **Não no `LeitorCsv`** — se o dígito verificador morasse lá, trocar o layout do
  arquivo (para posicional, XML, JSON) obrigaria a reescrever regra fiscal. As duas
  coisas mudam por motivos diferentes e em ritmos diferentes; por isso vivem separadas.
- **No domínio**, no construtor de `Empresa` (`Dominio/Empresa.cs`), que já rejeita
  CNPJ inválido e portanto **não consegue existir** com um. É a garantia forte.
- **Num passo intermediário de validação de importação**, entre ler e persistir, para
  o caso em que você quer o relatório do lote inteiro com as linhas ruins marcadas em
  vez de abortadas. É o que o `Program.cs` faz no "segundo passo" do exercício 4.

Os dois últimos não competem: o passo de importação **reporta**, o domínio **impede**.
Relatório de importação que joga a linha fora em silêncio é o pior dos mundos — o
operador fiscal não descobre que faltou nota até o fechamento do mês.

### 3. Por que 31 de fevereiro foi rejeitado sem nenhuma regra escrita sobre isso?

Porque `DateTime` **não tem representação para uma data que não existe**. Internamente é
um contador de ticks desde 01/01/0001 — não é um struct com campos dia/mês/ano
independentes. Não existe o ticks correspondente a 31/02, então
`DateTime.TryParseExact` devolve `false`.

É o sistema de tipos trabalhando de graça. O tipo certo carrega a regra dentro dele.

Compare com o que aconteceria se o DTO guardasse a data como `string`: 31/02 entraria,
seguiria pelo sistema, e explodiria três camadas depois — na gravação no banco, ou no
`ToString("dd/MM/yyyy")` de um relatório, longe da origem. **Converta na fronteira,
para o tipo mais restritivo disponível, e o erro aparece onde ele nasceu.**

Nota lateral: `DateTimeStyles.None` importa aqui. Com `AllowWhiteSpaces` ou alguns
outros estilos o parse fica mais permissivo em espaçamento, mas nunca em data
impossível — isso não tem flag que libere.

---

## Exercício 6 — o que é `(a, b) => b.ValorTotal.CompareTo(a.ValorTotal)`

### Que tipo é

É uma **expressão lambda**, e o tipo dela neste contexto é `Comparison<LinhaRelatorio>` —
um **delegate**. Delegate é um tipo cujo valor é *um método*: uma variável que guarda
código executável, com assinatura verificada em tempo de compilação.

A declaração no .NET é:

```csharp
public delegate int Comparison<in T>(T x, T y);
```

Recebe dois `T`, devolve `int`. A lambda `(a, b) => b.ValorTotal.CompareTo(a.ValorTotal)`
casa com isso: dois parâmetros, corpo que produz `int`. O compilador **infere** que `a`
e `b` são `LinhaRelatorio` a partir da sobrecarga de `Sort` que foi chamada — você não
escreve os tipos, mas eles estão lá, checados.

**Vindo do Delphi:** é o `reference to function(const Left, Right: T): Integer` dos
generics de Delphi, ou o `TFunc<T,T,Integer>`. O conceito é idêntico. A diferença é de
ergonomia — em C# a sintaxe lambda é curta o bastante para você usar todo dia, e é
por isso que o LINQ inteiro pôde ser construído sobre ela.

### Por que um método aceita código como parâmetro

`List<T>.Sort` sabe **ordenar** — quicksort, particionamento, troca de elementos. É
algoritmo genérico, escrito uma vez, testado exaustivamente pela equipe do .NET.

O que ele **não sabe e não tem como saber** é o que "maior" significa para um
`LinhaRelatorio`. Por valor total? Por CNPJ alfabético? Por quantidade de notas?
Decrescente? Só quem chama sabe.

As alternativas seriam piores:

1. **`LinhaRelatorio` implementar `IComparable<T>`** — funciona, mas fixa *uma* ordem
   natural no tipo. Se o relatório precisa ordenar por valor numa tela e por CNPJ em
   outra, você não tem para onde ir.
2. **Escrever seu próprio laço de ordenação** — reimplementar quicksort para cada tipo,
   com os mesmos bugs de índice que todo mundo comete.

Passar o critério como parâmetro separa **o que varia** (a comparação) do **que não
varia** (o algoritmo). É *inversão de controle*: `Sort` chama o **seu** código, não o
contrário.

### Por que `b.CompareTo(a)` e não `a.CompareTo(b)`

`CompareTo` devolve um `int` cujo **sinal** é o que importa: negativo se o primeiro vem
antes, zero se empatam, positivo se vem depois. Trocar a ordem dos operandos inverte o
sinal — e é assim que se obtém decrescente sem escrever `-1 *` nada.

Detalhe que morde: **nunca compare o retorno com `-1` ou `1`**. O contrato garante o
sinal, não a magnitude. `string.CompareTo` devolve a diferença real entre caracteres,
que pode ser `-32`. Escreva `if (x.CompareTo(y) < 0)`, nunca `== -1`.

### Por que isso é o fundamento do LINQ

Todo o LINQ é essa ideia repetida. `Where(x => x.Valor > 100)` recebe um
`Func<T, bool>`. `Select(x => x.Nome)` recebe um `Func<T, TResult>`.
`OrderBy(x => x.Data)` recebe um `Func<T, TKey>`. `Sum(x => x.Total)` recebe outro.

Em todos os casos: um método genérico que implementa a **mecânica** (iterar, filtrar,
projetar, acumular) e recebe o **critério** como delegate. `Sort` com lambda é o mesmo
padrão sem o açúcar sintático de método de extensão em cima.

Na Semana 3 `Relatorio.AgruparPorEmitente` — 20 linhas de `Dictionary` e `foreach` —
vira:

```csharp
notas.GroupBy(n => n.CnpjEmitente)
     .Select(g => new LinhaRelatorio(g.Key, g.Count(), g.Sum(n => n.Valor)))
     .OrderByDescending(l => l.ValorTotal)
     .ToList();
```

Quatro delegates. E `GroupBy` usa um `Dictionary` por baixo — exatamente o que foi
escrito na mão aqui. Não é mágica, é o mesmo código com outra roupa.

---

## Prova de conhecimento — Semana 2

### 1. Uma `Empresa` é passada para um método que altera `RazaoSocial`. O chamador vê a alteração? E se fosse um `decimal`?

**Sim, vê.** Não, com `decimal` não veria.

O mecanismo: `Empresa` é `class` — **tipo por referência**. A variável não contém o
objeto, contém o **endereço** de um objeto no heap. Ao passar para um método, o que é
copiado é o endereço, não o objeto. As duas variáveis — a do chamador e o parâmetro —
apontam para o **mesmo** objeto. Alterar `RazaoSocial` altera aquele objeto único, e o
chamador está olhando para ele.

```csharp
void Renomear(Empresa e) => e.RazaoSocial = "Novo Nome";

var empresa = new Empresa("Antigo", "11222333000181", RegimeTributario.LucroReal);
Renomear(empresa);
Console.WriteLine(empresa.RazaoSocial);   // "Novo Nome"
```

`decimal` é **tipo por valor** (`struct`). O que é copiado é o conteúdo. O método
escreve na cópia dele, o original não muda:

```csharp
void Dobrar(decimal v) => v *= 2;

decimal preco = 100m;
Dobrar(preco);
Console.WriteLine(preco);   // 100 — inalterado
```

**A distinção crítica que quase todo mundo erra:** trocar a *referência* dentro do
método também não afeta o chamador, porque a referência em si foi passada por valor:

```csharp
void Trocar(Empresa e) => e = new Empresa("Outra", "11222333000181", RegimeTributario.LucroReal);

Trocar(empresa);
Console.WriteLine(empresa.RazaoSocial);   // "Antigo" — nada aconteceu
```

Então: **por referência, passada por valor**. Mutar o objeto apontado o chamador vê;
reapontar a variável, não. Para o chamador ver a reatribuição, precisa de `ref`.

**Delphi:** o comportamento de `class` é o mesmo — variável de classe é ponteiro
implícito, e você já vive isso todo dia. O que muda é que em Delphi você gerencia a
vida do objeto (`Free`, `try..finally`) e em C# o GC faz isso. `record` de C# é um
`class` com igualdade por valor gerada — não confunda com o `record` de Delphi, que é
tipo por valor (esse é o `struct` do C#).

### 2. Por que `decimal` e não `double` para valor de nota? Dê um exemplo numérico

`double` é ponto flutuante **binário** (IEEE 754). Ele representa frações como somas de
potências de 2. `0,1` em base 2 é uma dízima periódica infinita — então `double` guarda
uma aproximação, nunca o valor exato. `decimal` é ponto flutuante **decimal**: guarda um
inteiro de 96 bits com um expoente de base 10, e representa qualquer decimal com até 28
dígitos significativos exatamente.

O exemplo canônico:

```csharp
double d = 0.1 + 0.2;
Console.WriteLine(d == 0.3);        // False
Console.WriteLine(d.ToString("R")); // 0.30000000000000004

decimal m = 0.1m + 0.2m;
Console.WriteLine(m == 0.3m);       // True
```

Fiscal, mais concreto — 3 itens de R$ 0,10:

```csharp
double  total = 0;
decimal totalM = 0m;

for (int i = 0; i < 3; i++) { total += 0.1; totalM += 0.1m; }

Console.WriteLine(total == 0.3);    // False
Console.WriteLine(totalM == 0.3m);  // True
```

Onde isso vira problema real:

- **Comparação de fechamento.** `if (totalNota == somaItens)` dá `false` por 1e-17. O
  sistema acusa divergência que não existe.
- **Acúmulo.** 40.000 itens num SPED, cada um com erro de 1e-16, viram centavos. Aí a
  divergência existe e o arquivo é rejeitado.
- **Arredondamento.** `Math.Round` sobre `double` arredonda a aproximação, não o número
  que você quis escrever.

Preço: `decimal` é ~10x mais lento que `double` e ocupa 16 bytes contra 8. Em cálculo
financeiro isso é irrelevante — o gargalo é I/O, não aritmética. Em física, gráficos ou
machine learning, `double` está certo e `decimal` está errado. **A regra não é "decimal
é melhor", é "decimal para dinheiro".**

### 3. Diferença entre `record` e `class`. Por que `Endereco` é record e `Empresa` é class?

`record` **é** uma `class` — não é uma categoria paralela. É uma `class` com membros
gerados pelo compilador:

| | `class` | `record` |
|---|---|---|
| Igualdade | por **referência** (é o mesmo objeto?) | por **valor** (todos os campos batem?) |
| `GetHashCode` | identidade do objeto | derivado dos campos |
| `ToString` | nome do tipo | `Endereco { Logradouro = ..., Uf = SC }` |
| Cópia com alteração | escrita na mão | operador `with` |
| Desconstrução | não | sim, na forma posicional |
| Sintaxe posicional | não | `record X(int A, string B);` |

O critério de escolha **não é** "tem pouco campo" nem "é imutável". É:

> Este conceito tem **identidade própria**, independente dos seus valores?

**`Endereco` é record.** Dois endereços com logradouro, número, município, UF e CEP
iguais **são** o mesmo endereço. Não existe "este endereço" versus "aquele endereço"
com os mesmos dados. Não tem ciclo de vida, não tem chave, não é referenciado por
nada. Igualdade por valor é o comportamento *correto* aqui. E `with` sai de graça:

```csharp
var novoEndereco = endereco with { Numero = "482" };
```

**`Empresa` é class.** Ela tem identidade — o CNPJ. Duas empresas com a mesma razão
social são empresas diferentes se o CNPJ difere; e a mesma empresa continua sendo ela
depois de mudar de endereço e de razão social. Ela **muda no tempo** (é isso que
identidade significa: persistir através da mudança), na Semana 7 vira uma linha no
banco com chave primária, e outras entidades vão apontar para ela.

Se `Empresa` fosse record, `empresaA == empresaB` compararia todos os campos —
inclusive `NomeFantasia` e `Ativa`. Duas leituras da mesma empresa em momentos
diferentes dariam "diferentes". Isso é semanticamente errado e quebra qualquer cache
ou rastreamento por identidade.

**Regra prática:** entidade → `class`. Value object, DTO, resultado de cálculo,
mensagem, evento → `record`. `ItemNota` é o caso de fronteira: é imutável como um
value object, mas está aqui como `class` porque tem validação no construtor e pertence
ao ciclo de vida de uma nota específica. Defensável dos dois jeitos.

### 4. Expressão `switch` que devolve a alíquota conforme UF de origem e destino

```csharp
private static readonly HashSet<string> SulSudeste =
    new(StringComparer.OrdinalIgnoreCase) { "PR", "SC", "RS", "SP", "RJ", "MG", "ES" };

decimal aliquota = (origem, destino) switch
{
    _ when regime == RegimeTributario.SimplesNacional              => 0.00m,
    (var o, var d) when o == d                                     => 0.18m,
    (var o, var d) when SulSudeste.Contains(o)
                     && !SulSudeste.Contains(d)                    => 0.07m,
    _                                                              => 0.12m
};
```

Três coisas que importam nisso:

1. **A ordem é significativa.** O primeiro padrão que casa ganha, e para. Simples
   Nacional vem primeiro porque zera tudo independentemente das UFs. Se viesse por
   último, `SP -> SP` no Simples seria tributado a 18%.
2. **É expressão, não comando.** Ela *devolve* um valor, então o compilador exige que
   todo caminho produza um `decimal`. O `_` final não é decoração — sem ele o
   compilador avisa que o switch pode não casar nada (`CS8509`). Numa escada de `if`
   você esquece um `else` e a variável fica com o default: 0% de imposto, silencioso.
3. **`_ when` é padrão de descarte com guarda** — ignora a tupla e decide por outra
   coisa. `(var o, var d)` é padrão posicional que desconstrói a tupla em duas
   variáveis usáveis na guarda.

### 5. Diferença entre `throw;` e `throw ex;` dentro de um `catch`

`throw;` **repropaga** a exceção preservando o stack trace original.
`throw ex;` **relança** e **reseta** o stack trace para a linha do `throw`.

```csharp
try { MetodoQueFalhaLaEmbaixo(); }
catch (Exception ex)
{
    Log(ex);
    throw;      // stack trace aponta para MetodoQueFalhaLaEmbaixo
    // throw ex; // stack trace aponta para ESTA linha — origem perdida
}
```

`throw ex;` destrói a informação de onde o erro realmente aconteceu. Às 3h da manhã,
com um log dizendo "NullReferenceException na linha 47 do controller" quando o bug está
40 frames abaixo, isso deixa de ser detalhe técnico.

**Use `throw;`** quando você quer observar (logar, incrementar métrica) e deixar passar.

**Use `throw new XException("contexto", ex)`** — exceção nova com a original como
`InnerException` — quando você quer *traduzir* a exceção para o vocabulário da sua
camada. `SqlException` virando `EmpresaNaoEncontradaException` faz sentido; o chamador
não deveria saber que existe SQL embaixo. O `ex` como inner preserva a cadeia inteira.

**Nunca use `throw ex;`.** Não existe caso em que ele seja a escolha certa. Se você
precisa mesmo relançar a mesma instância de outro contexto (fora do `catch` original),
`ExceptionDispatchInfo.Capture(ex).Throw()` faz isso preservando o trace.

### 6. Por que `IReadOnlyList<ItemNota>` em vez de `List<ItemNota>` na propriedade `Itens`?

Porque `List<T>` exposta publicamente entrega **Add e Remove** para qualquer código, e
isso fura toda a validação da entidade:

```csharp
// Se Itens fosse List<ItemNota>:
nota.Autorizar();
nota.Itens.Add(new ItemNota("Contrabando", 1m, 999999m));   // compila, roda, passa
```

`AdicionarItem` checa se a nota ainda está `EmDigitacao`. `Itens.Add` não checa nada. A
regra de negócio existe, mas tem uma porta ao lado dela.

O padrão é: **campo privado mutável, propriedade pública somente-leitura.**

```csharp
private readonly List<ItemNota> _itens = [];
public IReadOnlyList<ItemNota> Itens => _itens;
```

`IReadOnlyList<T>` dá indexação, `Count` e `foreach` — tudo que um leitor precisa — e
não expõe mutação. Quem quiser mexer passa por `AdicionarItem`, que é o único caminho.

Dois limites honestos:

- **Não é imutabilidade real.** É a *interface* que não tem `Add`; o objeto por trás
  continua sendo um `List<T>`. Um cast (`(List<ItemNota>)nota.Itens`) contorna. Isso
  impede acidente, não sabotagem — e é o que se pede de um design, não de um cofre.
  `ImmutableList<T>` ou `.ToArray()` em cada leitura dariam garantia real, com custo
  de alocação.
- **`readonly` no campo protege a referência, não o conteúdo.** `_itens` nunca vai
  apontar para outra lista, mas `_itens.Add` funciona normalmente ali dentro — é para
  isso que ele existe. Confusão frequente de quem chega no C#.

Vale para retorno de método também: devolver `IReadOnlyList<T>` em vez de `List<T>`
comunica "isto é resultado, não é para você mexer".

### 7. Por que ler CSV com `TryParse` e `CultureInfo.InvariantCulture` em vez de `Parse` direto?

São dois problemas separados.

**`TryParse` em vez de `Parse` — porque linha ruim é rotina, não excepcional.**

`Parse` lança `FormatException` quando o texto não converte. Num arquivo de importação
de 10.000 linhas com 50 ruins, isso significa 50 exceções, ou uma que aborta as 9.950
boas. Nenhum dos dois é o que o operador fiscal quer: ele quer as 9.950 processadas e
um relatório das 50.

`TryParse` devolve `bool` e preenche o `out`. O erro vira **fluxo de controle normal**,
que é o que ele é. Bônus: lançar e capturar exceção é ordens de magnitude mais caro que
devolver um `bool` — em volume, a diferença é medível em minutos.

Princípio: **exceção é para o que você não esperava.** Dado externo malformado você
espera, por definição.

**`InvariantCulture` — porque o formato do arquivo não é o formato do usuário.**

`decimal.Parse("1234.56")` numa máquina com locale pt-BR interpreta o ponto como
**separador de milhar** e devolve `123456`. Fator 100, sem exceção, sem aviso. O número
entra errado no banco e ninguém descobre até a conferência.

`InvariantCulture` fixa o comportamento: ponto é decimal, vírgula é milhar, sempre,
em qualquer máquina. O arquivo tem *um* formato definido pelo emissor; ele não muda
porque o servidor foi reinstalado com outro locale.

Mesma coisa em `DateTime.TryParseExact` com `"dd/MM/yyyy"`: `"01/02/2026"` é 1º de
fevereiro em pt-BR e 2 de janeiro em en-US. Sem formato explícito, o bug só aparece
depois do dia 12 do mês — quando `13/01` deixa de ser interpretável como mês 13 e o
parse muda de comportamento.

**A regra completa:** `InvariantCulture` para **ler e gravar dado de máquina**
(arquivos, APIs, banco, log). Cultura do usuário para **exibir**. O `Program.cs` deste
projeto faz as duas coisas de propósito: lê com `InvariantCulture`, imprime com `pt-BR`.

### 8. O que é um campo `static` e por que ele é perigoso num servidor web?

Campo `static` pertence ao **tipo**, não à instância. Existe uma cópia só, criada no
primeiro acesso ao tipo, viva até o processo morrer. Não importa se você tem zero ou
um milhão de instâncias da classe — o campo static é um.

É o `class var` do Delphi, ou uma variável global de unit com escopo de classe.

**Por que é perigoso na web:** um servidor ASP.NET Core atende **muitas requisições
simultâneas no mesmo processo**, em threads diferentes. Todas veem o mesmo campo
static. Duas consequências:

**1. Corrida de dados (data race).**

```csharp
public static class ContextoAtual
{
    public static Empresa? EmpresaLogada { get; set; }   // BOMBA
}
```

Requisição A grava a empresa X. Requisição B, 2ms depois, grava Y. Requisição A lê e
enxerga **Y**. O usuário da empresa X vê as notas da empresa Y. Isso é vazamento de
dados entre clientes — em sistema fiscal, incidente reportável.

Pior: não reproduz em desenvolvimento, porque lá tem um usuário só.

**2. Estado que nunca é liberado.** Static enraíza o objeto: o GC nunca coleta o que
um campo static alcança. Um `static Dictionary` usado como cache sem política de
expiração cresce até o processo cair. Vazamento de memória clássico em .NET.

**O que é seguro:**

- `static readonly` de valor **imutável** — os arrays de pesos do `ValidadorCnpj`, o
  `HashSet` de UFs da `CalculadoraIcms`. Escritos uma vez na inicialização, só lidos
  depois. Zero risco. (Cuidado: `static readonly List<T>` **não** é imutável —
  `readonly` protege a referência, e `Add` continua funcionando. Use
  `FrozenSet`/`ImmutableArray` quando a garantia importa.)
- **Métodos** static que são funções puras — `ValidadorCnpj.EhValido`,
  `CalculadoraIcms.Calcular`. Sem estado, sem problema.
- `const`.

**O que fazer em vez de static mutável:** injeção de dependência com tempo de vida
declarado (`AddScoped` = uma instância por requisição, `AddSingleton` = uma por
aplicação — e singleton mutável tem exatamente os mesmos problemas do static, com a
vantagem de ser explícito e testável). Semana 4.

**O que muda em relação ao Delphi:** num executável desktop, uma variável global é
"o estado do usuário" e funciona, porque tem um usuário e uma thread principal. Na web
o mesmo código é "o estado de todos os usuários misturado". É a manifestação mais
direta do ponto que atravessa a trilha inteira: **não existe estado entre requisições,
e tudo que parece estado global é armadilha.**

---

## Defeitos encontrados no material da semana

Achados rodando o código. Precisam de correção no `GUIA-PROJETO.md` / `notas.csv`:

### 1. `GUIA-PROJETO.md`, exercício 4 — a conta não fecha

O guia diz:

```text
✓ 11 notas carregadas
✗ 5 linhas rejeitadas:
```

São **6** rejeitadas. 11 + 5 = 16, e o arquivo tem 17 linhas de dados. As seis estão
tabeladas acima.

### 2. `GUIA-PROJETO.md`, exercício 2, passo 2 — afirmação tecnicamente falsa

O guia diz:

> Rejeite os 14 dígitos iguais (`11111111111111` é matematicamente válido pelo cálculo,
> mas é inválido por convenção — está na sua massa de teste).

**`11111111111111` não é matematicamente válido.** O DV calculado é `80`, não `11`.

Conferido para os dez casos de dígito repetido:

| CNPJ | DV esperado | DV calculado | Fecha? |
|---|---|---|---|
| `00000000000000` | `00` | `00` | **sim** |
| `11111111111111` | `11` | `80` | não |
| `22222222222222` | `22` | `59` | não |
| `33333333333333` | `33` | `28` | não |
| `44444444444444` | `44` | `05` | não |
| `55555555555555` | `55` | `76` | não |
| `66666666666666` | `66` | `45` | não |
| `77777777777777` | `77` | `14` | não |
| `88888888888888` | `88` | `93` | não |
| `99999999999999` | `99` | `62` | não |

O único que fecha é `00000000000000` — soma zero, resto zero, DV `00`.

**De onde vem o folclore:** do **CPF**, onde `11111111111` realmente fecha (DV calculado
`11`, confere). Alguém transportou a afirmação de CPF para CNPJ sem recalcular. Os
algoritmos são parecidos mas os pesos são outros — CPF usa `10..2` e `11..2`, CNPJ usa
`5,4,3,2,9,8,7,6,5,4,3,2`.

**A regra continua necessária**, só a justificativa está errada. Sem ela,
`00000000000000` entra no banco. E ela também serve de defesa contra dado de teste
vazando para produção — `11111111111111` é o que o operador digita quando quer "só
passar da tela".

**Correção sugerida no guia:** trocar o parênteses por
"(`00000000000000` fecha no cálculo; os outros nove não, mas todos são inválidos por
convenção da Receita)". E acrescentar `00000000000000` à massa de teste — é o caso que
de fato exercita a regra.

### 3. `notas.csv` — todos os CNPJs da massa reprovam no dígito verificador

Nenhum dos cinco CNPJs do arquivo fecha no DV:

| CNPJ no arquivo | DV esperado | DV calculado |
|---|---|---|
| `12345678000199` | `99` | `95` |
| `98765432000110` | `10` | `98` |
| `45612378000105` | `05` | `84` |
| `33221144000177` | `77` | `27` |
| `11111111111111` | `11` | `80` |

Isso cria um conflito real entre os exercícios: o 2 manda ligar `ValidadorCnpj` no
construtor de `Empresa`, e o 5 manda montar o relatório a partir do CSV. Se o
relatório construísse `Empresa`, **as 11 notas válidas estourariam**.

Aqui isso não acontece porque `Relatorio` agrupa os DTOs `NotaFiscalCsv`, não entidades
— que é a modelagem certa por outros motivos. Mas o aluno que ligar as duas pontas de
forma ingênua trava, e vai achar que o validador dele está errado.

**Correção sugerida:** trocar os CNPJs do `notas.csv` por valores que fechem no DV,
mantendo `11111111111111` (que é dado de teste proposital e bom). Aí o exercício 4
passa a ter um passo de validação com resultado misto — 10 boas, 1 barrada — que é bem
mais didático do que "todas erradas".

### 4. `GUIA-PROJETO.md`, exercício 5 — os números do exemplo estão errados

O exemplo mostra:

```text
12.345.678/0001-99       5      5.561,40         1.112,28
```

O real é **4 notas / 5.311,40 / 1.327,85**. A diferença de R$ 250,00 é a linha 14
(`133;;250.00;02/07/2026`), que tem CNPJ vazio e é **rejeitada pelo leitor** — não pode
entrar em nenhum grupo.

A outra linha do exemplo (`98.765.432/0001-10  3  61.631,55  20.543,85`) está correta.

Saída real, conferida:

```text
CNPJ                   Notas     Valor total    Ticket médio
------------------------------------------------------------
98.765.432/0001-10 *       3       61.631,55       20.543,85
12.345.678/0001-99 *       4        5.311,40        1.327,85
45.612.378/0001-05 *       2        4.055,75        2.027,88
11.111.111/1111-11 *       1          500,00          500,00
33.221.144/0001-77 *       1          420,00          420,00
------------------------------------------------------------
TOTAL                     11       71.918,70        6.538,06
```
