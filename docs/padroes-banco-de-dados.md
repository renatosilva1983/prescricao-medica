# Padrões de banco de dados

Banco **MySQL 8.4**, criado por scripts SQL versionados (ver ADR-004). O modelo de dados (DER) fica em `docs/der.md`.

## Nomes

| Elemento | Padrão | Exemplo |
| --- | --- | --- |
| Tabelas | snake_case, singular, português | `paciente`, `prescricao`, `item_prescricao` |
| Colunas | snake_case, português, sem acento | `data_nascimento`, `via_administracao` |
| Chave primária | `id` | `prescricao.id` |
| Chave estrangeira (coluna) | `<tabela_referenciada>_id` | `prescricao.paciente_id` |
| Booleanos | afirmação | `uso_continuo` |
| Datas sem hora | `data_<evento>` | `data_inicio` |
| Data e hora (UTC) | `<evento>_em_utc` | `emitida_em_utc`, `atualizada_em_utc` |

### Nomes das restrições

Toda restrição tem nome explícito, para que uma mensagem de erro do banco diga qual regra falhou.

| Restrição | Padrão | Exemplo |
| --- | --- | --- |
| Chave primária | `pk_<tabela>` | `pk_prescricao` |
| Chave estrangeira | `fk_<tabela>_<referência>` | `fk_item_prescricao_medicamento` |
| Unicidade | `uq_<tabela>_<colunas>` | `uq_medicamento_nome_forma_concentracao` |
| Verificação | `ck_<tabela>_<regra>` | `ck_item_prescricao_duracao` |
| Índice | `ix_<tabela>_<colunas>` | `ix_prescricao_paciente_id` |

## Tipos

| Dado | Tipo | Observação |
| --- | --- | --- |
| Identificadores | `INT AUTO_INCREMENT` | Suficiente para o volume; legível nos dados de exemplo |
| Textos | `VARCHAR(n)` com tamanho definido | O mesmo limite é validado no domínio |
| Texto livre da receita | `VARCHAR(4000)` | Limite da regra R01.2 |
| Quantidades | `DECIMAL(10,3)` | Doses fracionadas sem erro de arredondamento |
| Data sem hora | `DATE` | Início do tratamento, prazo de utilização, nascimento |
| Data e hora | `DATETIME(6)` em UTC | Emissão e revisão |
| Booleano | `TINYINT(1)` | `uso_continuo` |
| Valores fechados | `VARCHAR` + `CHECK` | `tipo IN ('NaoEstruturada', 'Estruturada')`; legível e sem depender da ordem de um enum |

Charset `utf8mb4` com collation `utf8mb4_0900_ai_ci`: acentos corretos, e buscas sem diferenciar maiúsculas nem acentos ("dipirona" encontra "Dipirona").

## Integridade

- **Chave primária** em todas as tabelas e **chave estrangeira** em todas as relações.
- **Exclusão:** em cascata só de `prescricao` para `item_prescricao`, porque o item não existe sem a prescrição. Paciente e medicamento referenciados não podem ser excluídos.
- **`CHECK`** repete no banco as regras principais do domínio (defesa em profundidade):
  - texto livre só na receita não estruturada (ECF.10.01);
  - item com medicamento do cadastro **ou** nome livre, nunca os dois (ECF.10.03 b, c);
  - uso contínuo sem duração, ou duração de 1 a 365 dias (ECF.10.03 d);
  - dose maior que zero e prazo de utilização a partir do início.
- **Unicidade** onde o negócio exige: o mesmo medicamento, forma e concentração não se repetem no cadastro.
- **Índices** nas chaves estrangeiras e nas colunas usadas em filtros e ordenações (nome do medicamento, data de emissão).

## Normalização

O modelo está na **terceira forma normal**:
- **1FN:** sem listas dentro de uma coluna. Os medicamentos de uma receita são linhas de `item_prescricao`, não um texto separado por vírgulas.
- **2FN:** cada coluna depende da chave inteira da sua tabela.
- **3FN:** sem dependência entre colunas não chave. Os dados do medicamento (princípio ativo, concentração) ficam em `medicamento`; o item guarda o que pertence à prescrição (dose, frequência, via).

A relação entre prescrição e medicamento é N:N, resolvida por `item_prescricao`. A chave para `medicamento` é opcional, para permitir medicamentos fora do cadastro (ECF.10.03 c).

## Scripts

```
database/
├─ 01-esquema.sql        tabelas, chaves, restrições e índices
└─ 02-dados-iniciais.sql pacientes, medicamentos e prescrições de exemplo
```

- Executados em ordem alfabética pelo container do MySQL na primeira subida (`/docker-entrypoint-initdb.d`).
- **Script publicado não é editado.** Uma mudança de esquema entra como um novo script numerado (`03-...sql`), como numa migration: qualquer versão do projeto reconstrói o banco daquela versão.
- Dados iniciais com nomes fictícios.
- Credenciais só por variáveis de ambiente; os valores padrão do `docker-compose.yml` valem apenas para desenvolvimento local.
