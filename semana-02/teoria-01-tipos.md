# Teoria 1 — Tipos, valor vs referência, null

Leitura: ~2h. Digite os exemplos, não só leia.

---

## 1. Delphi → C#: o mapa rápido

| Delphi | C# | Observação |
|---|---|---|
| `Integer` | `int` | 32 bits nos dois |
| `Int64` | `long` | |
| `Double` | `double` | **não use para dinheiro** |
| `Currency` | `decimal` | é o equivalente correto |
| `Boolean` | `bool` | |
| `string` | `string` | em C# é **imutável** |
| `Char` | `char` | aspas simples: `'A'` |
| `array of T` | `T[]` | |
| `TList<T>` | `List<T>` | |
| `record` (antigo) | `struct` | tipo por valor |
| `class` | `class` | por referência nos dois |
| `nil` | `null` | |
| `procedure` | método `void` | |
| `function` | método com retorno | |
| `:=` | `=` | atribuição |
| `=` | `==` | comparação |
| `begin`/`end` | `{ }` | |
| `//` `{ }` | `//` `/* */` | comentário |

Diferença de sintaxe que pega todo mundo no primeiro dia: **o tipo vem antes do nome**.

```pascal
var quantidade: Integer;   // Delphi
```
```csharp
int quantidade;            // C#
```

E C# **diferencia maiúsculas de minúsculas**. `Empresa` e `empresa` são coisas distintas. Delphi não liga; C# liga.

---

## 2. Declarar variáveis

```csharp
int quantidade = 10;
decimal valorUnitario = 25.50m;      // 'm' = decimal. Sem ele, é double.
string razaoSocial = "Padaria do João";
bool ativa = true;

// var: o compilador deduz o tipo pelo lado direito.
// NÃO é tipagem dinâmica — o tipo é fixo em tempo de compilação.
var total = quantidade * valorUnitario;   // total é decimal, decidido na compilação
```

`var` só funciona com inicialização na mesma linha. Use quando o tipo é óbvio pelo lado direito; escreva o tipo quando não for.

Sufixos numéricos que importam:

```csharp
decimal a = 10.5m;    // m ou M — decimal
double  b = 10.5;     // padrão de literal com ponto
float   c = 10.5f;
long    d = 10L;
```

Esquecer o `m` num `decimal` é erro de compilação. Bom — o compilador está te protegendo.

---

## 3. Valor vs referência — a seção que importa

**Tipo por valor** (`int`, `decimal`, `bool`, `DateTime`, `struct`, `enum`): a variável **contém** o dado. Atribuir copia.

**Tipo por referência** (`class`, `string`, arrays, `record class`): a variável contém um **endereço**. Atribuir copia o endereço — as duas apontam para o mesmo objeto.

```csharp
// POR VALOR
int a = 10;
int b = a;      // copiou o valor
b = 20;
// a continua 10

// POR REFERÊNCIA
var empresa1 = new Empresa { RazaoSocial = "Padaria" };
var empresa2 = empresa1;              // copiou o ENDEREÇO
empresa2.RazaoSocial = "Metalúrgica";
// empresa1.RazaoSocial TAMBÉM virou "Metalúrgica" — é o mesmo objeto
```

Em Delphi você já viveu isso: `TObject` é referência, `record` é valor. Mesma ideia, e o mesmo tipo de bug.

Consequência em método:

```csharp
void AplicarDesconto(NotaFiscal nota)   // classe = referência
{
    nota.Valor = nota.Valor * 0.9m;     // altera o objeto do CHAMADOR
}

void Dobrar(int numero)                 // int = valor
{
    numero = numero * 2;                // altera só a cópia local. Inútil.
}
```

Regra prática: se o método recebe uma `class` e mexe nela, o chamador **vê** a alteração. Se recebe um `int`/`decimal`, não vê.

### `string` é caso especial

`string` é tipo por referência, **mas é imutável**. Nenhuma operação altera a string existente — todas criam uma nova.

```csharp
string nome = "padaria";
nome.ToUpper();               // não faz nada de útil: descarta o resultado
nome = nome.ToUpper();        // correto: reatribui
```

Consequência prática: concatenar em laço cria uma string nova a cada volta.

```csharp
// RUIM: 10.000 strings intermediárias jogadas fora
string relatorio = "";
for (int i = 0; i < 10000; i++)
    relatorio = relatorio + linha[i];

// CERTO
var sb = new System.Text.StringBuilder();
for (int i = 0; i < 10000; i++)
    sb.Append(linha[i]);
string relatorio = sb.ToString();
```

Em Delphi você usava `TStringBuilder` pelo mesmo motivo.

---

## 4. `null` e nullable reference types

Em Delphi, qualquer objeto pode ser `nil`, e você descobre o problema em tempo de execução com um Access Violation.

C# moderno tenta impedir isso **em tempo de compilação**. Com *nullable reference types* ligado (padrão em projeto novo do .NET 10):

```csharp
string razaoSocial = null;    // AVISO do compilador: não pode ser null
string? nomeFantasia = null;  // OK — o '?' declara "isto pode ser null"
```

E ao usar:

```csharp
Console.WriteLine(nomeFantasia.Length);    // AVISO: pode ser null aqui

// Formas corretas:
if (nomeFantasia != null)
    Console.WriteLine(nomeFantasia.Length);          // o compilador entende o if

Console.WriteLine(nomeFantasia?.Length);             // ?. => se for null, resultado é null
Console.WriteLine(nomeFantasia?.Length ?? 0);        // ?? => valor padrão quando null
```

| Operador | Nome | O que faz |
|---|---|---|
| `?` no tipo | nullable | declara que aquele tipo aceita null |
| `?.` | acesso condicional | se for null, para e devolve null |
| `??` | coalescência nula | usa o valor da direita quando a esquerda é null |
| `??=` | atribuição nula | só atribui se estiver null |
| `!` | perdão nulo | "eu garanto que não é null" — **evite**, desliga a proteção |

Tipos por valor também aceitam nullable:

```csharp
DateTime? dataCancelamento = null;   // nota não cancelada
decimal? desconto = null;            // sem desconto ≠ desconto de zero
```

Essa distinção importa em sistema fiscal: **`null` significa "não informado"; `0` significa "informado como zero"**. Alíquota zero é diferente de alíquota não preenchida.

---

## 5. `decimal` para dinheiro — não é preferência

```csharp
double a = 0.1 + 0.2;
Console.WriteLine(a);              // 0,30000000000000004

decimal b = 0.1m + 0.2m;
Console.WriteLine(b);              // 0,3
```

`double` é binário: não representa 0,1 exatamente, do mesmo jeito que decimal não representa 1/3. `decimal` é base 10 e guarda 28-29 dígitos significativos.

Num sistema fiscal com milhares de itens, o erro do `double` acumula e o total da NF-e não bate com a soma dos itens. A SEFAZ rejeita. Regra: **dinheiro, alíquota, quantidade fiscal → `decimal`. Sempre.**

`double` serve para cálculo científico e gráfico. Não para você.

### Arredondamento

```csharp
decimal valor = 10.555m;
Math.Round(valor, 2);                                  // 10,56
Math.Round(2.5m, MidpointRounding.AwayFromZero);       // 3
Math.Round(2.5m);                                      // 2  <- padrão é "banker's rounding"
```

O padrão do .NET arredonda 2,5 para **2** (para o par mais próximo). Legislação fiscal normalmente exige "meio para cima". Sempre declare `MidpointRounding.AwayFromZero` quando for valor fiscal.

---

## 6. Conversões

```csharp
// Implícita (sem perda): int -> long -> decimal
int i = 10;
decimal d = i;

// Explícita (com perda): precisa de cast
decimal preco = 10.99m;
int inteiro = (int)preco;      // 10 — TRUNCA, não arredonda

// Texto -> número
int n1 = int.Parse("42");                    // lança exceção se não for número
decimal v1 = decimal.Parse("10,50");         // depende da cultura do sistema!

// A forma segura — não lança exceção:
if (int.TryParse(texto, out int n2))
    Console.WriteLine($"Número válido: {n2}");
else
    Console.WriteLine("Não é número");
```

**`TryParse` é a forma correta ao ler dado externo** (CSV, formulário, arquivo). Dado de fora é sempre suspeito — você aprendeu isso na Semana 1.

### Cultura — a armadilha do CSV

```csharp
using System.Globalization;

// CSV brasileiro usa vírgula decimal
decimal.Parse("1234,56", new CultureInfo("pt-BR"));       // 1234,56

// CSV gerado por sistema em inglês usa ponto
decimal.Parse("1234.56", CultureInfo.InvariantCulture);   // 1234,56
```

Se você não declarar a cultura, o resultado depende da configuração da máquina onde o programa roda — e o servidor de produção normalmente está em inglês. `1234.56` vira `123456`. Já derrubou sistema fiscal de gente grande.

Regra: **em arquivo e integração, sempre `CultureInfo.InvariantCulture` explícito.** Cultura local só para exibir na tela.

---

## 7. Strings

```csharp
string razao = "Padaria do João";

razao.Length                    // 15
razao.ToUpper()                 // "PADARIA DO JOÃO"
razao.Trim()                    // remove espaços das pontas
razao.Contains("Padaria")       // true
razao.StartsWith("Pad")         // true
razao.Replace("João", "Maria")
razao.Substring(0, 7)           // "Padaria"
razao.Split(' ')                // string[] { "Padaria", "do", "João" }
string.IsNullOrWhiteSpace(x)    // null, vazio ou só espaços — use SEMPRE esta
```

**Interpolação** (equivalente ao `Format` do Delphi, mas legível):

```csharp
string nome = "Padaria";
decimal total = 1234.5m;

Console.WriteLine($"Empresa {nome} faturou {total:C}");        // R$ 1.234,50
Console.WriteLine($"Total: {total:N2}");                       // 1.234,50
Console.WriteLine($"Data: {DateTime.Now:dd/MM/yyyy}");
Console.WriteLine($"Percentual: {0.18m:P}");                   // 18,00%
```

O `$` antes das aspas liga a interpolação. `{expressão:formato}`.

Comparação sem diferenciar maiúsculas:

```csharp
if (uf.Equals("sp", StringComparison.OrdinalIgnoreCase))  // correto
if (uf.ToUpper() == "SP")                                  // funciona, mas aloca string à toa
```

---

## Checklist de saída

- [ ] Diferença entre tipo por valor e por referência, e o que muda ao passar para um método
- [ ] Por que `decimal` e não `double` para dinheiro
- [ ] O que `?`, `?.` e `??` fazem
- [ ] Por que `TryParse` em vez de `Parse` ao ler arquivo
- [ ] Por que `CultureInfo.InvariantCulture` em CSV
- [ ] Por que concatenar string em laço é ruim

Próximo: [`teoria-02-oo.md`](teoria-02-oo.md)
