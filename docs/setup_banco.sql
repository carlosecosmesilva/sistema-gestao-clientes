/*
 * SCRIPT DE SETUP DO BANCO DE DADOS - SISTEMA GESTÃO DE CLIENTES
 * * Objetivo: Criar estrutura, índices de performance e Stored Procedures.
 * * Notas Importantes:
 * - Utilização de VARBINARY(MAX) para armazenamento de logotipos (Blob).
 * - Definição de Constraints de Unicidade para E-mail.
 * - Stored Procedures para operações de escrita (INSERT/UPDATE/DELETE).
 */
-- 1. Criação do Banco de Dados (Verifica se já existe)
IF NOT EXISTS(
    SELECT
        *
    FROM
        sys.databases
    WHERE
        name = 'SistemaGestaoDB'
) BEGIN CREATE DATABASE SistemaGestaoDB;

END
GO
    USE SistemaGestaoDB;

GO
    -- 2. Criação da Tabela Clientes
    IF NOT EXISTS (
        SELECT
            *
        FROM
            sys.objects
        WHERE
            object_id = OBJECT_ID(N'[dbo].[Clientes]')
            AND type in (N'U')
    ) BEGIN CREATE TABLE Clientes (
        Id INT IDENTITY(1, 1) PRIMARY KEY,
        Nome VARCHAR(100) NOT NULL,
        Email VARCHAR(100) NOT NULL,
        Logotipo VARBINARY(MAX) NULL,
        -- Armazenamento da imagem
        DataCadastro DATETIME DEFAULT GETDATE(),
        -- Garante que não existam dois clientes com mesmo e-mail (Regra de Negócio)
        CONSTRAINT UQ_Clientes_Email UNIQUE(Email)
    );

-- Índice não-clusterizado para acelerar buscas por nome (Performance)
CREATE INDEX IX_Clientes_Nome ON Clientes(Nome);

PRINT 'Tabela Clientes criada com sucesso.';

END
GO
    -- 3. Criação da Tabela Logradouros
    IF NOT EXISTS (
        SELECT
            *
        FROM
            sys.objects
        WHERE
            object_id = OBJECT_ID(N'[dbo].[Logradouros]')
            AND type in (N'U')
    ) BEGIN CREATE TABLE Logradouros (
        Id INT IDENTITY(1, 1) PRIMARY KEY,
        ClienteId INT NOT NULL,
        Endereco VARCHAR(200) NOT NULL,
        Complemento VARCHAR(100) NULL,
        Bairro VARCHAR(50) NOT NULL,
        Cidade VARCHAR(50) NOT NULL,
        Estado CHAR(2) NOT NULL,
        CEP VARCHAR(10) NOT NULL,
        -- Foreign Key com Delete Cascade: Se apagar o Cliente, seus endereços somem
        CONSTRAINT FK_Logradouros_Clientes FOREIGN KEY (ClienteId) REFERENCES Clientes(Id) ON DELETE CASCADE
    );

-- Índice para acelerar o JOIN entre Cliente e Logradouro (Performance)
CREATE INDEX IX_Logradouros_ClienteId ON Logradouros(ClienteId);

PRINT 'Tabela Logradouros criada com sucesso.';

END
GO
    -- ==========================================================================================
    -- STORED PROCEDURES (Requisito: Utilizar SPs para operações de escrita)
    -- ==========================================================================================
    -- SP: Adicionar Cliente
    -- Retorna o ID do novo cliente via SCOPE_IDENTITY()
    CREATE
    OR ALTER PROCEDURE sp_AdicionarCliente @Nome VARCHAR(100),
    @Email VARCHAR(100),
    @Logotipo VARBINARY(MAX),
    @Id INT OUTPUT AS BEGIN
SET
    NOCOUNT ON;

-- Melhora performance de rede evitando mensagens desnecessárias
INSERT INTO
    Clientes (Nome, Email, Logotipo)
VALUES
    (@Nome, @Email, @Logotipo);

SET
    @Id = SCOPE_IDENTITY();

END
GO
    -- SP: Atualizar Cliente
    -- Nota: Só atualiza o Logotipo se um novo valor for passado (tratamento de nulo)
    CREATE
    OR ALTER PROCEDURE sp_AtualizarCliente @Id INT,
    @Nome VARCHAR(100),
    @Email VARCHAR(100),
    @Logotipo VARBINARY(MAX) = NULL AS BEGIN
SET
    NOCOUNT ON;

UPDATE
    Clientes
SET
    Nome = @Nome,
    Email = @Email,
    -- Se @Logotipo for NULL, mantém o valor antigo. Se tiver bytes, atualiza.
    Logotipo = ISNULL(@Logotipo, Logotipo)
WHERE
    Id = @Id;

END
GO
    -- SP: Excluir Cliente
    -- O Cascade na FK removerá os logradouros automaticamente
    CREATE
    OR ALTER PROCEDURE sp_ExcluirCliente @Id INT AS BEGIN
SET
    NOCOUNT ON;

DELETE FROM
    Clientes
WHERE
    Id = @Id;

END
GO
    -- SP: Adicionar Logradouro (Caso precise adicionar individualmente)
    CREATE
    OR ALTER PROCEDURE sp_AdicionarLogradouro @ClienteId INT,
    @Endereco VARCHAR(200),
    @Complemento VARCHAR(100),
    @Bairro VARCHAR(50),
    @Cidade VARCHAR(50),
    @Estado CHAR(2),
    @CEP VARCHAR(10) AS BEGIN
SET
    NOCOUNT ON;

INSERT INTO
    Logradouros (
        ClienteId,
        Endereco,
        Complemento,
        Bairro,
        Cidade,
        Estado,
        CEP
    )
VALUES
    (
        @ClienteId,
        @Endereco,
        @Complemento,
        @Bairro,
        @Cidade,
        @Estado,
        @CEP
    );

END
GO
    -- ==========================================================================================
    -- Dados iniciais para teste
    -- ==========================================================================================
    IF NOT EXISTS(
        SELECT
            TOP 1 1
        FROM
            Clientes
    ) BEGIN
INSERT INTO
    Clientes (Nome, Email, Logotipo)
VALUES
    (
        'Empresa Teste S.A.',
        'contato@empresateste.com.br',
        NULL
    );

DECLARE @NewId INT = SCOPE_IDENTITY();

INSERT INTO
    Logradouros (ClienteId, Endereco, Bairro, Cidade, Estado, CEP)
VALUES
    (
        @NewId,
        'Av. Paulista, 1000',
        'Bela Vista',
        'São Paulo',
        'SP',
        '01310-100'
    );

PRINT 'Dados de teste inseridos com sucesso.';

END
GO