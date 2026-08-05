# Teoria 1 — Cliente/servidor, HTTP e o problema do estado

Leitura: ~2h, com pausas para testar no navegador. Não pule os "**Faça agora**".

---

## 1. O que muda vindo do Delphi

No Delphi você escreve um `.exe`. Ele roda na máquina do usuário, tem memória própria, e essa memória **persiste** enquanto o programa está aberto.

Na web existem **dois programas em máquinas diferentes**:

- **Cliente** — o navegador, na máquina do usuário
- **Servidor** — a sua aplicação ASP.NET Core, num datacenter

Eles não compartilham memória. Só trocam **mensagens de texto** pela rede. Toda a web é isso: mensagens de texto indo e voltando.

> Analogia: em Delphi você conversa com alguém na mesma sala. Na web você troca cartas com alguém em outro país — e a pessoa do outro lado **queima a carta e esquece você** assim que responde.

---

## 2. O caminho de uma requisição

Você digita `https://fiscallab.com.br/empresas` e aperta Enter. O que acontece:

1. **DNS** — o navegador pergunta a um servidor de nomes: "qual o IP de `fiscallab.com.br`?" Resposta: `203.0.113.42`. DNS é a agenda de contatos da internet: nome → número.
2. **TCP** — o navegador abre uma conexão com `203.0.113.42` na porta 443. TCP garante que os bytes cheguem inteiros e na ordem. É o "canal".
3. **TLS** — cliente e servidor negociam criptografia (é o "S" do HTTPS). A partir daqui ninguém no meio do caminho lê o conteúdo. É por isso que sistema fiscal **sempre** usa HTTPS.
4. **HTTP** — só agora vai a mensagem de fato: "me dê o recurso `/empresas`".
5. O servidor responde com HTML.
6. O navegador **interpreta** o HTML, descobre que precisa de CSS e imagens, e faz **novas requisições** para cada um.

Ponto que quase todo iniciante erra: uma página não é uma requisição. É **dezenas**. Uma para o HTML, uma para cada CSS, cada imagem, cada script.

**Faça agora:** abra qualquer site, F12 → aba **Network** → F5. Conte as linhas. Cada linha é uma requisição.

---

## 3. Anatomia de uma requisição HTTP

HTTP é texto puro. Isso é literalmente o que trafega:

```http
POST /empresas/criar HTTP/1.1
Host: fiscallab.com.br
Content-Type: application/x-www-form-urlencoded
Cookie: sessao=a7f3c9e1
Content-Length: 62

razaoSocial=Padaria+do+Jo%C3%A3o&cnpj=12345678000199&uf=SP
```

Quatro partes:

| Parte | No exemplo | O que é |
|---|---|---|
| **Método** | `POST` | o verbo — o que você quer fazer |
| **Caminho** | `/empresas/criar` | qual recurso |
| **Headers** | `Host`, `Cookie`, `Content-Type` | metadados: quem sou, o que aceito, formato do corpo |
| **Corpo** | `razaoSocial=...` | os dados (só em POST/PUT/PATCH) |

A resposta tem estrutura igual, trocando método/caminho por um **status**:

```http
HTTP/1.1 302 Found
Location: /empresas
Set-Cookie: sessao=a7f3c9e1; HttpOnly; Secure; SameSite=Lax

```

---

## 4. Métodos (verbos)

| Método | Uso | Tem corpo? | Seguro¹ | Idempotente² |
|---|---|---|---|---|
| `GET` | buscar/exibir | não | sim | sim |
| `POST` | criar / executar ação | sim | não | **não** |
| `PUT` | substituir inteiro | sim | não | sim |
| `PATCH` | alterar parte | sim | não | não |
| `DELETE` | remover | raramente | não | sim |

¹ **Seguro** = não altera nada no servidor.
² **Idempotente** = chamar 10 vezes tem o mesmo efeito que chamar 1 vez.

**Por que isso importa na prática:** `POST` não é idempotente. Se o usuário aperta F5 depois de enviar o formulário de emissão de NF-e, o navegador reenvia o POST — e você emite a nota **duas vezes**. A solução é o padrão **PRG** (Post/Redirect/Get), que você vai implementar na Semana 5. Guarde o termo.

Regra prática: **GET nunca altera dados**. Se você fizer um link `GET /empresas/excluir/5`, o robô de indexação do Google — ou o pré-carregador do navegador — vai apagar seu banco. Isso já aconteceu com gente grande.

---

## 5. Status codes

Decore as faixas, não os números:

| Faixa | Significado | Exemplos que você vai usar |
|---|---|---|
| **2xx** | deu certo | `200 OK`, `201 Created`, `204 No Content` |
| **3xx** | vá para outro lugar | `302 Found` (redirect do PRG), `304 Not Modified` (cache) |
| **4xx** | **você**, cliente, errou | `400 Bad Request`, `401` (não autenticado), `403` (autenticado mas sem permissão), `404 Not Found`, `422` (validação falhou) |
| **5xx** | **eu**, servidor, errei | `500 Internal Server Error`, `503 Service Unavailable` |

A diferença entre `401` e `403` cai em entrevista: **401 = não sei quem você é** (vá fazer login). **403 = sei quem você é e você não pode** (perfil Consulta tentando emitir nota).

---

## 6. O núcleo: HTTP é *stateless*

**Esta é a seção mais importante das 12 semanas.** Leia duas vezes.

O protocolo HTTP não tem memória. Cada requisição chega ao servidor como se fosse a primeira da vida. O servidor **não sabe**:

- quem é você
- que você fez login há 2 minutos
- o que você digitou na tela anterior

Não é limitação de implementação. É **design deliberado**, e é o que permite colocar 3 réplicas da aplicação atrás de um balanceador: qualquer réplica atende qualquer requisição, porque nenhuma delas precisa lembrar de nada.

### O choque com Delphi

```pascal
// Delphi: isso funciona. O objeto vive.
procedure TFormEmpresa.btnProximoClick(Sender: TObject);
begin
  FRazaoSocial := edtRazaoSocial.Text;  // fica na memória
  PageControl1.ActivePageIndex := 1;    // outra aba, mesmo objeto
end;                                    // FRazaoSocial ainda está lá
```

```csharp
// ASP.NET Core: isso NÃO funciona.
public class CadastroModel : PageModel
{
    private string _razaoSocial;   // morre no fim da requisição

    public void OnPost()
    {
        _razaoSocial = Request.Form["razaoSocial"];
    }
    // Próxima requisição = NOVA instância de CadastroModel.
    // _razaoSocial é null de novo. Sempre.
}
```

O `PageModel` é criado, usado e descartado a cada requisição. Você não tem controle sobre isso e não deve tentar burlar (campo `static` para "guardar" estado = bug de concorrência garantido, e quebra assim que subir a segunda réplica).

### As três formas de simular estado

Como então o site sabe que você está logado? Você **carrega o estado junto** em toda requisição. Três lugares:

**1. Cookie** — um pedacinho de texto que o servidor manda uma vez e o navegador **reenvia automaticamente** em toda requisição para aquele domínio.

```http
# resposta do servidor, uma vez:
Set-Cookie: sessao=a7f3c9e1; HttpOnly; Secure; SameSite=Lax; Max-Age=3600

# toda requisição seguinte, automático:
Cookie: sessao=a7f3c9e1
```

Flags que importam em sistema fiscal:
- `HttpOnly` — JavaScript **não** consegue ler. Barra roubo de sessão por XSS.
- `Secure` — só trafega em HTTPS.
- `SameSite=Lax` — não é enviado em requisições vindas de outro site. Defesa contra CSRF.

**2. Sessão no servidor** — o cookie guarda só um ID; os dados ficam no servidor, num dicionário indexado por esse ID.

Aqui mora a armadilha de "é stateless, escala sozinho": se a sessão está na memória do Pod A e a próxima requisição cai no Pod B, os dados sumiram. Por isso sessão distribuída exige Redis. Guarde: **estado no servidor é inimigo de escala horizontal**.

**3. No próprio HTML / no cliente** — campos `hidden` no formulário, `localStorage`, query string. O dado vai e volta a cada requisição. É a opção mais compatível com múltiplas réplicas — e é a base do modelo hipermídia do HTMX.

> Resposta da prova de conhecimento nº 1 está aqui. Não decore o texto: entenda o mecanismo, você vai ter que explicar com suas palavras.

### Onde tem estado de verdade

O único lugar de estado permanente é o **banco de dados**. Formulário fiscal longo (NF-e com 40 campos, wizard de SPED) precisa de **autosave** para o banco. Isso é código que você escreve, não propriedade grátis da arquitetura. Você faz isso na Semana 9.

---

## 7. Cache, em uma respirada

O navegador guarda respostas para não pedir de novo. Controlado por headers:

- `Cache-Control: max-age=3600` — pode reusar por 1 hora sem perguntar
- `Cache-Control: no-store` — nunca guarde (dado sensível)
- `ETag` + `If-None-Match` — "tenho a versão X, mudou?" → `304 Not Modified`, resposta sem corpo

Regra: CSS/JS/imagem = cache agressivo. Página com dado fiscal = `no-store`.

---

## 8. DevTools — sua nova casa

F12 no navegador. As duas abas que importam agora:

**Network**
- Ligue **Preserve log** (senão o redirect apaga o histórico e você não vê o POST)
- Clique numa linha → **Headers** (requisição e resposta), **Payload** (o que foi enviado), **Response** (o que voltou)
- Coluna **Status** e coluna **Type**

**Elements**
- O HTML **como está agora** na memória do navegador (o DOM), não o arquivo original
- Painel Styles à direita: quais regras CSS se aplicam, quais foram sobrescritas (aparecem riscadas)

**Faça agora:** vá em qualquer site com login, F12 → aba **Application** → **Cookies**. Veja o cookie de sessão. Apague-o e recarregue: você foi deslogado. Acabou de ver, na prática, que "estar logado" é só um texto no cookie.

---

## Checklist de saída

Você só passa desta teoria quando consegue responder sem consultar:

- [ ] Por que HTTP é stateless e qual a vantagem disso?
- [ ] Três formas de simular estado, com o trade-off de cada uma
- [ ] Diferença entre 401 e 403
- [ ] Por que `GET` nunca deve alterar dados
- [ ] O que é `HttpOnly` e de que ataque ele protege
- [ ] Por que uma página gera dezenas de requisições

Próximo: [`teoria-02-html.md`](teoria-02-html.md)
