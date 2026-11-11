# Changelog

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e versionamento [SemVer](https://semver.org/lang/pt-BR/).

## [Não publicado]
### Adicionado
- Persistência com EF Core e MySQL (Pomelo): mapeamento das tabelas existentes, repositórios, unidade de trabalho, conexão validada na partida e verificação do banco no `/health`.

## [0.2.0] - 2026-11-11
### Adicionado
- Domínio da prescrição: agregado `Prescricao` com `ItemPrescricao`, entidades `Paciente` e `Medicamento`, e as regras ECF.10.01 (receita não estruturada) e ECF.10.03 (receita estruturada) com testes unitários.

## [0.1.0] - 2026-11-11
### Adicionado
- Escopo, regras de negócio, arquitetura, padrões de código e de banco de dados, fluxo de Git, planejamento e decisões de arquitetura.
- Solution do back-end em .NET 9 com os projetos Domain, Application, Infrastructure, Api e testes unitários; configurações comuns e versões de pacotes centralizadas.
- Modelo de dados (DER) e scripts do MySQL: esquema com chaves, restrições e índices, e dados iniciais de pacientes, medicamentos e prescrições de exemplo.
- `docker-compose.yml` com o MySQL 8.4.
