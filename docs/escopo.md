# Escopo

Solução CRUD de prescrição médica com modelagem de dados, back-end, front-end e pipeline de entrega contínua.

## Requisitos e como serão atendidos

### Banco de dados

| Requisito | Como será atendido |
| --- | --- |
| DER com Paciente, Prescrição e Medicamento | DER em Mermaid em `docs/der.md`, com a tabela de itens da prescrição resolvendo a relação N:N entre prescrição e medicamento |
| Integridade referencial | Chaves estrangeiras em todas as relações; exclusão em cascata só dos itens de uma prescrição |
| Normalização adequada | Terceira forma normal: dados do medicamento ficam no cadastro; o item guarda só o que pertence à prescrição (dose, frequência, via...) |
| Refletir o domínio | Os dois tipos de receita do padrão SBIS e os dados mínimos do item (ECF.10.03 d) estão no modelo; restrições `CHECK` repetem as regras no banco |
| Paciente e Medicamento pré-populados em MySQL | Script de dados iniciais executado na primeira subida do container MySQL |

### Back-end — Web API

| Requisito | Como será atendido |
| --- | --- |
| Web API em .NET 9 | ASP.NET Core 9 com controllers (MVC) |
| CRUD completo da prescrição (GET, POST, PUT, DELETE) | `GET /api/prescricoes/{id}`, `POST /api/prescricoes`, `PUT /api/prescricoes/{id}`, `DELETE /api/prescricoes/{id}` |
| Clean Architecture (Domain, Application, Infrastructure, API) | Quatro projetos, com dependências apontando para o domínio |
| Independência de frameworks e facilidade de testes | Domínio sem dependências; casos de uso dependem de interfaces (portas) e são testados com dublês |
| Clean Code, SOLID, baixo acoplamento e alta coesão | Um caso de uso por classe; portas pequenas; injeção de dependência; decorators para validação e log |
| OData (filter, orderby, select...) | `/odata/Prescricoes`, `/odata/Pacientes` e `/odata/Medicamentos` com `$filter`, `$orderby`, `$select`, `$top`, `$skip` e `$count` |
| Dockerfile da aplicação | Imagem multi-stage, executada sem usuário root |
| Pipeline CI/CD com build, artefatos e deploy, em YAML | GitHub Actions em `.github/workflows/backend.yml` |
| Preparada para execução como microsserviço | Banco próprio, configuração por variáveis de ambiente, `/health`, imagem e pipeline independentes |
| Ao menos duas regras SBIS | ECF.10.01 (receita não estruturada) e ECF.10.03 (receita estruturada) — ver [regras de negócio](regras-de-negocio.md) |

### Front-end — Angular

| Requisito | Como será atendido |
| --- | --- |
| Angular 21 | Projeto criado com o Angular CLI 21: componentes standalone, signals e detecção de mudanças sem Zone.js |
| Listar, criar, editar e visualizar prescrições | Telas de listagem (com OData), formulário de criação e edição, e visualização |
| Clean Architecture no front | Camadas `domain`, `data`, `application` e `presentation` por funcionalidade |
| Dockerfile da aplicação | Build com Node e publicação com nginx |
| Pipeline CI/CD com build, artefatos e deploy, em YAML | GitHub Actions em `.github/workflows/frontend.yml` |

### Critérios de avaliação

| Critério | Onde se vê |
| --- | --- |
| Qualidade do código | Padrões em [padroes.md](padroes.md); análise no build; testes |
| Organização da arquitetura | [arquitetura.md](arquitetura.md) e a estrutura de pastas |
| Clareza das regras de negócio | [regras-de-negocio.md](regras-de-negocio.md); regras no domínio com o código SBIS nas mensagens e nos testes |
| Aderência às boas práticas | ADRs, padrões de banco, fluxo de Git e pipelines |
| Capacidade de entrega end-to-end | `docker compose up` sobe a solução; pipelines verdes; versões publicadas com tag |

## Fora do escopo

| Item | Motivo |
| --- | --- |
| Cadastro (CRUD) de pacientes e medicamentos | O enunciado permite tê-los pré-populados; o foco é a prescrição |
| Autenticação e perfis de usuário | Não pedido; o profissional responsável é informado na prescrição |
| Impressão da receita (ECF.10.04) e assinatura digital | Fora das regras escolhidas |
| Textos padrão (ECF.10.02) e medicamentos favoritos (ECF.10.10) | Fora das regras escolhidas; possíveis evoluções |
| Integração com outros sistemas ou padrões de interoperabilidade (HL7 FHIR) | Não pedido |
| Internacionalização | O domínio segue o padrão brasileiro SBIS; o código não fica preso a ele (ver ADR-005) |

Requisito de usabilidade atendido sem custo extra: a lista de seleção de medicamentos destaca linhas alternadas e a linha sob o cursor, como pede a ECF.10.07.

## Premissas

- O profissional responsável é identificado por nome e registro no conselho (ex.: `CRM-SP 123456`), informados na prescrição.
- O paciente de uma prescrição não muda depois de emitida: uma receita para outro paciente é outra receita.
- Uma prescrição pode ser revisada (PUT) inteira, inclusive trocando de tipo; a revisão passa pelas mesmas regras da emissão.
- Datas e horas são gravadas em UTC e exibidas no fuso do usuário.
