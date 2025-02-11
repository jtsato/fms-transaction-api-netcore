-- ============================================================================
-- SEQUENCES
-- ============================================================================
-- Utilizadas para gerar valores de auto-incremento em versões mais antigas do 
-- PostgreSQL que não suportam diretamente "BIGSERIAL" em estilo ANSI.
-- Se desejar, pode trocar por BIGSERIAL diretamente (sem CREATE SEQUENCE).

CREATE SEQUENCE transactions_sequences
    START WITH 1
    INCREMENT BY 1;

CREATE SEQUENCE transactions_audit_sequences
    START WITH 1
    INCREMENT BY 1;

-- ============================================================================
-- TABELA: transactions
-- ============================================================================
-- Equivale à classe Transaction (Transação Financeira).

CREATE TABLE transactions (
    id BIGINT NOT NULL DEFAULT nextval('transactions_sequences'),  -- Identificador único
    description VARCHAR(255) NOT NULL,                             -- Descrição
    amount DECIMAL(12,2) NOT NULL,                                 -- Valor, deve ser positivo
    type VARCHAR(10) NOT NULL,                                     -- 'DEBIT' ou 'CREDIT'
    "date" TIMESTAMP WITH TIME ZONE NOT NULL,                      -- Data/hora da transação
    status VARCHAR(10) NOT NULL,                                   -- 'ACTIVE' ou 'DELETED'
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,                  -- Data/hora de criação
    updated_at TIMESTAMP WITH TIME ZONE NOT NULL,                  -- Data/hora da última atualização
    
    -- Constraint de PK
    CONSTRAINT pk_transactions PRIMARY KEY (id),
    
    -- Constraint de valor positivo
    CONSTRAINT chk_transactions_amount CHECK (amount > 0),
    
    -- Constraint para validar tipo (enum)
    CONSTRAINT chk_transactions_type CHECK (type IN ('DEBIT','CREDIT')),
    
    -- Constraint para validar status (enum)
    CONSTRAINT chk_transactions_status CHECK (status IN ('ACTIVE','DELETED'))
);

-- ============================================================================
-- ÍNDICES para a tabela transactions
-- ============================================================================
-- Crie conforme a necessidade de desempenho e consultas frequentes.
-- Geralmente indexa-se campos usados em WHERE ou ORDER BY.

CREATE INDEX idx_transactions_date ON transactions ("date");
CREATE INDEX idx_transactions_type ON transactions (type);
CREATE INDEX idx_transactions_status ON transactions (status);

-- ============================================================================
-- TABELA: transaction_audit
-- ============================================================================
-- Equivale à classe TransactionAudit (Auditoria de Transações).

CREATE TABLE transaction_audit (
    id BIGINT NOT NULL 
        DEFAULT nextval('transactions_audit_sequences'),     -- Identificador único
    action VARCHAR(20) NOT NULL,                      -- 'INSERTED', 'MODIFIED', 'DELETED'
    oldDescription VARCHAR(255),                      -- Descrição anterior
    oldAmount DECIMAL(12,2),                          -- Valor anterior
    oldType VARCHAR(10),                              -- 'DEBIT' ou 'CREDIT'
    changeDate TIMESTAMP WITH TIME ZONE NOT NULL,     -- Data/hora da alteração
    
    -- Constraint de PK
    CONSTRAINT pk_transaction_audit PRIMARY KEY (id),
    
    -- Constraint para validar ação (enum)
    CONSTRAINT chk_transaction_audit_action CHECK (action IN ('INSERTED','MODIFIED','DELETED')),
    
    -- Constraint para validar tipo anterior (enum)
    CONSTRAINT chk_transaction_audit_oldType CHECK (oldType IN ('DEBIT','CREDIT') OR oldType IS NULL)
);

-- ============================================================================
-- ÍNDICE para a tabela transaction_audit
-- ============================================================================

CREATE INDEX idx_transaction_audit_change_date ON transaction_audit (changeDate);

-- Fim do script
