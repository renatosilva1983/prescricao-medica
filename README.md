# Prescrição Médica

Solução de **prescrição médica eletrônica** com back-end em **.NET 9**, front-end em **Angular 21** e banco **MySQL**, seguindo o padrão de requisitos da **SBIS** para emissão de receitas (ECF.10.01 e ECF.10.03).

O foco é a prescrição: emissão, revisão, consulta e exclusão de receitas **não estruturadas** (texto livre) e **estruturadas** (itens com medicamento, dose, frequência, via e duração). Pacientes e medicamentos vêm pré-cadastrados no banco.

> **Situação:** back-end completo (API REST, OData, regras SBIS, testes, Docker e pipeline). O front-end Angular é a próxima etapa (ver [Pendências](#pendências)).

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

### Com Docker (banco e API)

```powershell
docker compose up -d --build                                # API em http://localhost:5080
```

A porta do host pode ser trocada pela variável `API_PORT` (no Windows, a 8080 costuma estar reservada).

## Entrega contínua

O pipeline [`backend.yml`](.github/workflows/backend.yml) roda a cada push em `develop`, `main` ou tag `v*`:

1. **Build** com avisos tratados como erro e **testes** (resultado guardado como artefato).
2. **Artefato** da API publicada.
3. **Imagem Docker** no GitHub Container Registry (`ghcr.io/<repositório>/api`), com tags do commit, da branch e da versão.
4. **Deploy de verificação:** sobe banco e API com o `docker-compose` no runner e testa o `/health` e uma consulta OData. O alvo definitivo (nuvem) entra trocando só este job.

## Pendências

| Item | Situação |
| --- | --- |
| Front-end Angular 21 (lista, criação, edição, visualização), com Dockerfile e pipeline próprios | Planejado na etapa f5 de [docs/planejamento.md](docs/planejamento.md); não entrou no prazo desta entrega |
| Alvo de deploy definitivo | A definir; o pipeline já entrega a imagem pronta para publicar |
