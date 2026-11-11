# Fluxo de Git

## Modelo: Git Flow

| Branch | Papel | Sai de | Volta para |
| --- | --- | --- | --- |
| `main` | Versão entregue; cada merge recebe uma tag (`v0.1.0`) | — | — |
| `develop` | Integração do que está em construção | `main` | — |
| `feature/` | Funcionalidade nova | `develop` | `develop` |
| `bugfix/` | Correção antes de publicar | `develop` | `develop` |
| `chore/` | Tarefa que não é funcionalidade: estrutura, documentação, dependências | `develop` | `develop` |
| `release/` | Fechar uma versão: CHANGELOG e ajustes finais | `develop` | `main` (com tag) e `develop` |
| `hotfix/` | Correção urgente numa versão entregue | `main` | `main` (com tag) e `develop` |

Os merges usam `--no-ff`, para que o histórico mostre onde cada funcionalidade começou e terminou.

## Nomes de branch

Formato da especificação **Conventional Branch**: `<tipo>/<descrição>`, com a convenção do projeto de prefixar a descrição com a etapa (`fN`, ver [planejamento](planejamento.md)).

- Só letras minúsculas, números e hífen; sem acento, espaço, sublinhado ou hífen duplo.
- Descrição curta e em português, dizendo o que a branch entrega.
- Releases e hotfixes usam a versão.

| Tipo | Exemplo |
| --- | --- |
| `chore/` | `chore/f0-escopo-e-padroes` |
| `feature/` | `feature/f2-regras-ecf` |
| `bugfix/` | `bugfix/f3-status-prescricao-inexistente` |
| `release/` | `release/0.2.0` |
| `hotfix/` | `hotfix/1.0.1` |

## Commits: Conventional Commits

Formato: `tipo(escopo): descrição no imperativo`, em português. O corpo, quando necessário, explica o porquê.

| Tipo | Quando | Exemplo |
| --- | --- | --- |
| `feat` | Funcionalidade nova | `feat(domain): regra ECF.10.01 de receita não estruturada` |
| `fix` | Correção | `fix(api): retorna 404 para prescrição inexistente` |
| `test` | Somente testes | `test(domain): duração e uso contínuo do item` |
| `docs` | Somente documentação | `docs: padrões de banco de dados` |
| `refactor` | Muda o código sem mudar o comportamento | `refactor(application): extrai validação de referências` |
| `build` | Build, dependências, Docker | `build(backend): Dockerfile multi-stage` |
| `ci` | Pipelines | `ci(frontend): build, artefato e deploy` |
| `chore` | Manutenção e releases | `chore(release): versão 0.2.0` |

Escopos usados: `domain`, `application`, `infrastructure`, `api`, `database`, `frontend`, `backend`.

Regras:
- Commits pequenos, cada um compilando e com os testes passando.
- Um commit, uma intenção.
- O autor do commit é quem o faz e o revisa. Quando houver apoio de IA, o commit leva no fim a linha:

  ```
  Co-Authored-By: Claude <noreply@anthropic.com>
  ```

## Versões: SemVer

`MAIOR.MENOR.CORREÇÃO`. Cada etapa do planejamento fecha uma versão `0.x.0`; a entrega completa é a `1.0.0`. Correções numa versão entregue sobem o último número (`1.0.1`). O [CHANGELOG](../CHANGELOG.md) segue o formato Keep a Changelog, com uma seção por versão.

## Gatilhos dos pipelines

Pipeline, testes, dependências e scripts do banco são versionados com o código: cada commit é uma foto completa do projeto, e o pipeline roda sobre o commit que recebeu o evento.

| Evento | O que roda |
| --- | --- |
| Push em `feature/**`, `bugfix/**`, `chore/**` | Build e testes |
| Pull request para `develop` ou `main` | Build e testes, obrigatórios para o merge |
| Push na `develop` | Build, testes e artefatos |
| Tag `v*` | Build, testes, artefatos, imagem com a versão e deploy |
| Manual (`workflow_dispatch`) | Reexecução sob demanda, escolhendo a branch ou a tag |

Cada aplicação tem seu pipeline e só dispara com mudanças na própria pasta (`backend/` e `database/`, ou `frontend/`).

## Referências

- Vincent Driessen, *A successful Git branching model* (2010) — origem do Git Flow.
- [Conventional Branch](https://shenxianpeng.github.io/en/posts/conventional-branch) e [Conventional Commits](https://www.conventionalcommits.org/pt-br/v1.0.0/).
- [Versionamento Semântico](https://semver.org/lang/pt-BR/) e [Keep a Changelog](https://keepachangelog.com/pt-BR/1.1.0/).
