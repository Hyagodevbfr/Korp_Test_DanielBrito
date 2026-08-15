# Stock Service
O **Stock Service** será responsável pelo cadastro dos produtos e controle de estoque.

## Responsabilidades
- Cadastrar produtos;
- Consultar produtos;
- Controlar o saldo disponível;
- Validar disponibilidade;
- Realizar a baixa de estoque.

## Endpoints
### Produtos

`POST /api/products`

Cadastra um novo produto.

`GET /api/products`

Retorna os produtos cadastrados.

`GET /api/products/{id}`

Retorna um produto específico.

### Estoque

`POST /api/stock/consume`

Realiza a baixa dos produtos utilizados em uma nota fiscal.

A operação deverá receber os produtos e respectivas quantidades.

## Regras
- O saldo não poderá ficar negativo;
- Todos os produtos devem possuir saldo suficiente antes da operação ser concluída;
- A baixa de múltiplos produtos deverá ser realizada de forma consistente.