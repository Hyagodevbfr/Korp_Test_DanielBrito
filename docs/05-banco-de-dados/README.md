# Banco de Dados - Estoque
O **Stock Service** será responsável pela persistência dos produtos e controle de saldo.

O banco utilizado será **SQL Server**.

## Produto
A entidade de produto deverá possuir:

- **Id**: identificador interno;
- **Code**: código único do produto;
- **Description**: descrição do produto;
- **Balance**: saldo disponível em estoque;
- **CreatedAt**: data de criação;
- **UpdatedAt**: data da última alteração.

## Regras
- O código do produto deve ser único;
- O saldo não pode ser negativo;
- A quantidade utilizada em uma nota deve ser descontada do saldo;
- O estoque não poderá ser alterado diretamente pelo Billing Service.

---

# Banco de Dados - Faturamento
O **Billing Service** será responsável pela persistência das notas fiscais e seus itens.

O banco utilizado será **SQL Server**.

## Nota Fiscal
A entidade de nota fiscal deverá possuir:

- **Id**: identificador interno;
- **Number**: número sequencial da nota;
- **Status**: status atual da nota;
- **CreatedAt**: data de criação;
- **ClosedAt**: data de fechamento.

Os status disponíveis serão:

- **Aberta**;
- **Fechada**.

Toda nova nota deverá ser criada com status **Aberta**.

## Itens da Nota
Cada nota poderá possuir um ou mais produtos.

A entidade de item deverá possuir:

- **Id**: identificador interno;
- **InvoiceId**: identificação da nota;
- **ProductId**: identificação do produto;
- **ProductCode**: código do produto;
- **ProductDescription**: descrição do produto;
- **Quantity**: quantidade utilizada.

O código e a descrição do produto serão armazenados na nota como um **snapshot**, preservando as informações existentes no momento da criação.

## Regras
- O número da nota deve ser único e sequencial;
- A quantidade de cada item deve ser maior que zero;
- Uma nota deve possuir pelo menos um produto;
- Apenas notas **Abertas** poderão ser fechadas;
- Uma nota **Fechada** não poderá gerar uma nova baixa de estoque.