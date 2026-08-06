# Teoria 2 — Classes, records, interfaces

Leitura: ~2h30.

---

## 1. Classe

```csharp
public class Empresa
{
    // Propriedade: parece campo, mas é um par get/set.
    // Em Delphi você escrevia FRazaoSocial + property RazaoSocial read/write.
    // Em C# o compilador gera o campo escondido para você.
    public string RazaoSocial { get; set; } = string.Empty;
    public string Cnpj { get; set; } = string.Empty;
    public string? NomeFantasia { get; set; }      // pode não existir
    public bool Ativa { get; set; } = true;

    // Método
    public string ObterIdentificacao()
    {
        return $"{RazaoSocial} ({Cnpj})";
    }
}
```

Uso:

```csharp
var empresa = new Empresa
{
    RazaoSocial = "Padaria do João",   // sintaxe de inicialização de objeto
    Cnpj = "12345678000199"
};

Console.WriteLine(empresa.ObterIdentificacao());
```

Sem `Create`, sem `Free`. O **garbage collector** libera quando ninguém mais referencia o objeto. Vindo de Delphi, esse é um alívio e um risco: você para de pensar em tempo de vida, e depois se surpreende com `DbContext` vivendo demais (Semana 4).

### Modificadores de acesso

| Modificador | Quem enxerga |
|---|---|
| `public` | todo mundo |
| `private` | só a própria classe (**padrão** se você omitir) |
| `protected` | a classe e suas herdeiras |
| `internal` | só o mesmo projeto (assembly) |

Regra: comece tudo `private` e abra o mínimo necessário.

---

## 2. Propriedades — as variações

```csharp
public class NotaFiscal
{
    // 1. Automática, leitura e escrita
    public int Numero { get; set; }

    // 2. Só leitura de fora, escrita só dentro da classe
    public decimal Total { get; private set; }

    // 3. init: só pode ser atribuída na criação. Depois é imutável.
    public DateTime DataEmissao { get; init; }

    // 4. Somente leitura, calculada. Não guarda nada.
    public bool Cancelada => DataCancelamento != null;

    // 5. Com campo de apoio e lógica no set
    private decimal _desconto;
    public decimal Desconto
    {
        get => _desconto;
        set
        {
            if (value < 0)
                throw new ArgumentException("Desconto não pode ser negativo");
            _desconto = value;
        }
    }

    public DateTime? DataCancelamento { get; set; }
}
```

O `=>` na nº 4 é **corpo de expressão**: forma curta de escrever um método/propriedade de uma linha só. Equivale a `get { return DataCancelamento != null; }`.

`init` (nº 3) é a ferramenta certa para dado que não pode mudar depois de criado — número de nota, data de emissão, CNPJ do emitente.

---

## 3. Construtores

```csharp
public class ItemNota
{
    public string Descricao { get; }
    public decimal Quantidade { get; }
    public decimal ValorUnitario { get; }

    // Construtor tradicional
    public ItemNota(string descricao, decimal quantidade, decimal valorUnitario)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descrição obrigatória", nameof(descricao));
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade deve ser positiva", nameof(quantidade));

        Descricao = descricao;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
    }

    public decimal Total => Quantidade * ValorUnitario;
}
```

**Construtor primário** (forma curta, C# 12+):

```csharp
public class CalculadoraIcms(decimal aliquotaPadrao)
{
    public decimal Calcular(decimal baseCalculo) => baseCalculo * aliquotaPadrao;
}
```

O parâmetro fica disponível no corpo inteiro da classe. É a forma que você vai ver em todo lugar a partir da Semana 4, quando as dependências chegarem pelo construtor:

```csharp
public class EmpresaService(ICnpjService cnpjService, ILogger<EmpresaService> logger)
{
    public void Fazer() => logger.LogInformation("...");
}
```

Guarde esse formato. É injeção de dependência, o assunto central da Semana 4.

### Validar no construtor

Objeto que nasce inválido é veneno: o erro aparece longe de onde foi causado. Valide no construtor e o objeto **não consegue existir** em estado inválido. Isso vale mais em domínio fiscal do que em qualquer outro.

---

## 4. `record` — a novidade que muda o design

`record` é uma classe otimizada para **representar dados**. O compilador gera de graça:

- construtor
- propriedades `init` (imutáveis)
- comparação por **valor**, não por referência
- `ToString()` legível
- `with` para copiar alterando um campo

```csharp
// Forma posicional — uma linha
public record Endereco(string Logradouro, string Numero, string Municipio, string Uf);

var e1 = new Endereco("Rua das Flores", "128", "São Paulo", "SP");
var e2 = new Endereco("Rua das Flores", "128", "São Paulo", "SP");

Console.WriteLine(e1 == e2);        // True  <- compara CONTEÚDO
Console.WriteLine(e1);              // Endereco { Logradouro = Rua das Flores, ... }

// Cópia com uma alteração — o original não muda
var e3 = e1 with { Numero = "130" };
```

Compare com `class`:

```csharp
var c1 = new EmpresaClasse { Cnpj = "123" };
var c2 = new EmpresaClasse { Cnpj = "123" };
Console.WriteLine(c1 == c2);        // False <- compara ENDEREÇO
```

### Quando usar cada um

| Use | Quando |
|---|---|
| `record` | o objeto **é** um dado: endereço, alíquota, período, resultado de cálculo, DTO |
| `class` | o objeto **faz** coisas ou muda ao longo do tempo: serviço, entidade que vive no banco |

Para o FiscalLab: `Endereco` e `ResultadoIcms` são `record`. `Empresa` e `NotaFiscal` são `class` — mudam, têm identidade própria e vão para o banco na Semana 7.

Existe também `record struct` (record por valor). Não precisa agora.

---

## 5. Interfaces

Contrato: **o que** um tipo faz, sem dizer **como**.

```csharp
public interface ICalculadoraImposto
{
    decimal Calcular(ItemNota item, string ufOrigem, string ufDestino);
    string Nome { get; }
}

public class CalculadoraIcms : ICalculadoraImposto
{
    public string Nome => "ICMS";

    public decimal Calcular(ItemNota item, string ufOrigem, string ufDestino)
    {
        decimal aliquota = ufOrigem == ufDestino ? 0.18m : 0.12m;
        return item.Total * aliquota;
    }
}

public class CalculadoraIpi : ICalculadoraImposto
{
    public string Nome => "IPI";
    public decimal Calcular(ItemNota item, string o, string d) => item.Total * 0.05m;
}
```

Uso — o código depende do contrato, não da implementação:

```csharp
var calculadoras = new List<ICalculadoraImposto>
{
    new CalculadoraIcms(),
    new CalculadoraIpi()
};

foreach (var calc in calculadoras)
    Console.WriteLine($"{calc.Nome}: {calc.Calcular(item, "SP", "MG"):C}");
```

Acrescentar PIS/COFINS = criar mais uma classe. Nada do código existente muda.

Convenção: nome de interface começa com **`I`** — igual ao Delphi.

**Por que isso importa já:** a partir da Semana 4, o ASP.NET Core injeta dependências **por interface**. Você registra `ICnpjService` e recebe a implementação pronta. E em teste (Semana 11), troca a implementação real por uma falsa sem tocar no código testado. Interface não é enfeite acadêmico — é o que torna o resto possível.

### 5.1 Vindo do Delphi: o que muda (e por que você pode usar à vontade)

O nome é igual, a palavra-chave é igual, a convenção `I` é igual. O que está por baixo, não. Três coisas que você aprendeu a temer no Delphi **não existem em C#**:

| | Delphi | C# |
|---|---|---|
| Tempo de vida | descende de `IInterface` → `_AddRef`/`_Release`. Misturar `Free` com referência de interface = double free ou vazamento | GC. Referência de interface é uma referência comum. Nada a gerenciar |
| Ancestral | toda interface herda de `IInterface` | interface não herda de nada |
| GUID | obrigatório para `Supports` / `as` | não existe. `is` e `as` funcionam direto. GUID só em interop COM |
| Custo por objeto | **um ponteiro escondido por instância, por interface implementada** (4 bytes x86 / 8 x64) | zero. O mapa de interfaces vive no tipo, não na instância. O objeto tem o mesmo header implementando 0 ou 40 interfaces |
| Limite de empacotamento | tabela de export do formato PE: **65.535 símbolos por BPL/DLL**. Package grande estoura e precisa ser quebrada | não se aplica. Tipos são resolvidos por metadados, não pela export table do PE |

Aquele limite de 65.535 é do Windows, não do Delphi — e é a origem do conselho "não abusa de interface". Ele é real, mas o gatilho é o **tamanho da package**, não a interface em si. Em .NET o mecanismo é outro: o assembly carrega uma tabela de metadados com teto na casa dos milhões de linhas. Não é um limite que você alcança escrevendo código.

Sobra só o custo de despacho: chamar método por interface é um pouco mais caro que chamada virtual (o runtime usa um stub com cache). Ordem de nanossegundo. Num app web, a consulta ao banco custa dez mil vezes mais. Não é critério de decisão.

**Consequência:** em C#, o único critério para criar uma interface é **design** — acoplamento e testabilidade. Nunca orçamento de recurso. Se você se pegar contando interfaces, está resolvendo um problema que a plataforma não tem.

### 5.2 Assembly não é BPL

Os dois são `.dll`. O comportamento é bem diferente, e isso muda como você organiza o projeto.

| | BPL (Delphi) | Assembly (.NET) |
|---|---|---|
| Compilador | package do Delphi 11 não carrega no 12 | o contrato é o IL. Biblioteca compilada com C# 10 roda em app C# 14 sem recompilar |
| Memória | compartilha RTL e memory manager — é por isso que passar `string` ou `TObject` por DLL comum quebra, e por isso a package existe | um GC, um heap. Passar `List<Empresa>` entre assemblies é trivial |
| Opcional? | escolha de build (*runtime packages* liga/desliga) | não é opcional. Todo projeto .NET **é** um assembly |
| Resolução | PATH e diretório do executável | `deps.json` + NuGet. Duas versões da mesma biblioteca podem coexistir no processo em `AssemblyLoadContext` diferentes — impossível com BPL |
| Fronteira de visibilidade | o que a package exporta | o modificador `internal`, verificado pelo **compilador** |

Na Semana 4 o FiscalLab vira três assemblies: `FiscalLab.Domain`, `FiscalLab.Data`, `FiscalLab.Web`. `Domain` não referencia `Web` — e o compilador impede. É a mesma disciplina de dependência entre packages, sem a fragilidade de versão e sem a export table.

---

## 6. Herança

```csharp
public abstract class DocumentoFiscal        // abstract = não pode ser instanciada
{
    public int Numero { get; init; }
    public DateTime DataEmissao { get; init; }

    public abstract string Tipo { get; }              // filha É OBRIGADA a implementar
    public virtual string Descrever()                 // filha PODE substituir
        => $"{Tipo} nº {Numero} de {DataEmissao:dd/MM/yyyy}";
}

public class NotaFiscalEletronica : DocumentoFiscal
{
    public override string Tipo => "NF-e";
    public string ChaveAcesso { get; init; } = string.Empty;

    public override string Descrever()
        => base.Descrever() + $" — chave {ChaveAcesso}";      // base = herdado
}
```

| C# | Delphi |
|---|---|
| `abstract` | `abstract` |
| `virtual` | `virtual` |
| `override` | `override` |
| `sealed` | `sealed` |
| `base.Metodo()` | `inherited` |

Diferença: em C#, `override` é **obrigatório** ao substituir. Em Delphi, esquecer `override` compila e cria um método novo por acidente — bug clássico. C# não deixa.

Duas regras herdadas de anos de projeto ruim:

- **C# tem herança simples.** Uma classe base só. Interfaces, quantas quiser.
- **Prefira composição a herança.** Herança acopla forte. Antes de criar uma hierarquia, pergunte se não resolve com interface + uma classe que contém a outra. Hierarquia de 4 níveis é sinal de problema, não de sofisticação.

---

## 7. `static`

```csharp
public static class ValidadorCnpj      // classe estática: não se instancia
{
    public static bool EhValido(string cnpj)
    {
        // ...
        return true;
    }
}

// Chama pelo nome da classe, sem new:
if (ValidadorCnpj.EhValido("12345678000199")) { }
```

Serve para função pura — entra dado, sai resultado, sem estado guardado.

**Perigo:** campo `static` é **compartilhado por toda a aplicação**. Num programa de console, tudo bem. Num servidor web com dezenas de requisições simultâneas, é estado global compartilhado entre usuários — corrupção de dado e vazamento entre sessões. Foi por isso que eu disse na Semana 1 que "guardar estado num campo `static`" é a pior saída possível.

Regra: `static` só para função sem estado.

---

## 8. Organização de arquivos

```csharp
namespace FiscalLab.Domain;      // ponto e vírgula: vale para o arquivo inteiro

public class Empresa { }
```

- **Um tipo público por arquivo**, com o nome do arquivo igual ao do tipo (`Empresa.cs`)
- Namespace acompanha as pastas: `FiscalLab.Domain.Fiscal` → `Domain/Fiscal/`
- `using FiscalLab.Domain;` no topo de quem consome

Em .NET moderno, `ImplicitUsings` já traz `System`, `System.Collections.Generic` e outros automaticamente. Por isso você não vê `using System;` em projeto novo.

---

## Checklist de saída

- [ ] Diferença entre `class` e `record`, e quando usar cada um
- [ ] O que `init` faz e por que é útil em dado fiscal
- [ ] Para que serve uma interface, com um exemplo do seu domínio
- [ ] Por que os limites de interface que valem no Delphi não valem em C#
- [ ] Uma diferença prática entre assembly .NET e BPL do Delphi
- [ ] Diferença entre `abstract` e `virtual`
- [ ] Por que campo `static` é perigoso em servidor web
- [ ] Por que validar no construtor

Próximo: [`teoria-03-colecoes.md`](teoria-03-colecoes.md)
