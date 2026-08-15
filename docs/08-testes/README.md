# Testes
Os testes deverão validar principalmente as regras de negócio e a integração entre os microsserviços.

## Produtos
- Cadastrar produto válido;
- Impedir código duplicado;
- Validar campos obrigatórios;
- Impedir saldo negativo.

## Notas Fiscais
- Criar nota com status **Aberta**;
- Gerar numeração sequencial;
- Adicionar múltiplos produtos;
- Validar quantidade dos itens.

## Fechamento
- Fechar uma nota **Aberta**;
- Atualizar corretamente o saldo dos produtos;
- Alterar o status para **Fechada**;
- Impedir novo fechamento de uma nota **Fechada**;
- Impedir fechamento quando não houver saldo suficiente.

## Integração
- Validar a comunicação entre Billing Service e Stock Service;
- Garantir que a nota só seja fechada após confirmação da baixa de estoque.

## Falhas
- Stock Service indisponível;
- Timeout na comunicação;
- Erro durante a baixa de estoque;
- Garantir que a nota permaneça **Aberta** quando a operação falhar.

## Tipos de Teste
Serão utilizados conforme necessário:

- **Unitários**: regras de negócio;
- **Integração**: banco de dados e comunicação entre serviços;
- **Manuais**: validação do fluxo completo pela interface.