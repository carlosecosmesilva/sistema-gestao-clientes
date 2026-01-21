# Sistema de Gestão de Clientes

Este repositório contém a Prova de Conceito (POC) para o sistema de gestão de clientes, desenvolvida com foco em alta performance, segurança e escalabilidade. A solução foi arquitetada para atender aos requisitos de expansão e garantir integridade de dados além de otimização de recursos.

## Visão Geral do Projeto

A solução foi projetada seguindo os princípios da **Clean Architecture**, visando o desacoplamento entre as regras de negócio, a interface de usuário e a infraestrutura de dados. O sistema permite o gerenciamento completo de clientes (CRUD), incluindo múltiplos endereços e armazenamento de logotipos corporativos, com restrições de unicidade de e-mail e validações de negócio.

### Decisões Arquiteturais

Para atender aos requisitos não funcionais de alta performance e às restrições técnicas impostas, foram adotadas as seguintes estratégias:

- **Persistência Híbrida (CQRS Simplificado):**
- **Leitura (Query):** Utilização do **Entity Framework Core** com `AsNoTracking` e projeções diretas para DTOs. Isso minimiza o overhead de memória e serialização nas consultas de listagem.

- **Escrita (Command):** Utilização de **Stored Procedures** executadas via **Dapper**. Esta abordagem garante a execução performática de operações transacionais e atende à restrição de uso de procedures para operações complexas.

- **Gerenciamento de BLOBs (Imagens):** Embora as imagens sejam armazenadas no banco de dados (VARBINARY) conforme requisito, a arquitetura implementa o carregamento sob demanda (_Lazy Loading_ manual). As listagens retornam apenas metadados, evitando gargalos de tráfego de rede; o conteúdo binário é trafegado apenas em endpoints específicos de detalhamento.

- **Segurança:** A API implementa autenticação e autorização via JWT (JSON Web Token), garantindo que o acesso, embora público, seja seguro e auditável.

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

- `src/SistemaGestao.Domain`: Entidades, Interfaces de Repositório e Regras de Negócio.
- `src/SistemaGestao.Application`: Casos de uso, DTOs e Serviços.
- `src/SistemaGestao.Infra.Data`: Implementação de persistência (Contexto EF, Repositórios e chamadas de Procedures).
- `src/SistemaGestao.API`: API RESTful responsável por expor os dados.
- `src/SistemaGestao.Web`: Aplicação Front-end MVC que consome a API.

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
