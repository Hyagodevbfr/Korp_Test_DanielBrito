## Produtos
Cada produto deverá possuir:

 - Código;
 - Descrição;
 - Saldo disponível;
 
O código do produto seve ser único.

O produto será cadastrado antes de ser útilizado em uma nota fiscal.

# Nota Fiscal
Cada nota deverá possuir:

  - Número sequencial;
  - Status;
  - Um ou mais produtos;
  - Quantidade de cada produto.

  Estados disponíveis:
  
 - Aberta
 - Fechada

  Toda nota nasce como:
  
 - Aberta

# Fechamento da nota
Somente uma nota **Aberta** poderá ser processada.

Ao concluir a operação:
 1. validar a nota;
 2. validar os itens;
 3. validar a disponibilidade dos produtos;
 4. reduzir o saldo;
 5. alterar a nota para fechada.

Uma nota **Fechada** não poderá provocar uma nova baixa de estoque.

# Falhas
Caso o serviço de estoque esteja indisponível:
 - a operação deverá falhar de forma controlada;
 - a nota não deverá ser considerada concluída;
  o frontend deverá receber uma mensagem apropriada.