# Frontend
O frontend será desenvolvido utilizando **Angular**.

Será responsável pelo cadastro de produtos, criação de notas fiscais e acompanhamento do processo de fechamento.

## Telas
### Produtos
- Listagem de produtos;
- Cadastro de produto;
- Visualização do saldo disponível.

### Notas Fiscais
- Listagem de notas;
- Criação de uma nova nota;
- Inclusão de múltiplos produtos;
- Definição da quantidade de cada produto;
- Visualização do status da nota.

### Detalhes da Nota
Deverá apresentar:

- Número da nota;
- Status;
- Produtos;
- Quantidades;
- Botão de impressão.

O botão de impressão estará disponível apenas para notas **Abertas**.

## Biblioteca Visual
Será utilizado **Angular Material** para os componentes visuais.

Principais componentes:

- Inputs;
- Selects;
- Buttons;
- Tables;
- Snackbar;
- Spinner;
- Chips para status.

## Comunicação com o Backend
A comunicação com as APIs será realizada através do `HttpClient` do Angular.

Os acessos ao backend serão centralizados em services.

Exemplo:

`InvoiceService → Billing Service`

`ProductService → Stock Service`

## Loading
Durante o processo de impressão da nota, a interface deverá apresentar um indicador de processamento.

O botão deverá permanecer bloqueado enquanto a operação estiver sendo executada.

## RxJS
RxJS será utilizado no tratamento das operações assíncronas realizadas através do `HttpClient`.

Possíveis operadores:

- `finalize`: controle do loading;
- `catchError`: tratamento de erros;
- `switchMap`: quando houver operações dependentes.

O detalhamento técnico final deverá informar somente os operadores realmente utilizados.

## Lifecycle Hooks
### ngOnInit
Será utilizado quando necessário para carregar informações iniciais das telas, como:

- Produtos;
- Notas;
- Detalhes da nota.

### ngOnDestroy
Será utilizado somente caso existam subscriptions ou recursos que precisem ser encerrados manualmente.

Não serão utilizados lifecycle hooks sem necessidade real.