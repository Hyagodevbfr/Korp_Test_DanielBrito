# Billing Service
O **Billing Service** será responsável pela criação e gerenciamento das notas fiscais.

## Responsabilidades
- Criar notas fiscais;
- Gerar a numeração sequencial;
- Gerenciar os itens da nota;
- Controlar o status;
- Realizar o fechamento;
- Solicitar a baixa de estoque ao Stock Service.

## Endpoints
`POST /api/invoices`

Cria uma nova nota fiscal com status **Aberta**.

`GET /api/invoices`

Retorna as notas fiscais cadastradas.

`GET /api/invoices/{id}`

Retorna uma nota fiscal específica e seus itens.

`POST /api/invoices/{id}/close`

Realiza o fechamento da nota fiscal.

## Fechamento
Ao fechar uma nota:

1. Validar se a nota está **Aberta**;
2. Validar seus itens;
3. Solicitar a baixa ao Stock Service;
4. Aguardar a confirmação;
5. Alterar o status para **Fechada**.

Caso a baixa de estoque falhe, a nota deverá permanecer **Aberta**.

Uma nota **Fechada** não poderá ser processada novamente.