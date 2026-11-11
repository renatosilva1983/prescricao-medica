# Decisões de arquitetura

Registro das decisões no formato ADR (*Architecture Decision Record*): contexto, decisão e consequências. Uma decisão nova entra com o próximo número; uma decisão revista ganha um ADR novo que a substitui.

## ADR-001 — Clean Architecture no back-end e no front-end

**Contexto:** o enunciado exige separação de responsabilidades, independência de frameworks e facilidade de testes, nos dois lados.

**Decisão:** quatro projetos no back (`Domain`, `Application`, `Infrastructure`, `Api`), com dependências apontando para o domínio; no front, camadas `domain`, `data`, `application` e `presentation` por funcionalidade.

**Consequências:** as regras de negócio são testadas sem banco nem HTTP; trocar o banco ou a forma de expor a API não muda as regras. Em troca, há mais projetos e pastas do que num CRUD simples.

## ADR-002 — Linguagem ubíqua em português, vocabulário técnico em inglês

**Contexto:** o domínio vem de um padrão brasileiro (SBIS), com termos próprios: receita estruturada, via de administração, uso contínuo.

**Decisão:** conceitos do domínio e casos de uso em português (`Prescricao`, `CriarPrescricao`), sem acentos nos identificadores; termos de arquitetura e padrões em inglês (`Repository`, `Handler`, `Domain`).

**Consequências:** o código usa as palavras do requisito, e quem conhece o domínio o lê sem traduzir. Os padrões técnicos continuam reconhecíveis para qualquer desenvolvedor.

## ADR-003 — Regras no domínio, repetidas no banco

**Contexto:** a clareza das regras de negócio é critério de avaliação.

**Decisão:** o agregado `Prescricao` garante as regras ECF.10.01 e ECF.10.03 e lança `RegraNegocioException` com o código da regra; a API a devolve como 422. As principais regras também viram `CHECK` no banco.

**Consequências:** nenhuma entrada (tela, API, script) cria uma prescrição inválida; o código da regra aparece no erro, no teste e no documento. A repetição no banco é intencional, como última barreira.

## ADR-004 — Banco criado por scripts SQL versionados

**Contexto:** o enunciado pede a modelagem e a implementação do DER, com dados pré-populados em MySQL.

**Decisão:** o esquema e os dados iniciais ficam em scripts SQL numerados (`database/`), executados pelo container do MySQL; o EF Core só mapeia as tabelas existentes, sem migrations.

**Consequências:** a modelagem fica explícita e revisável (chaves, restrições, índices e comentários); o mapeamento do EF precisa acompanhar o script. Mudanças de esquema entram como novos scripts, nunca editando os publicados.

## ADR-005 — Escopo nacional, sem amarrar o código ao Brasil

**Contexto:** as regras seguem o padrão SBIS e campos da regulamentação brasileira (conselho profissional com UF).

**Decisão:** o domínio segue o padrão brasileiro, mas o registro do profissional é texto livre (`CRM-SP 123456`), as datas são gravadas em UTC e unidades e vias são texto, sem tabelas fixas.

**Consequências:** a solução atende ao pedido sem custo extra; outros conselhos, fusos ou idiomas (por exemplo, com o i18n do Angular) podem entrar sem mudar o modelo.

## ADR-006 — CQRS com OData sobre DTOs de leitura

**Contexto:** a listagem precisa de consultas flexíveis (`$filter`, `$orderby`, `$select`), e a escrita precisa das regras do domínio.

**Decisão:** comandos passam pelo agregado; consultas usam `IQueryable` de DTOs expostos pela Application, sobre os quais a Api aplica o OData.

**Consequências:** o cliente consulta só os campos da listagem, nunca a entidade; o EF Core traduz filtro, ordenação e paginação numa única consulta SQL. A Application não conhece o OData.

## ADR-007 — Uma aplicação por pasta, cada uma com imagem e pipeline próprios

**Contexto:** a estrutura deve estar preparada para execução como microsserviço.

**Decisão:** `backend/` e `frontend/` independentes, cada um com sua solution, testes, Dockerfile e pipeline, disparado só por mudanças na própria pasta. A configuração vem de variáveis de ambiente.

**Consequências:** cada aplicação é publicada e escalada sozinha; separar em dois repositórios não exige mudanças internas. O `docker-compose.yml` da raiz junta tudo para desenvolvimento.
