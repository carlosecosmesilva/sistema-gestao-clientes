# Arquitetura do Sistema

## Visão Geral

A solução foi projetada seguindo os princípios da **Clean Architecture**, visando o desacoplamento entre as regras de negócio, a interface de usuário e a infraestrutura de dados. Esta abordagem garante a manutenibilidade, testabilidade e evolução do sistema de forma sustentável.

## Decisões Arquiteturais

Para atender aos requisitos não funcionais de alta performance e às restrições técnicas impostas, foram adotadas as seguintes estratégias:

### Persistência Híbrida (CQRS Simplificado)

#### Leitura (Query)

Utilização do **Entity Framework Core** com `AsNoTracking` e projeções diretas para DTOs. Isso minimiza o overhead de memória e serialização nas consultas de listagem.

#### Escrita (Command)

Utilização de **Stored Procedures** executadas via **Dapper**. Esta abordagem garante a execução performática de operações transacionais e atende à restrição de uso de procedures para operações complexas.

### Gerenciamento de BLOBs (Imagens)

Embora as imagens sejam armazenadas no banco de dados (VARBINARY) conforme requisito, a arquitetura implementa o carregamento sob demanda (Lazy Loading manual). As listagens retornam apenas metadados, evitando gargalos de tráfego de rede; o conteúdo binário é trafegado apenas em endpoints específicos de detalhamento.

### Segurança

A API implementa autenticação e autorização via JWT (JSON Web Token), garantindo que o acesso, embora público, seja seguro e auditável.

## Camadas da Aplicação

### Domain (Domínio)

Camada central que contém as entidades de negócio, interfaces de repositório e regras de validação. Esta camada não possui dependências externas e representa o núcleo da aplicação.

**Responsabilidades:**

- Definição das entidades de negócio
- Interfaces de repositório
- Validações de domínio com FluentValidation

### Application (Aplicação)

Camada que contém os casos de uso e serviços da aplicação. Orquestra o fluxo de dados entre a camada de apresentação e a camada de domínio.

**Responsabilidades:**

- Serviços de aplicação
- DTOs (Data Transfer Objects)
- Mapeamentos com AutoMapper
- Coordenação de casos de uso

### Infrastructure (Infraestrutura)

Camada responsável pela implementação de persistência de dados e integrações externas.

**Responsabilidades:**

- Contexto do Entity Framework Core
- Implementação de repositórios
- Execução de Stored Procedures via Dapper
- Configurações de banco de dados

### API

Camada de apresentação que expõe endpoints RESTful para consumo.

**Responsabilidades:**

- Controllers REST
- Configuração de autenticação JWT
- Tratamento de exceções
- Documentação de API

### Web

Aplicação front-end MVC que consome a API.

**Responsabilidades:**

- Views Razor
- Controllers MVC
- Consumo da API via HttpClient
- Interface do usuário

## Padrões Implementados

### Repository Pattern

Abstração da camada de acesso a dados, permitindo a substituição da implementação sem impactar as camadas superiores.

### Dependency Injection

Inversão de controle para gerenciamento de dependências, facilitando testes e manutenção.

### DTO Pattern

Transferência de dados entre camadas sem expor as entidades de domínio diretamente.

### CQRS Simplificado

Separação de responsabilidades entre operações de leitura e escrita para otimização de performance.
