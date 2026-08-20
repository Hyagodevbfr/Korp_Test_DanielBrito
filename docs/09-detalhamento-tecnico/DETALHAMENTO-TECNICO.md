# Detalhamento Técnico

## Visão Geral

O projeto implementa o sistema de emissão de notas fiscais proposto no desafio: cadastro de produtos com controle de saldo, criação de notas fiscais com múltiplos itens, e um fluxo de fechamento/impressão que dá baixa no estoque de forma consistente entre dois microsserviços independentes. O frontend é uma SPA em Angular com Angular Material; o backend é dividido em dois serviços .NET (Stock Service e Billing Service), cada um com seu próprio banco SQL Server, comunicando-se via HTTP síncrono.

## Arquitetura

A aplicação foi dividida em dois microsserviços:

- **Stock Service**: responsável pelo cadastro de produtos e pelo controle de saldo em estoque;
- **Billing Service**: responsável pelo cadastro e fechamento de notas fiscais.

A comunicação entre os serviços é feita via HTTP, com o Billing Service atuando como cliente do Stock Service (nunca o contrário). Ao fechar uma nota, o Billing Service consulta os produtos e solicita a baixa de estoque ao Stock Service antes de marcar a nota como Fechada — não existe transação distribuída entre os dois bancos; a consistência é garantida pela ordem das operações (a baixa de estoque só é confirmada se o Stock Service aceitar a operação, e a nota só fecha depois disso) e pelo tratamento de falha descrito mais adiante.

Cada serviço segue a mesma organização em camadas:

- `Api`: endpoints HTTP (Minimal APIs) e configuração da aplicação;
- `Application`: use cases, um por operação de negócio;
- `Domain`: entidades, regras de negócio e o padrão `Result` usado para representar sucesso/falha sem exceções;
- `Infrastructure`: EF Core, migrations e o cliente HTTP que fala com o outro serviço.

## Tecnologias

### Frontend
- **Angular** (standalone components, sem NgModules);
- **TypeScript**;
- **RxJS**, usado no tratamento de chamadas assíncronas via `HttpClient`;
- **Angular Material** como biblioteca visual, com **Tailwind CSS** para estilização utilitária complementar.

### Backend
- **C#** / **.NET**;
- **ASP.NET Core** com Minimal APIs;
- **Entity Framework Core**;
- **SQL Server**, um banco por serviço (`KorpStock` e `KorpBilling`).

## Angular Lifecycle Hooks

- **`ngOnInit`**: usado em todos os componentes que precisam carregar dado assim que a tela é montada — lista de produtos, lista de notas fiscais, formulário de nova nota (carrega os produtos disponíveis para o select) e detalhe da nota (busca a nota pelo id recebido na rota).
- **`ngOnDestroy`**: não foi utilizado em nenhum componente. Essa foi uma decisão deliberada e não uma omissão: como todas as chamadas assíncronas passam pelo `HttpClient` do Angular, os Observables retornados completam sozinhos assim que a resposta HTTP chega — não existem subscriptions de longa duração, timers ou listeners manuais que precisem ser encerrados no componente, então adicionar `ngOnDestroy` só para cumprir o hook seria código sem função real.

## RxJS

RxJS é usado em dois pontos concretos da aplicação:

- **`finalize`**: usado para controlar o estado de carregamento das operações que disparam requisição HTTP (criar nota, criar/editar produto, fechar nota). O `isLoading` é ligado antes da chamada e o `finalize` garante que ele seja desligado independentemente do resultado ter sido sucesso ou erro, mantendo o botão da ação bloqueado durante todo o processamento.
- **`catchError`** combinado com **`throwError`**: usado no interceptor HTTP global (`error.interceptor.ts`). Ele intercepta qualquer erro de qualquer requisição feita pela aplicação, exibe uma mensagem de erro amigável via snackbar e repropaga o erro, para que quem fez a chamada ainda possa reagir localmente se precisar.

Não foi necessário usar `switchMap` nem outros operadores de combinação — a aplicação não tem, no momento, fluxos de chamadas HTTP encadeadas/dependentes dentro do mesmo stream reativo.

## Bibliotecas

| Biblioteca | Finalidade |
|---|---|
| `@angular/material` + `@angular/cdk` | Componentes visuais: tabelas, dialogs, formulários, chips, spinner, snackbar |
| `@angular/animations` | Animações usadas pelos componentes do Angular Material |
| `rxjs` | Tratamento de operações assíncronas (`finalize`, `catchError`) |
| `tailwindcss` (+ `@tailwindcss/postcss`) | Estilização utilitária nos templates, complementando o Angular Material |
| `zone.js` | Mecanismo padrão de change detection do Angular |

Do lado do backend, além do framework em si (ASP.NET Core e EF Core, detalhados abaixo), o único pacote adicional relevante é o `Swashbuckle.AspNetCore`, usado apenas em desenvolvimento para expor o Swagger UI de cada serviço.

## Componentes Visuais

A biblioteca visual escolhida foi o **Angular Material**, usada de forma consistente nas duas áreas da aplicação (Produtos e Notas Fiscais). Os principais componentes utilizados:

- `MatTable` para as listagens de produtos e notas fiscais;
- `MatDialog` para os formulários de criação/edição de produto e criação de nota, que abrem como modal sobre a listagem;
- `MatFormField`, `MatInput` e `MatSelect` nos formulários;
- `MatChip` para exibir o status da nota (Aberta/Fechada) com destaque visual;
- `MatProgressSpinner` nos indicadores de carregamento, tanto nos botões de ação quanto na tela de detalhe da nota;
- `MatButton`/`MatIconButton` e `MatIcon` nas ações;
- `MatSnackBar`, encapsulado em um serviço próprio (`Toaster`) que centraliza as mensagens de sucesso, erro e aviso da aplicação inteira, evitando que cada componente monte sua própria configuração de snackbar.

## Frameworks no C#

- **ASP.NET Core**, usando o modelo de **Minimal APIs**: os endpoints de cada serviço são registrados como extension methods sobre `WebApplication` (`MapProductEndpoints()`, `MapInvoiceEndpoints()`), sem uso de Controllers baseados em classe;
- **Entity Framework Core**, com um `DbContext` por serviço (`StockDbContext` e `BillingDbContext`) e migrations independentes — reforçando que os dois microsserviços têm bancos de dados fisicamente separados, sem nenhuma tabela compartilhada;
- Não foi utilizado Golang no projeto — o backend foi implementado inteiramente em C#/.NET.

## Tratamento de Erros e Exceções

O tratamento de erros segue três camadas complementares:

**1. Erros de regra de negócio (esperados):** o domínio e a camada de aplicação usam o padrão `Result`/`Result<T>` em vez de lançar exceções para casos como produto não encontrado, saldo insuficiente, código de produto duplicado ou nota já fechada. Cada `Result` carrega um `ErrorType` (`Validation`, `NotFound`, `Conflict` e, no Billing Service, também `Unavailable`), que é convertido para uma resposta HTTP padronizada por uma extensão (`ToHttpResult()`) usando `Results.Problem(...)` — ou seja, toda resposta de erro de negócio segue o formato `ProblemDetails` da RFC 7807, com o status HTTP correspondente (400, 404, 409 ou 503).

**2. Exceções não esperadas:** ambos os serviços registram `AddProblemDetails()` e `UseExceptionHandler()` no `Program.cs`. Isso garante que qualquer exceção não tratada (um bug, uma falha de infraestrutura) seja capturada centralizadamente e devolvida como um `ProblemDetails` genérico com status 500, sem vazar stack trace ou detalhes internos para o cliente.

**3. Falha de comunicação entre os microsserviços:** este é o cenário de falha exigido pelo desafio. O `StockServiceClient`, usado pelo Billing Service para falar com o Stock Service, captura `HttpRequestException` e timeout (`TaskCanceledException`) ao redor de cada chamada HTTP e converte isso em `ErrorType.Unavailable`, que vira um 503 Service Unavailable. Quando isso acontece durante o fechamento de uma nota, a baixa de estoque não é aplicada e a nota permanece com status Aberta — o fechamento simplesmente não avança. O frontend recebe esse erro pelo interceptor HTTP global e exibe uma mensagem ao usuário informando que não foi possível concluir a operação, sem travar a aplicação.

**4. Concorrência no consumo de estoque:** como reforço adicional de consistência, o `Product` tem um token de concorrência otimista (`RowVersion`, mapeado como `rowversion` no SQL Server). Se duas requisições tentarem consumir saldo do mesmo produto ao mesmo tempo — por exemplo, duas notas fechando simultaneamente e disputando a última unidade em estoque —, a segunda operação a tentar salvar é rejeitada pelo Entity Framework Core com uma `DbUpdateConcurrencyException`, que o `ProductRepository` traduz para o mesmo padrão `Result` já usado no resto da aplicação, com `ErrorType.Conflict` (409). Isso evita que o saldo fique inconsistente (ou negativo) por causa de uma corrida entre requisições, sem precisar de lock pessimista nem de uma transação distribuída entre os dois serviços — e, como consequência, também evita que a mesma nota seja fechada duas vezes com baixa de estoque duplicada, já que a segunda tentativa de consumo é rejeitada antes da nota ser efetivamente marcada como Fechada. Esse comportamento está coberto por testes automatizados no Stock Service.

## Persistência

- **SQL Server** como banco de dados, com **Entity Framework Core** como ORM;
- Um `DbContext` por microsserviço (`StockDbContext` para o Stock Service, `BillingDbContext` para o Billing Service), cada um com seu próprio conjunto de migrations, versionadas junto com o código de cada serviço;
- As connection strings ficam em `appsettings.Development.json`, arquivo fora do controle de versão (cada serviço mantém um `appsettings.Development.json.example` versionado como referência de configuração);
- Os dados de Stock Service e Billing Service são completamente separados: o Billing Service nunca acessa o banco do Stock Service diretamente, apenas através da API HTTP exposta por ele.

## Tratamento de Falhas

O cenário de falha implementado e demonstrado no vídeo é a indisponibilidade do Stock Service durante o fechamento de uma nota fiscal:

1. O Stock Service é derrubado propositalmente;
2. Uma tentativa de fechar uma nota Aberta é feita a partir do frontend;
3. O Billing Service tenta se comunicar com o Stock Service para dar baixa no estoque e recebe uma falha de conexão;
4. Essa falha é capturada e convertida em uma resposta 503, sem exceção não tratada;
5. A nota permanece com status Aberta — nenhuma baixa de estoque é aplicada e a nota não é fechada;
6. O frontend recebe o erro através do interceptor HTTP global e exibe uma mensagem ao usuário, sem quebrar a navegação.

O sistema se recupera normalmente assim que o Stock Service volta a responder: basta tentar fechar a nota novamente.

## Como Executar

**Pré-requisitos:** .NET SDK, Node.js, e uma instância de SQL Server acessível localmente (SQL Server local ou LocalDB).

**1. Bancos de dados**

Em cada serviço, copiar o arquivo de exemplo e preencher a connection string:

```bash
cp src/backend/StockService/Korp.Stock.Api/appsettings.Development.json.example src/backend/StockService/Korp.Stock.Api/appsettings.Development.json
cp src/backend/BillingService/Korp.Billing.Api/appsettings.Development.json.example src/backend/BillingService/Korp.Billing.Api/appsettings.Development.json
```

Aplicar as migrations de cada serviço:

```bash
dotnet ef database update --project src/backend/StockService/Korp.Stock.Infrastructure --startup-project src/backend/StockService/Korp.Stock.Api
dotnet ef database update --project src/backend/BillingService/Korp.Billing.Infrastructure --startup-project src/backend/BillingService/Korp.Billing.Api
```

**2. Backend**

Subir os dois serviços (cada um em um terminal):

```bash
dotnet run --project src/backend/StockService/Korp.Stock.Api    # http://localhost:5079
dotnet run --project src/backend/BillingService/Korp.Billing.Api # http://localhost:5270
```

**3. Frontend**

```bash
cd src/frontend
npm install
npm start   # http://localhost:4200
```

As URLs das APIs consumidas pelo frontend estão configuradas em `src/frontend/src/environments/environment.development.ts`, apontando por padrão para as portas acima.
