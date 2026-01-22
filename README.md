# Sistema de Gestão de Clientes

Este repositório contém a Prova de Conceito (POC) para o sistema de gestão de clientes, desenvolvida com foco em alta performance, segurança e escalabilidade.

## Visão Geral do Projeto

O sistema permite o gerenciamento completo de clientes (CRUD), incluindo múltiplos endereços e armazenamento de logotipos corporativos, com restrições de unicidade de e-mail e validações de negócio.

## Tecnologias Utilizadas

A solução foi desenvolvida utilizando as seguintes tecnologias e frameworks:

- **Back-end:** .NET 6+ (C#).

- **Front-end:** ASP.NET Core MVC com Razor Views e JavaScript.

- **Banco de Dados:** SQL Server 2016+.

- **ORM e Micro-ORM:** Entity Framework Core e Dapper.
- **Mapeamento e Validação:** AutoMapper e FluentValidation.
- **Testes:** xUnit.

## Estrutura do Projeto

A solução está organizada em camadas físicas para garantir a separação de responsabilidades:

### Camada de Aplicação

- `src/SistemaGestao.Domain`: Entidades, Interfaces de Repositório e Regras de Negócio.
- `src/SistemaGestao.Application`: Casos de uso, DTOs e Serviços.
- `src/SistemaGestao.Infra.Data`: Implementação de persistência (Contexto EF, Repositórios e chamadas de Procedures).
- `src/SistemaGestao.API`: API RESTful responsável por expor os dados.
- `src/SistemaGestao.Web`: Aplicação Front-end MVC que consome a API.

### Camada de Testes

- `tests/SistemaGestao.Domain.Tests`: Testes unitários das entidades e validações de domínio.
- `tests/SistemaGestao.Application.Tests`: Testes unitários dos serviços de aplicação.
- `tests/SistemaGestao.API.IntegrationTests`: Testes de integração da API.

## Configuração e Execução

Para executar o projeto localmente, siga os passos abaixo:

### 1. Pré-requisitos

- SDK do .NET 6.0 ou superior.
- SQL Server (Instância local ou container Docker).

### 2. Configuração do Banco de Dados

Antes de iniciar a aplicação, é necessário criar a estrutura do banco de dados e as Stored Procedures obrigatórias.
Execute o script localizado em:
`docs/setup_banco.sql`

### 3. Execução da API

Navegue até o diretório da API e inicie o serviço:

```bash
cd src/SistemaGestao.API
dotnet run

```

A API estará disponível em `https://localhost:5001` (ou porta configurada).

### 4. Execução do Front-end

Em um novo terminal, navegue até o diretório da aplicação Web:

```bash
cd src/SistemaGestao.Web
dotnet run

```

O Front-end estará acessível em `https://localhost:5002`.

## Documentação Completa

A documentação detalhada da solução, evidenciando como cada requisito foi atendido, juntamente com diagramas e explicações aprofundadas sobre o Front-end e Back-end, encontra-se disponível no diretório de documentação.

[Acesse a Documentação Técnica (docs/)](./docs/)
