# Padrões de código

## Nomenclatura

O código usa a **linguagem ubíqua** do domínio em português, com os mesmos termos do padrão SBIS, e mantém em inglês o vocabulário técnico de arquitetura e padrões de projeto (ver ADR-002).

| O quê | Idioma | Exemplos |
| --- | --- | --- |
| Conceitos do domínio | Português | `Prescricao`, `ItemPrescricao`, `Paciente`, `Medicamento`, `ViaAdministracao`, `UsoContinuo` |
| Casos de uso | Português | `CriarPrescricao`, `RevisarPrescricao`, `ExcluirPrescricao`, `ObterPrescricao` |
| Camadas e padrões | Inglês | `Domain`, `Application`, `Infrastructure`, `Api`, `Repository`, `Handler`, `Controller`, `Dto`, `Command`, `Query` |
| Combinação dos dois | Conceito + padrão | `IPrescricaoRepository`, `CriarPrescricaoHandler`, `PrescricoesController`, `PrescricaoDto` |
| Mensagens ao usuário | Português, com acentos | `"A dose deve ser maior que zero."` |

Regras:
- **Sem acento nem cedilha em identificadores:** `Prescricao`, não `Prescrição`.
- **Consistência:** um conceito tem um nome só em todo o código, no banco e na API.
- **Nomes que revelam a intenção:** `CriarEstruturada`, e não `Create2`; `UsoContinuo`, e não `Flag`.

### Convenções do C#

| Elemento | Convenção | Exemplo |
| --- | --- | --- |
| Classes, records, métodos, propriedades | PascalCase | `Prescricao.CriarEstruturada` |
| Interfaces | `I` + PascalCase | `IPrescricaoRepository` |
| Parâmetros e variáveis | camelCase | `pacienteId` |
| Campos privados | `_` + camelCase | `_itens` |
| Constantes | PascalCase | `TextoLivreTamanhoMaximo` |
| Métodos assíncronos | sufixo `Async` | `ObterPorIdAsync` |
| Namespaces | por arquivo (`namespace X;`) | `namespace PrescricaoMedica.Domain.Prescricoes;` |

### Convenções do Angular

| Elemento | Convenção | Exemplo |
| --- | --- | --- |
| Arquivos e pastas | kebab-case, sem sufixo de tipo (padrão do Angular 20+) | `prescricao-form.ts`, `prescricoes-api.ts` |
| Classes e tipos | PascalCase | `PrescricaoForm`, `Prescricao` |
| Signals e propriedades | camelCase | `prescricoes`, `carregando` |
| Seletores de componente | prefixo `app-` | `app-prescricao-lista` |

## Padrões de projeto

| Padrão | Onde | Para quê |
| --- | --- | --- |
| Agregado e factory methods (DDD) | `Prescricao.CriarNaoEstruturada`, `Prescricao.CriarEstruturada` | Uma prescrição só existe em estado válido; os itens só mudam pela raiz |
| CQRS sem biblioteca de mediator | `ICommandHandler<TCommand, TResult>`, `IQueryHandler<TQuery, TResult>` | Escrita pelo domínio, leitura otimizada; um handler por caso de uso |
| Result Pattern | `Result`, `Result<T>`, `Error` | Erros esperados (não encontrado, entrada inválida) como valor, sem exceção |
| Exceção de regra de negócio | `RegraNegocioException` com o código da regra | Invariante do domínio violada; vira 422 com o código SBIS |
| Decorator | Validação (FluentValidation) e log envolvendo os handlers | Comportamentos comuns sem repetir código |
| Repository e Unit of Work | `IPrescricaoRepository`; o DbContext como unidade de trabalho | A Application não conhece EF Core |
| Options Pattern | Configurações tipadas e validadas na subida | Erro de configuração aparece ao iniciar, não na primeira requisição |
| Facade (front) | `PrescricaoFacade` com signals | O componente não conhece HTTP nem estado |
| Smart e dumb components (front) | Páginas × componentes de exibição | Componentes de exibição reaproveitáveis e fáceis de testar |

## Regras de código

- Um caso de uso por classe; métodos curtos, com um nível de abstração.
- Sem números ou textos mágicos: constantes com nome (`ItensMaximo = 30`).
- Validação em duas camadas: **formato** na Application (FluentValidation), **regra de negócio** no domínio.
- Assíncrono de ponta a ponta, com `CancellationToken` repassado; nunca `.Result` ou `.Wait()`.
- Nenhuma regra de negócio no controller: ele traduz HTTP em comando e resultado em status.
- A API nunca expõe entidades: entrada e saída por DTOs.
- Comentários explicam o porquê, não o quê.
- O build trata avisos como erros e aplica as regras do `.editorconfig`.

## API

| Item | Padrão |
| --- | --- |
| Rotas REST | Substantivo no plural, minúsculo: `/api/prescricoes`, `/api/prescricoes/{id}` |
| Rotas OData | Conjuntos de entidades em PascalCase: `/odata/Prescricoes`, `/odata/Pacientes`, `/odata/Medicamentos` |
| JSON | camelCase; enums como texto (`"tipo": "Estruturada"`) |
| Datas | ISO 8601; data e hora em UTC (`2026-11-10T22:00:00Z`); datas sem hora como `2026-11-10` |
| Status | 200 leitura, 201 criação com `Location`, 204 revisão e exclusão, 400 entrada inválida, 404 inexistente, 422 regra de negócio |
| Erros | ProblemDetails (RFC 9457), com o campo `regra` nas violações de regra de negócio |
| Documentação | OpenAPI gerado pelo .NET e Scalar em desenvolvimento; arquivos `.http` com exemplos |

## Testes

| Camada | Ferramentas | O que se testa |
| --- | --- | --- |
| Domínio (back) | xUnit + Shouldly | Cada regra de [regras-de-negocio.md](regras-de-negocio.md): o caso válido e o inválido |
| Casos de uso (back) | xUnit + NSubstitute + Shouldly | Orquestração com as portas substituídas por dublês |
| Front-end | Vitest (padrão do Angular 21) | Facade, regras de tela e componentes principais |

- Nome do teste em três partes: `Metodo_Cenario_ResultadoEsperado`, por exemplo `CriarEstruturada_SemItens_LancaRegraEcf1003`.
- Estrutura Arrange, Act, Assert.
- Testes do back em projeto separado, na solution do back; testes do front ao lado do arquivo testado (`*.spec.ts`), como é o padrão do Angular.
- Testes de integração com MySQL, se houver, ficam num projeto próprio (`PrescricaoMedica.IntegrationTests`), por serem mais lentos e dependerem de banco.
