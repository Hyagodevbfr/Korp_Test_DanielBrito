# Falhas e Resiliência
O backend deverá tratar falhas de forma controlada, evitando expor detalhes internos da aplicação.

Os erros serão padronizados utilizando `ProblemDetails`.

## Respostas HTTP
- `400 Bad Request`: dados inválidos;
- `404 Not Found`: produto ou nota não encontrada;
- `409 Conflict`: conflito de regra de negócio;
- `503 Service Unavailable`: microsserviço dependente indisponível;
- `500 Internal Server Error`: erro inesperado.

## Falha do Stock Service
Caso o **Stock Service** esteja indisponível durante o fechamento de uma nota:

1. O Billing Service deverá identificar a falha;
2. A nota deverá permanecer **Aberta**;
3. O Billing Service deverá retornar `503 Service Unavailable`;
4. O frontend deverá informar que não foi possível concluir a operação.

## Timeout
A comunicação entre o **Billing Service** e o **Stock Service** deverá possuir um tempo limite.

Caso o Stock Service não responda dentro desse período, a operação será tratada como falha e a nota permanecerá **Aberta**.

## Retry
Não será utilizado retry automático inicialmente em operações de baixa de estoque.

Operações que alteram dados podem ser executadas com sucesso mesmo que a resposta HTTP seja perdida, portanto repetir automaticamente a requisição poderia gerar inconsistências.

Retry poderá ser avaliado posteriormente em uma etapa futura.