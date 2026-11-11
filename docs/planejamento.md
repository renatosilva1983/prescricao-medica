# Planejamento

A solução é construída em etapas. Cada etapa tem um número (`fN`, usado nos nomes de branch) e, a partir da etapa 1, fecha uma versão publicada na `main` com tag. Toda versão é utilizável por si só: as etapas seguem o peso da avaliação, com a prescrição e as regras antes da interface.

| Etapa | Versão | Entrega |
| --- | --- | --- |
| f0 | — | Escopo, padrões e planejamento |
| f1 | `0.1.0` | Estrutura das solutions e banco de dados |
| f2 | `0.2.0` | Domínio e regras de negócio |
| f3 | `0.3.0` | API REST: CRUD da prescrição |
| f4 | `0.4.0` | Consultas OData |
| f5 | `0.5.0` | Front-end |
| f6 | `1.0.0` | Containers, pipelines e entrega |

## f0 — Escopo, padrões e planejamento

**Branch:** `chore/f0-escopo-e-padroes` (entra na `develop`, sem release)

- Estrutura inicial do repositório: `.gitignore`, `.gitattributes`, `.editorconfig`, modelo de pull request
- Documentos: escopo, regras de negócio, arquitetura, padrões de código, padrões de banco, fluxo de Git, planejamento e decisões iniciais

**Pronto quando:** os documentos descrevem o que será feito e como, e servem de referência para as etapas seguintes.

## f1 — Estrutura e banco de dados (`0.1.0`)

**Branches:** `chore/f1-estrutura-backend`, `feature/f1-modelo-de-dados`

- Solution do back com Domain, Application, Infrastructure, Api e testes; `Directory.Build.props` e `Directory.Packages.props`
- DER em `docs/der.md`
- Scripts `database/01-esquema.sql` e `database/02-dados-iniciais.sql`
- `docker-compose.yml` com o MySQL

**Pronto quando:** `docker compose up -d mysql` cria as tabelas com os dados iniciais e `dotnet build` passa.

## f2 — Domínio e regras de negócio (`0.2.0`)

**Branches:** `feature/f2-dominio-prescricao`, `feature/f2-regras-ecf`

- Entidades `Paciente` e `Medicamento` (somente leitura)
- Agregado `Prescricao` com `ItemPrescricao`
- Regras ECF.10.01, ECF.10.03 e gerais no domínio, com `RegraNegocioException` e o código da regra
- Testes unitários de cada regra de [regras-de-negocio.md](regras-de-negocio.md)

**Pronto quando:** cada regra tem teste do caso válido e do inválido, e todos passam.

## f3 — API REST (`0.3.0`)

**Branches:** `feature/f3-persistencia-mysql`, `feature/f3-casos-de-uso`, `feature/f3-api-rest`

- Mapeamento EF Core para as tabelas existentes, repositório e unidade de trabalho
- Casos de uso: criar, revisar, excluir e obter prescrição, com Result Pattern, validação de entrada e decorators
- `PrescricoesController` com GET, POST, PUT e DELETE; erros em ProblemDetails; `/health`; OpenAPI e Scalar
- Testes dos casos de uso; arquivo `.http` com exemplos, inclusive dos erros de regra

**Pronto quando:** o CRUD funciona contra o MySQL pelo `.http` ou pelo Scalar, e uma regra violada devolve 422 com o código ECF.

## f4 — Consultas OData (`0.4.0`)

**Branch:** `feature/f4-consultas-odata`

- Modelo EDM com `Prescricoes`, `Pacientes` e `Medicamentos`, sobre DTOs de leitura
- `$filter`, `$orderby`, `$select`, `$top`, `$skip` e `$count`, com limite de página
- Exemplos de consulta no README e no `.http`

**Pronto quando:** as consultas de exemplo funcionam e viram uma única consulta SQL.

## f5 — Front-end (`0.5.0`)

**Branches:** `chore/f5-estrutura-frontend`, `feature/f5-listagem`, `feature/f5-formulario`, `feature/f5-visualizacao`

- Projeto Angular 21 e solution do front; camadas `domain`, `data`, `application` e `presentation`
- Listagem com filtro, ordenação e paginação por OData
- Formulário de criação e edição para os dois tipos de receita, com busca de medicamento por nome e opção de medicamento fora do cadastro (ECF.10.03 b e c)
- Visualização da prescrição
- Testes do facade e das regras de tela

**Pronto quando:** o fluxo de listar, criar, editar e visualizar funciona pela tela contra a API local.

## f6 — Containers, pipelines e entrega (`1.0.0`)

**Branches:** `chore/f6-containers`, `chore/f6-pipeline-backend`, `chore/f6-pipeline-frontend`, `release/1.0.0`

- Dockerfile da API e do front (com nginx encaminhando `/api` e `/odata`); `docker-compose.yml` completo
- Pipelines YAML com build, testes, geração de artefatos e deploy, um por aplicação
- README final: como rodar, endpoints, exemplos de OData, regras implementadas e pipelines

**Pronto quando:** `docker compose up -d` sobe a solução inteira, os pipelines estão verdes e a `v1.0.0` está publicada.

## Se faltar tempo

Corta-se nesta ordem, sem tocar no que o enunciado exige:
1. Visualização como página separada (passa a ser o formulário em modo leitura)
2. OData em pacientes (fica em prescrições e medicamentos)
3. Testes dos casos de uso (ficam os do domínio, que cobrem as regras)

Nunca se corta: as regras ECF.10.01 e ECF.10.03 com testes, o CRUD completo, o OData na listagem, os dois Dockerfiles e os dois pipelines.
