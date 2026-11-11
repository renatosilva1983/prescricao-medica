# Prescrição Médica

Solução de **prescrição médica eletrônica** com back-end em **.NET 9**, front-end em **Angular 21** e banco **MySQL**, seguindo o padrão de requisitos da **SBIS** para emissão de receitas (ECF.10.01 e ECF.10.03).

O foco é a prescrição: emissão, revisão, consulta e exclusão de receitas **não estruturadas** (texto livre) e **estruturadas** (itens com medicamento, dose, frequência, via e duração). Pacientes e medicamentos vêm pré-cadastrados no banco.

> **Situação:** API REST da prescrição pronta. As próximas etapas (OData, front-end e entrega) estão em [docs/planejamento.md](docs/planejamento.md).

## Documentação

| Documento | Conteúdo |
| --- | --- |
| [Escopo](docs/escopo.md) | Requisitos da avaliação, como cada um será atendido e o que fica fora |
| [Regras de negócio](docs/regras-de-negocio.md) | ECF.10.01 e ECF.10.03: requisito, interpretação e regras derivadas |
| [Arquitetura](docs/arquitetura.md) | Camadas, estrutura de pastas, fluxo de uma requisição, OData e entrega |
| [Padrões de código](docs/padroes.md) | Nomenclatura, padrões de projeto, API, front-end e testes |
| [Padrões de banco de dados](docs/padroes-banco-de-dados.md) | Nomes, chaves, restrições, tipos e scripts versionados |
| [Fluxo de Git](docs/fluxo-git.md) | Git Flow, nomes de branch, commits, versões e gatilhos do pipeline |
| [Planejamento](docs/planejamento.md) | Etapas, versões, entregáveis e critérios de pronto |
| [Decisões de arquitetura](docs/decisoes-arquitetura.md) | ADRs: o porquê de cada escolha |

## Estrutura do repositório

```
prescricao-medica/
├─ backend/      API .NET 9 (solution própria) e testes
├─ frontend/     Angular 21 (solution própria), testes ao lado do código
├─ database/     Scripts do banco: esquema e dados iniciais
├─ docs/         Documentação
├─ .github/      Pipelines (um por aplicação) e modelo de pull request
└─ docker-compose.yml
```

## Como rodar

Pré-requisitos: Docker e o SDK do .NET 9 (ou mais novo).

```powershell
docker compose up -d mysql                                  # banco com as tabelas e os dados iniciais
dotnet run --project backend/src/PrescricaoMedica.Api       # API em http://localhost:5180
dotnet test backend/PrescricaoMedica.slnx                   # testes do back-end
```

| O quê | Onde |
| --- | --- |
| Documentação interativa (Scalar) | http://localhost:5180/scalar |
| Saúde da API e do banco | http://localhost:5180/health |
| Exemplos de requisição, inclusive de erros | [`PrescricaoMedica.Api.http`](backend/src/PrescricaoMedica.Api/PrescricaoMedica.Api.http) |

Na versão `1.0.0`, `docker compose up -d` sobe banco, API e front-end.
