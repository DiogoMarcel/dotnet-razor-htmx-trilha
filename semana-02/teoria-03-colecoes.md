# Teoria 3 — Fluxo, pattern matching, coleções e exceções

Leitura: ~2h.

---

## 1. Controle de fluxo

```csharp
if (valor > 1000)
    Console.WriteLine("alto");
else if (valor > 100)
    Console.WriteLine("médio");
else
    Console.WriteLine("baixo");

// Operador ternário: condição ? seValor : senaoValor
string faixa = valor > 1000 ? "alto" : "baixo";
```

Laços:

```csharp
for (int i = 0; i < 10; i++) { }

foreach (var item in nota.Itens) { }          // o que você mais vai usar

while (linha != null) { }

do { } while (tentativas < 3);
```

`break` sai do laço, `continue` pula para a próxima volta. Igual ao Delphi.

**Não existe `for i := 10 downto 1`.** Escreva `for (int i = 10; i >= 1; i--)`.

---

## 2. `switch` e pattern matching

O `switch` clássico existe, mas o que você vai usar é a **expressão `switch`**:

```csharp
// Delphi: case regime of 1: ... 2: ... end;
string descricao = regimeTributario switch
{
    1 => "Simples Nacional",
    2 => "Lucro Presumido",
    3 => "Lucro Real",
    _ => "Desconhecido"          // _ é o "else". OBRIGATÓRIO cobrir todos os casos.
};
```

Diferenças do Delphi: é uma **expressão** (produz valor, dá para atribuir), não precisa de `break`, e o compilador exige que todos os casos estejam cobertos.

Com condições:

```csharp
string faixa = valorTotal switch
{
    < 0        => "inválido",
    0          => "zerado",
    < 1000     => "pequeno",
    < 100000   => "médio",
    _          => "grande"
};
```

Com múltiplos valores e vários critérios:

```csharp
decimal aliquota = (ufOrigem, ufDestino) switch
{
    ("SP", "SP")            => 0.18m,
    ("SP", "MG") or ("SP", "RJ") => 0.12m,
    (_, _) when ufOrigem == ufDestino => 0.17m,   // when = condição extra
    _                       => 0.07m
};
```

Isso substitui um `if/else if` de 20 linhas por uma tabela legível. Em regra fiscal — que é essencialmente uma tabela de casos — é a ferramenta certa.

Verificação de tipo:

```csharp
if (documento is NotaFiscalEletronica nfe)      // testa E já cria a variável tipada
    Console.WriteLine(nfe.ChaveAcesso);

if (valor is null) { }
if (valor is not null) { }
if (idade is >= 18 and < 65) { }
```

---

## 3. Coleções

### Array — tamanho fixo

```csharp
string[] ufs = new string[3];
ufs[0] = "SP";

string[] ufs2 = { "SP", "MG", "PR" };
Console.WriteLine(ufs2.Length);       // 3 — Length, sem parênteses
```

Índice começa em **0**. Delphi permitia começar em 1; C# não.

### `List<T>` — o que você vai usar sempre

```csharp
var empresas = new List<Empresa>();

empresas.Add(empresa);
empresas.Insert(0, outra);
empresas.Remove(empresa);            // remove pelo objeto
empresas.RemoveAt(0);                // remove pelo índice
empresas.Contains(empresa);
empresas.Clear();

Console.WriteLine(empresas.Count);   // Count, não Length
Console.WriteLine(empresas[0].RazaoSocial);

foreach (var e in empresas)
    Console.WriteLine(e.RazaoSocial);
```

**Armadilha:** não altere a lista dentro de um `foreach` sobre ela. Lança `InvalidOperationException`. Para remover durante a iteração, use `for` de trás para frente:

```csharp
for (int i = empresas.Count - 1; i >= 0; i--)
    if (!empresas[i].Ativa)
        empresas.RemoveAt(i);
```

### `Dictionary<K,V>` — busca por chave

```csharp
var empresasPorCnpj = new Dictionary<string, Empresa>();

empresasPorCnpj["12345678000199"] = empresa;      // adiciona ou substitui
empresasPorCnpj.Add("987...", outra);             // ERRO se a chave já existe

// Leitura segura:
if (empresasPorCnpj.TryGetValue(cnpj, out var achada))
    Console.WriteLine(achada.RazaoSocial);

foreach (var par in empresasPorCnpj)
    Console.WriteLine($"{par.Key} => {par.Value.RazaoSocial}");
```

Por que importa: buscar num `List` de 10.000 empresas percorre até 10.000 itens. No `Dictionary` é praticamente instantâneo, independente do tamanho. Para totalizar por emitente — exatamente o exercício desta semana — `Dictionary` é a estrutura certa.

### `IEnumerable<T>` — a abstração

```csharp
// Aceita List, array, Dictionary, resultado de LINQ... qualquer coisa iterável
public decimal Somar(IEnumerable<ItemNota> itens)
{
    decimal total = 0;
    foreach (var item in itens)
        total += item.Total;
    return total;
}
```

Regra de assinatura: **receba `IEnumerable<T>`, devolva `List<T>`**. Aceite o mais genérico possível na entrada, entregue o mais concreto na saída. Vale para o resto da sua carreira em .NET.

---

## 4. Exceções

```csharp
try
{
    var linhas = File.ReadAllLines("notas.csv");
}
catch (FileNotFoundException ex)          // mais específica primeiro
{
    Console.WriteLine($"Arquivo não encontrado: {ex.FileName}");
}
catch (IOException ex)                    // mais genérica depois
{
    Console.WriteLine($"Erro de leitura: {ex.Message}");
}
finally
{
    // roda sempre, com ou sem exceção
}
```

Ordem importa: do mais específico para o mais genérico. O compilador reclama se você inverter.

Lançar:

```csharp
if (string.IsNullOrWhiteSpace(cnpj))
    throw new ArgumentException("CNPJ obrigatório", nameof(cnpj));

if (nota.Cancelada)
    throw new InvalidOperationException("Nota cancelada não pode ser alterada");
```

`nameof(cnpj)` devolve a string `"cnpj"`. Se você renomear o parâmetro, a mensagem acompanha — não vira mentira silenciosa.

### Repropagar sem destruir a pilha

```csharp
catch (Exception ex)
{
    logger.LogError(ex, "Falha ao processar nota {Numero}", nota.Numero);
    throw;          // CERTO: preserva a pilha original
    // throw ex;    // ERRADO: reinicia a pilha, você perde onde o erro nasceu
}
```

Diferença de uma letra, e é a diferença entre depurar em 5 minutos ou em 3 horas.

### Quando NÃO usar exceção

Exceção é para o **excepcional**. Dado inválido vindo do usuário não é excepcional — é rotina.

```csharp
// RUIM: exceção como fluxo normal. Lento e ilegível.
try { var cnpj = int.Parse(entrada); }
catch { Console.WriteLine("inválido"); }

// BOM
if (!int.TryParse(entrada, out var cnpj))
    Console.WriteLine("inválido");
```

Na Semana 5 isso reaparece: validação de formulário usa `ModelState`, não exceção.

---

## 5. `using` e `IDisposable`

Recursos externos (arquivo, conexão, socket) precisam ser liberados. Em Delphi era `try/finally` com `Free`. Em C#:

```csharp
// using declaration: libera ao sair do escopo
using var leitor = new StreamReader("notas.csv");
string? linha;
while ((linha = leitor.ReadLine()) != null)
    Console.WriteLine(linha);
// leitor.Dispose() chamado automaticamente aqui, mesmo se der exceção
```

Equivale a `try/finally { leitor.Dispose(); }`.

Objetos comuns (`Empresa`, `List`) **não** precisam de `using` — o garbage collector cuida. Só o que segura recurso do sistema operacional.

---

## 6. Métodos — detalhes que aparecem

```csharp
// Parâmetro opcional
public decimal Calcular(decimal valor, decimal aliquota = 0.18m) => valor * aliquota;

Calcular(100);              // usa 0.18
Calcular(100, 0.12m);
Calcular(aliquota: 0.12m, valor: 100);     // argumento nomeado — legível

// out: devolve valor adicional
public bool TentarValidar(string cnpj, out string erro)
{
    if (cnpj.Length != 14) { erro = "Deve ter 14 dígitos"; return false; }
    erro = string.Empty;
    return true;
}

if (!TentarValidar(entrada, out string mensagem))
    Console.WriteLine(mensagem);

// Sobrecarga: mesmo nome, assinaturas diferentes
public decimal Calcular(ItemNota item) => Calcular(item.Total);
```

Argumento nomeado vale ouro em chamada com vários `bool`: `Processar(nota, validar: true, salvar: false)` é legível; `Processar(nota, true, false)` não.

---

## Checklist de saída

- [ ] Escrever uma expressão `switch` com `when`
- [ ] Diferença entre `List` e `Dictionary`, e quando cada uma
- [ ] Por que não alterar uma lista dentro do `foreach` dela
- [ ] Diferença entre `throw;` e `throw ex;`
- [ ] Por que `TryParse` em vez de `try/catch` para dado do usuário
- [ ] Quando usar `using`

Próximo: [`projeto/GUIA-PROJETO.md`](projeto/GUIA-PROJETO.md) — agora é código.
