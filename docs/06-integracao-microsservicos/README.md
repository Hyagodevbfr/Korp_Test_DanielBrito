
# Integração entre Microsserviços
A comunicação entre os microsserviços será realizada via HTTP.

O **Billing Service** será responsável por solicitar operações de estoque ao **Stock Service**.

## Fluxo de Fechamento
`Angular → Billing Service → Stock Service`

1. O usuário solicita a impressão da nota;
2. O Billing Service valida se a nota está **Aberta**;
3. Obtém os produtos e quantidades da nota;
4. Solicita a baixa ao Stock Service;
5. O Stock Service valida a disponibilidade dos produtos;
6. O estoque é atualizado;
7. O Billing Service recebe a confirmação;
8. A nota é alterada para **Fechada**.

## Comunicação
A integração será realizada utilizando `HttpClientFactory`.

O Billing Service não poderá acessar diretamente o banco de dados do Stock Service.

Toda operação relacionada ao estoque deverá passar pela API do Stock Service.

## Falhas
Caso o Stock Service esteja indisponível ou a baixa de estoque falhe:

- A nota deverá permanecer **Aberta**;
- A operação não deverá ser considerada concluída;
- O erro deverá ser retornado de forma controlada ao frontend.