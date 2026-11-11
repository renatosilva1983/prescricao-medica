# Changelog

Formato baseado em [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/) e versionamento [SemVer](https://semver.org/lang/pt-BR/).

## [Não publicado]
### Adicionado
- Front-end Angular 21 (`frontend/prescricao-web`): listagem com filtro, ordenação e paginação por OData; formulário de criação e edição dos dois tipos de receita, com busca de medicamento e opção fora do cadastro; visualização; testes Vitest; solution do Visual Studio (`frontend/prescricao-web.slnx`).
- Dockerfile do front-end (nginx com proxy para a API), serviço `web` no `docker-compose.yml` e pipeline `frontend.yml` com testes, build, artefato e imagem.

## [0.5.0] - 2026-11-11
### Adicionado
- Dockerfile da API (build em duas etapas, usuário sem privilégios) e serviço `api` no `docker-compose.yml`.
- Pipeline `backend.yml` no GitHub Actions: build com avisos como erro, testes, artefato publicado, imagem no GitHub Container Registry e deploy de verificação com docker-compose e health check.

## [0.4.0] - 2026-11-11
### Adicionado
- Consultas OData em `/odata/Prescricoes`, `/odata/Pacientes` e `/odata/Medicamentos`, sobre DTOs de leitura: `$filter`, `$orderby`, `$select`, `$top`, `$skip` e `$count`, com página de no máximo 50 linhas.

## [0.3.0] - 2026-11-11
### Adicionado
- Persistência com EF Core e MySQL (Pomelo): mapeamento das tabelas existentes, repositórios, unidade de trabalho, conexão validada na partida e verificação do banco no `/health`.
- Casos de uso de emitir, revisar, excluir e visualizar prescrição: CQRS sem mediator, Result Pattern, validação de entrada com FluentValidation e decorators de validação e log; testes dos casos de uso.
- Regras R01.3 e R03.2 com código próprio, para recusar conteúdo do tipo errado de receita.
- API REST `/api/prescricoes` (GET, POST, PUT e DELETE): erros em ProblemDetails, com 422 e o código da regra quando uma regra de negócio é violada; OpenAPI e Scalar em desenvolvimento; arquivo `.http` com exemplos.

## [0.2.0] - 2026-11-11
### Adicionado
- Domínio da prescrição: agregado `Prescricao` com `ItemPrescricao`, entidades `Paciente` e `Medicamento`, e as regras ECF.10.01 (receita não estruturada) e ECF.10.03 (receita estruturada) com testes unitários.

## [0.1.0] - 2026-11-11
### Adicionado
- Escopo, regras de negócio, arquitetura, padrões de código e de banco de dados, fluxo de Git, planejamento e decisões de arquitetura.
- Solution do back-end em .NET 9 com os projetos Domain, Application, Infrastructure, Api e testes unitários; configurações comuns e versões de pacotes centralizadas.
- Modelo de dados (DER) e scripts do MySQL: esquema com chaves, restrições e índices, e dados iniciais de pacientes, medicamentos e prescrições de exemplo.
- `docker-compose.yml` com o MySQL 8.4.
