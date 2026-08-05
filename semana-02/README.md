# Semana 2 — C# fundamentos (parte 1)

**Objetivo:** sair do zero em C#. Aqui você tem vantagem real: Anders Hejlsberg projetou o Delphi **e** o C#. Muita coisa vai parecer familiar. O perigo é justamente esse — as diferenças sutis são onde você vai errar.

## Antes de tudo: instalar o SDK

Sua máquina tem só o *runtime* do .NET (roda aplicação pronta, não compila). Precisa do **SDK**.

```powershell
winget install Microsoft.DotNet.SDK.10
```

Feche e reabra o terminal, depois confira:

```powershell
dotnet --version      # deve mostrar 10.x
dotnet --list-sdks
```

Se `winget` não estiver disponível, baixe em <https://dotnet.microsoft.com/download/dotnet/10.0> (opção **SDK**, x64).

No VS Code, instale a extensão **C# Dev Kit** (Microsoft). Ela traz IntelliSense, depurador e o executor de testes.

## Ordem de estudo

| # | Arquivo | Tempo | O que é |
|---|---|---|---|
| 1 | [teoria-01-tipos.md](teoria-01-tipos.md) | 2h | Tipos, valor vs referência, `null`, strings, conversões |
| 2 | [teoria-02-oo.md](teoria-02-oo.md) | 2h30 | Classes, propriedades, `record`, interfaces, herança, construtores |
| 3 | [teoria-03-colecoes.md](teoria-03-colecoes.md) | 2h | Fluxo, `switch`, pattern matching, coleções, exceções |
| 4 | [projeto/GUIA-PROJETO.md](projeto/GUIA-PROJETO.md) | 5h | **Você escrevendo código** — FiscalLab console |
| 5 | Prova de conhecimento | 1h | No fim do guia do projeto |

Total ~12h30.

## Recursos externos

- [Microsoft Learn — C# for beginners](https://learn.microsoft.com/pt-br/training/paths/get-started-c-sharp-part-1/) — sandbox no navegador, PT-BR disponível
- [Balta.io — Fundamentos do C#](https://balta.io/cursos/fundamentos-csharp) — PT-BR, gratuito no plano free
- [Referência da linguagem C#](https://learn.microsoft.com/pt-br/dotnet/csharp/) — consulta

## As 4 ideias que precisam ficar

1. **Valor vs referência.** `struct`/`int`/`decimal` copiam; `class` compartilham. Passar uma classe para um método e alterá-la lá dentro altera o original. É a fonte nº 1 de bug de quem está começando.
2. **`null` é opcional agora.** `string` não aceita null; `string?` aceita. O compilador avisa. Ligue isso e obedeça.
3. **`decimal` para dinheiro. Sempre.** `double` erra centavos. Em sistema fiscal isso é auditoria.
4. **`record` para dado, `class` para comportamento.** Saber escolher é metade do design.

## O que NÃO entra esta semana

**LINQ e `async`/`await`** ficam para a Semana 3, de propósito. Você vai escrever laços `for`/`foreach` na mão e sentir a dor — depois LINQ vira alívio, não sintaxe decorada.

## Ao terminar

Traga o código do console app e as respostas da prova. Reviso e libero a Semana 3.
