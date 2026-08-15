                   ┌───────────────────┐
                   │      Angular      │
                   └─────────┬─────────┘
                             │
                 ┌───────────┴───────────┐
                 │                       │
                 ▼                       ▼
        ┌─────────────────┐     ┌─────────────────┐
        │ Billing Service │────▶│  Stock Service  │
        └────────┬────────┘     └────────┬────────┘
                 │                       │
                 ▼                       ▼
          Billing Database        Stock Database

# Stock Service
Responsável exclusivamente por:

 - produtos;
 - disponibilidade;
 - saldo;
 - movimentação de estoque.

# Billing Service
Responsável por:

 - notas fiscais;
 - itens das notas;
 - número sequencial;
 - status;
 - fechamento da nota.

# Regra importante
O Billing Service:

Não acessa diretamente o banco do Stock Service.

Quando precisar alterar estoque, deverá chamar uma API do Stock Service.

Isso mantém os microsserviços realmente separados.