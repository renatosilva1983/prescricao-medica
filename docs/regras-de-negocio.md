# Regras de negócio

Regras do padrão **SBIS** (Sociedade Brasileira de Informática em Saúde), documento ECF-PRESCRIÇÃO, adotadas na solução. Cada regra derivada tem um identificador, usado nas mensagens de erro da API e no nome dos testes.

Onde a regra é garantida:
- **Domínio:** é a fonte da regra; uma prescrição inválida não chega a existir.
- **Banco:** restrições `CHECK` repetem as principais, como última barreira (defesa em profundidade).
- **Front-end:** orienta o usuário antes de enviar, mas não substitui a validação da API.

## ECF.10.01 — Emissão de receita não estruturada

> O S-RES deve possuir uma funcionalidade para elaboração de receita de forma não estruturada (texto livre), de forma a permitir a prescrição de princípios ativos e medicamentos que não serão dispensados e/ou administrados na instituição.

**Interpretação:** uma prescrição do tipo não estruturada tem como conteúdo um texto livre, escrito pelo profissional.

| Id | Regra |
| --- | --- |
| R01.1 | O texto livre é obrigatório e não pode ser só espaços |
| R01.2 | O texto livre tem no máximo 4.000 caracteres |
| R01.3 | A receita não estruturada não tem itens estruturados |

## ECF.10.03 — Emissão de receita estruturada

> a) O S-RES deve possuir uma funcionalidade para elaboração de receita de forma estruturada [...]
> b) O S-RES deve permitir que o usuário busque por nome, a partir de uma lista de seleção, os princípios ativos e/ou medicamentos que serão prescritos. [...]
> c) O S-RES deve permitir a prescrição de princípios ativos e/ou medicamentos que não estejam cadastrados ou padronizados pela instituição (por exemplo, medicamentos manipulados não disponíveis na instituição).
> d) Para cada princípio ativo e/ou medicamento prescrito, o S-RES deve permitir a indicação estruturada de minimamente: nome; forma farmacêutica; dose e unidade de medida; frequência; início do tratamento; duração do tratamento (dias); indicação de uso contínuo; via de administração; prazo de utilização; observação em texto livre.

**Interpretação:** uma prescrição do tipo estruturada tem como conteúdo uma lista de itens, e cada item descreve um medicamento prescrito com os dados mínimos do item d.

| Id | Item | Regra |
| --- | --- | --- |
| R03.1 | a | A receita estruturada tem ao menos 1 e no máximo 30 itens |
| R03.2 | a | A receita estruturada não tem texto livre (o conteúdo são os itens) |
| R03.3 | b | O medicamento pode ser escolhido do cadastro, buscando por nome (`/odata/Medicamentos`) |
| R03.4 | c | O medicamento pode ser informado por nome, quando não estiver no cadastro (ex.: manipulado) |
| R03.5 | b, c | Cada item tem exatamente uma das duas origens: medicamento do cadastro **ou** nome livre |
| R03.6 | b | Um medicamento do cadastro informado precisa existir |
| R03.7 | d | Forma farmacêutica, dose, unidade da dose, frequência, via de administração e data de início são obrigatórios |
| R03.8 | d | A dose é maior que zero |
| R03.9 | d | Uso contínuo não tem duração; uso não contínuo tem duração de 1 a 365 dias |
| R03.10 | d | O prazo de utilização, quando informado, não é anterior ao início do tratamento |
| R03.11 | d | A observação é opcional, com no máximo 500 caracteres |

## Regras gerais da prescrição

| Id | Regra |
| --- | --- |
| RG.1 | Paciente obrigatório e existente no cadastro |
| RG.2 | Nome do profissional responsável e registro no conselho (ex.: `CRM-SP 123456`) obrigatórios |
| RG.3 | O paciente não muda numa revisão; para outro paciente, emite-se outra prescrição |
| RG.4 | A data de emissão é definida pelo sistema; a revisão registra a data da alteração |

## Respostas da API para regras violadas

| Situação | Status | Exemplo |
| --- | --- | --- |
| Formato inválido (tipo desconhecido, campo com tipo errado) | 400 | `tipo` diferente de `NaoEstruturada` ou `Estruturada` |
| Paciente ou prescrição inexistente | 404 | `GET /api/prescricoes/999` |
| Regra de negócio violada | 422 | Item com medicamento do cadastro e nome livre ao mesmo tempo |

As respostas seguem o formato ProblemDetails (RFC 9457) e, nas regras de negócio, trazem o código da regra:

```json
{
  "status": 422,
  "title": "Regra de negócio violada",
  "detail": "Informe um medicamento do cadastro ou o nome de um medicamento não cadastrado, e apenas um dos dois.",
  "regra": "ECF.10.03"
}
```
