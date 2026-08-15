# Backend
O backend será desenvolvido em **C# com .NET 8 e ASP.NET Core Web API**.

Será dividido em dois microsserviços:

- **Stock Service**: responsável pelos produtos e controle de estoque;
- **Billing Service**: responsável pelas notas fiscais e seus itens.

Cada serviço será responsável pelos próprios dados e a comunicação entre eles será feita via HTTP.

## Tecnologias
- **.NET 8 / ASP.NET Core**: desenvolvimento das APIs;
- **Entity Framework Core**: persistência e consultas;
- **SQL Server**: banco de dados;
- **HttpClientFactory**: comunicação entre os microsserviços;
- **Swagger/OpenAPI**: documentação e testes das APIs;
- **ProblemDetails**: padronização dos erros.

## LINQ
Será utilizado nas consultas e manipulações de dados, principalmente com:

- `Where`;
- `Select`;
- `Any`;
- `FirstOrDefault`.

O detalhamento técnico final deverá registrar apenas os usos realmente implementados.

## Tratamento de erros
Os erros serão tratados de forma centralizada e retornados utilizando `ProblemDetails`.

Principais respostas:

- `400`: dados inválidos;
- `404`: recurso não encontrado;
- `409`: conflito de regra de negócio;
- `503`: microsserviço indisponível;
- `500`: erro inesperado.

## Comunicação
O **Billing Service** não acessará diretamente os dados do **Stock Service**.

Fluxo principal:

`Billing Service → Stock Service`

Durante o fechamento da nota, o Billing solicitará a baixa do estoque e somente alterará a nota para **Fechada** após a confirmação da operação.