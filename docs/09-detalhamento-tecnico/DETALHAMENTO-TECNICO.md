# Detalhamento Técnico
## Visão Geral
Breve descrição da solução desenvolvida e da arquitetura utilizada.

## Arquitetura
A aplicação foi dividida em dois microsserviços:

- **Stock Service**: responsável pelos produtos e controle de estoque;
- **Billing Service**: responsável pelas notas fiscais.

A comunicação entre os serviços é realizada via HTTP.

## Tecnologias
### Frontend
- Angular:
- TypeScript:
- RxJS:
- Biblioteca visual:

### Backend
- C#:
- .NET:
- ASP.NET Core:
- Entity Framework Core:
- SQL Server:

## Angular Lifecycle Hooks
Descrever os ciclos de vida utilizados e onde foram aplicados.

Exemplo:

- `ngOnInit`:
- `ngOnDestroy`:

## RxJS
Descrever os operadores utilizados e suas finalidades.

Exemplo:

- `finalize`:
- `catchError`:
- `switchMap`:

## Bibliotecas
Listar somente as bibliotecas realmente utilizadas.

| Biblioteca | Finalidade |
|---|---|
|   |   |

## Componentes Visuais

Informar a biblioteca visual escolhida e os principais componentes utilizados.

## LINQ

Descrever onde LINQ foi utilizado no backend.

Exemplo:

- Consultas;
- Filtros;
- Projeções;
- Validações.

## Tratamento de Erros e Exceções

Descrever:

- Tratamento centralizado de exceções;
- Uso de `ProblemDetails`;
- Códigos HTTP utilizados;
- Tratamento de falhas entre os microsserviços.

## Persistência

Descrever:

- SQL Server;
- Entity Framework Core;
- Migrations;
- Separação dos dados entre Stock Service e Billing Service.

## Tratamento de Falhas

Descrever o cenário de falha implementado e como o sistema se comporta quando o Stock Service está indisponível.

## Como Executar

Adicionar as instruções necessárias para executar o frontend, os microsserviços e o banco de dados.