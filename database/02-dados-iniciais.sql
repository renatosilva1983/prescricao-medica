-- =============================================================================
-- Dados iniciais: pacientes e medicamentos pré-populados (o foco da solução é a
-- prescrição) e prescrições de exemplo dos dois tipos. Nomes fictícios.
-- =============================================================================

SET NAMES utf8mb4;

INSERT INTO paciente (nome_completo, data_nascimento, sexo) VALUES
    ('Ana Beatriz Souza',     '1988-03-14', 'F'),
    ('Carlos Eduardo Lima',   '1975-11-02', 'M'),
    ('Mariana Alves Rocha',   '1992-07-25', 'F'),
    ('João Pedro Martins',    '2001-01-30', 'M'),
    ('Helena Costa Ribeiro',  '1959-09-09', 'F'),
    ('Rafael Gomes Ferreira', '1983-05-18', 'M'),
    ('Luiza Fernandes Dias',  '2015-12-03', 'F'),
    ('Paulo Henrique Nunes',  '1968-04-21', 'M');

INSERT INTO medicamento (nome, principio_ativo, forma_farmaceutica, concentracao) VALUES
    ('Dipirona Sódica',     'Dipirona monoidratada',       'Comprimido',                '500 mg'),
    ('Dipirona Sódica',     'Dipirona monoidratada',       'Solução oral (gotas)',      '500 mg/mL'),
    ('Paracetamol',         'Paracetamol',                 'Comprimido',                '750 mg'),
    ('Ibuprofeno',          'Ibuprofeno',                  'Comprimido revestido',      '600 mg'),
    ('Amoxicilina',         'Amoxicilina tri-hidratada',   'Cápsula',                   '500 mg'),
    ('Azitromicina',        'Azitromicina di-hidratada',   'Comprimido revestido',      '500 mg'),
    ('Losartana Potássica', 'Losartana potássica',         'Comprimido revestido',      '50 mg'),
    ('Metformina',          'Cloridrato de metformina',    'Comprimido',                '850 mg'),
    ('Sinvastatina',        'Sinvastatina',                'Comprimido revestido',      '20 mg'),
    ('Omeprazol',           'Omeprazol',                   'Cápsula',                   '20 mg'),
    ('Levotiroxina Sódica', 'Levotiroxina sódica',         'Comprimido',                '50 mcg'),
    ('Prednisona',          'Prednisona',                  'Comprimido',                '20 mg'),
    ('Salbutamol',          'Sulfato de salbutamol',       'Aerossol',                  '100 mcg/dose'),
    ('Cefalexina',          'Cefalexina monoidratada',     'Cápsula',                   '500 mg'),
    ('Loratadina',          'Loratadina',                  'Comprimido',                '10 mg'),
    ('Ondansetrona',        'Cloridrato de ondansetrona',  'Comprimido orodispersível', '8 mg');

-- 1) Receita não estruturada (ECF.10.01)
INSERT INTO prescricao (paciente_id, tipo, nome_profissional, registro_profissional, texto_livre, emitida_em_utc) VALUES
    (1, 'NaoEstruturada', 'Dra. Fernanda Moreira', 'CRM-SP 123456',
     'Repouso relativo por 3 dias. Hidratação oral abundante. Retornar se a febre persistir por mais de 48 horas.',
     '2026-11-03 13:20:00');

-- 2) Receita estruturada com um medicamento do cadastro e um manipulado (ECF.10.03 b e c)
INSERT INTO prescricao (paciente_id, tipo, nome_profissional, registro_profissional, texto_livre, emitida_em_utc) VALUES
    (2, 'Estruturada', 'Dr. Ricardo Tavares', 'CRM-SP 654321', NULL, '2026-11-05 17:45:00');

INSERT INTO item_prescricao
    (prescricao_id, medicamento_id, nome_medicamento_livre, forma_farmaceutica, dose_quantidade, dose_unidade,
     frequencia, via_administracao, data_inicio, duracao_dias, uso_continuo, data_limite_uso, observacao) VALUES
    (2, 5,    NULL,                              'Cápsula', 500, 'mg',        'De 8 em 8 horas', 'Oral',    '2026-11-05', 7,  0, NULL,         'Tomar após as refeições.'),
    (2, NULL, 'Creme de ureia 10% (manipulado)', 'Creme',   1,   'aplicação', '2 vezes ao dia',  'Tópica',  '2026-11-05', 15, 0, '2026-11-20', NULL);

-- 3) Receita estruturada de uso contínuo
INSERT INTO prescricao (paciente_id, tipo, nome_profissional, registro_profissional, texto_livre, emitida_em_utc) VALUES
    (5, 'Estruturada', 'Dra. Fernanda Moreira', 'CRM-SP 123456', NULL, '2026-11-07 12:10:00');

INSERT INTO item_prescricao
    (prescricao_id, medicamento_id, nome_medicamento_livre, forma_farmaceutica, dose_quantidade, dose_unidade,
     frequencia, via_administracao, data_inicio, duracao_dias, uso_continuo, data_limite_uso, observacao) VALUES
    (3, 7, NULL, 'Comprimido revestido', 50,  'mg', '1 vez ao dia',      'Oral', '2026-11-07', NULL, 1, NULL, 'Pela manhã.'),
    (3, 8, NULL, 'Comprimido',           850, 'mg', 'De 12 em 12 horas', 'Oral', '2026-11-07', NULL, 1, NULL, 'No café da manhã e no jantar.');
