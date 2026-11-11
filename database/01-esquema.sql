-- =============================================================================
-- Prescrição médica: esquema do banco (MySQL 8.4)
-- Modelo descrito em docs/der.md; padrões em docs/padroes-banco-de-dados.md.
-- Executado pelo container do MySQL na primeira subida (docker-entrypoint-initdb.d).
--
--   paciente 1 ── N prescricao 1 ── N item_prescricao N ── 0..1 medicamento
--
-- Os CHECKs repetem no banco as principais regras do domínio (defesa em
-- profundidade). A fonte das regras continua sendo o domínio da aplicação.
-- =============================================================================

SET NAMES utf8mb4;

CREATE TABLE paciente (
    id              INT          NOT NULL AUTO_INCREMENT,
    nome_completo   VARCHAR(150) NOT NULL,
    data_nascimento DATE         NOT NULL,
    sexo            CHAR(1)      NOT NULL COMMENT 'F = feminino, M = masculino, I = indeterminado',
    CONSTRAINT pk_paciente PRIMARY KEY (id),
    CONSTRAINT ck_paciente_sexo CHECK (sexo IN ('F', 'M', 'I')),
    INDEX ix_paciente_nome_completo (nome_completo)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci
  COMMENT = 'Pacientes (cadastro pré-populado)';

CREATE TABLE medicamento (
    id                  INT          NOT NULL AUTO_INCREMENT,
    nome                VARCHAR(150) NOT NULL,
    principio_ativo     VARCHAR(150) NOT NULL,
    forma_farmaceutica  VARCHAR(60)  NOT NULL,
    concentracao        VARCHAR(60)  NOT NULL,
    CONSTRAINT pk_medicamento PRIMARY KEY (id),
    CONSTRAINT uq_medicamento_nome_forma_concentracao UNIQUE (nome, forma_farmaceutica, concentracao),
    INDEX ix_medicamento_nome (nome),
    INDEX ix_medicamento_principio_ativo (principio_ativo)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci
  COMMENT = 'Medicamentos da instituição: lista de seleção da receita estruturada (ECF.10.03 b)';

CREATE TABLE prescricao (
    id                     INT           NOT NULL AUTO_INCREMENT,
    paciente_id            INT           NOT NULL,
    tipo                   VARCHAR(20)   NOT NULL COMMENT 'NaoEstruturada (ECF.10.01) ou Estruturada (ECF.10.03)',
    nome_profissional      VARCHAR(150)  NOT NULL,
    registro_profissional  VARCHAR(30)   NOT NULL COMMENT 'Conselho, número e UF. Ex.: CRM-SP 123456',
    texto_livre            VARCHAR(4000) NULL     COMMENT 'Conteúdo da receita não estruturada (ECF.10.01)',
    emitida_em_utc         DATETIME(6)   NOT NULL,
    atualizada_em_utc      DATETIME(6)   NULL,
    CONSTRAINT pk_prescricao PRIMARY KEY (id),
    CONSTRAINT fk_prescricao_paciente FOREIGN KEY (paciente_id) REFERENCES paciente (id),
    CONSTRAINT ck_prescricao_tipo CHECK (tipo IN ('NaoEstruturada', 'Estruturada')),
    -- ECF.10.01: texto livre só (e sempre) na receita não estruturada.
    CONSTRAINT ck_prescricao_texto_livre CHECK (
        (tipo = 'NaoEstruturada' AND texto_livre IS NOT NULL) OR
        (tipo = 'Estruturada' AND texto_livre IS NULL)),
    INDEX ix_prescricao_paciente_id (paciente_id),
    INDEX ix_prescricao_emitida_em_utc (emitida_em_utc)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci
  COMMENT = 'Prescrições médicas';

CREATE TABLE item_prescricao (
    id                      INT           NOT NULL AUTO_INCREMENT,
    prescricao_id           INT           NOT NULL,
    medicamento_id          INT           NULL     COMMENT 'Medicamento do cadastro (ECF.10.03 b)',
    nome_medicamento_livre  VARCHAR(150)  NULL     COMMENT 'Medicamento fora do cadastro, ex.: manipulado (ECF.10.03 c)',
    forma_farmaceutica      VARCHAR(60)   NOT NULL,
    dose_quantidade         DECIMAL(10,3) NOT NULL,
    dose_unidade            VARCHAR(20)   NOT NULL,
    frequencia              VARCHAR(60)   NOT NULL,
    via_administracao       VARCHAR(40)   NOT NULL,
    data_inicio             DATE          NOT NULL,
    duracao_dias            INT           NULL,
    uso_continuo            TINYINT(1)    NOT NULL,
    data_limite_uso         DATE          NULL     COMMENT 'Prazo de utilização',
    observacao              VARCHAR(500)  NULL,
    CONSTRAINT pk_item_prescricao PRIMARY KEY (id),
    CONSTRAINT fk_item_prescricao_prescricao FOREIGN KEY (prescricao_id)
        REFERENCES prescricao (id) ON DELETE CASCADE,
    CONSTRAINT fk_item_prescricao_medicamento FOREIGN KEY (medicamento_id) REFERENCES medicamento (id),
    -- ECF.10.03 b/c: medicamento do cadastro OU nome livre, exatamente um dos dois.
    CONSTRAINT ck_item_prescricao_medicamento CHECK (
        (medicamento_id IS NULL) <> (nome_medicamento_livre IS NULL)),
    -- ECF.10.03 d: uso contínuo sem duração; uso não contínuo com duração de 1 a 365 dias.
    CONSTRAINT ck_item_prescricao_duracao CHECK (
        (uso_continuo = 1 AND duracao_dias IS NULL) OR
        (uso_continuo = 0 AND duracao_dias BETWEEN 1 AND 365)),
    CONSTRAINT ck_item_prescricao_dose CHECK (dose_quantidade > 0),
    CONSTRAINT ck_item_prescricao_data_limite_uso CHECK (data_limite_uso IS NULL OR data_limite_uso >= data_inicio),
    INDEX ix_item_prescricao_prescricao_id (prescricao_id),
    INDEX ix_item_prescricao_medicamento_id (medicamento_id)
) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci
  COMMENT = 'Itens da receita estruturada (ECF.10.03): resolve o N:N entre prescrição e medicamento';
