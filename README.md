# TODOapp

Este é um aplicativo TODO que utiliza HTML, CSS e JavaScript puro para o frontend e ASP.NET com Entity Framework Core para o backend.

## Estrutura do Projeto

- **Frontend:** HTML, CSS e JavaScript
- **Backend:** ASP.NET com Entity Framework Core

## Requisitos

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

## Instalação e Configuração

### Backend

1. Clone o repositório:
    ```bash
    git clone https://github.com/lucao12/TODOapp-AspNet.git
    ```

2. Navegue para o diretório do projeto:
    ```bash
    cd TODOapp-AspNet
    ```

3. Navegue até a pasta `Properties` e abra o arquivo `launchSettings.json` para configurar o endereço do servidor.

4. Altere o endereço do servidor diretamente no arquivo `launchSettings.json`, em `profiles`. Recomenda-se usar o endereço IP do seu computador (você pode encontrá-lo com o comando `ipconfig` no terminal).

5. Execute o backend:
    ```bash
    dotnet run
    ```

### Frontend

1. No diretório do frontend, localize todos os arquivos `.js` e atualize o endereço do backend para corresponder ao novo endereço configurado no backend.

2. Certifique-se de que todos os endpoints estejam corretos e correspondam à configuração do backend.

## Uso

1. Certifique-se de que o backend está em execução.
2. Abra o frontend no seu navegador. Se você configurou tudo corretamente, o frontend deve se comunicar com o backend e você poderá gerenciar suas tarefas TODO.

## Contato

Lucas Rodrigues - lucasrs2206@gmail.com
