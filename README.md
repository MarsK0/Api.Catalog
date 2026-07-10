# Api.Catalog

> ⚠️ **Status: em desenvolvimento — projeto não funcional no momento.**
> Este repositório contém a estrutura inicial da solução. Ainda não há build/execução estáveis e endpoints completos. Este README documenta o **objetivo e a arquitetura pretendida** do projeto, servindo como guia para o desenvolvimento contínuo.

## Sobre o projeto

**Api.Catalog** é uma API para **catálogo de produtos** e **gestão de orçamentos/pedidos**, construída com suporte nativo a **multi-tenancy**.

O diferencial do projeto é o modelo de **usuários compartilhados entre tenants**: um mesmo usuário (mesma identidade/login) pode estar vinculado a múltiplos tenants (empresas/organizações), mas com:

- **Isolamento total de contexto de dados** entre tenants (um tenant nunca acessa dados de outro);
- **Papéis (roles/permissões) independentes por tenant** — o mesmo usuário pode ser, por exemplo, administrador em um tenant e apenas vendedor/consultor em outro;
- Vínculo do usuário ao tenant feito por meio de **cadastro/associação explícita**, e não por criação automática de conta duplicada.

## Objetivos do domínio

- **Catálogo de produtos**: cadastro, categorização e consulta de produtos por tenant.
- **Orçamentos**: criação e acompanhamento de propostas/orçamentos vinculados a clientes e produtos do catálogo.
- **Pedidos**: conversão de orçamentos aprovados em pedidos, com controle de status.
- **Multi-tenancy**: cada tenant opera de forma isolada (dados, catálogo, usuários e permissões próprios).
- **Usuários compartilhados**: uma única identidade de usuário pode participar de vários tenants, com papel e permissões específicos em cada um.
- **Platform**: administração global do projeto (fora do conceito de tenant), responsável por cadastro de tenants, liberação de módulos/planos e suporte.
- **Planos e módulos**: cada tenant terá acesso apenas aos módulos liberados pelo seu plano (conceito ainda não definido em detalhes).

> As regras de negócio detalhadas (fluxo de aprovação de orçamento, transições de status de pedido, etc.) ainda estão em definição e serão documentadas conforme forem implementadas.

## Arquitetura

O projeto segue os princípios de **Clean Architecture** e de **Domain Driven Design**, separando responsabilidades em camadas independentes:

```
Api.Catalog
├── Api.Catalog.Api              # Camada de apresentação (Web API, controllers, configuração de startup)
├── Api.Catalog.Application      # Casos de uso, regras de aplicação, DTOs, orquestração
├── Api.Catalog.Domain           # Entidades, regras de negócio, interfaces de domínio (núcleo da aplicação)
├── Api.Catalog.Infrastructure   # Persistência, integrações externas, implementação de repositórios
└── Api.Catalog.slnx             # Arquivo de solução
```

### Camadas

| Camada | Responsabilidade |
|---|---|
| **Api** | Exposição de endpoints HTTP, autenticação/autorização, middlewares (incluindo resolução do tenant da requisição), documentação da API |
| **Application** | Orquestração dos casos de uso (produtos, orçamentos, pedidos, tenants, usuários), validações de aplicação, contratos (interfaces) consumidos pela Api |
| **Domain** | Entidades e agregados (Produto, Orçamento, Pedido, Tenant, Usuário, Vínculo Usuário-Tenant/Papel), regras de negócio, validações de domínio |
| **Infrastructure** | Acesso a dados, mecanismo de aplicação do filtro de tenant (multi-tenancy), repositórios, serviços externos |

A regra de dependência segue o padrão de Clean Architecture: `Api → Application → Domain`, com `Infrastructure` implementando as interfaces definidas em `Domain`/`Application`.

### Tratamento de fluxo: Result Pattern (`AppResult`)

O projeto adota o **Result Pattern**  em vez de usar `exceptions` para controle de fluxo.

Na prática:

- Casos de uso (`Application`) e regras de domínio (`Domain`) que podem falhar de forma **previsível** (validação de negócio, regra não atendida, recurso não encontrado, acesso negado, etc.) devem **retornar um `AppResult`** indicando sucesso/falha, em vez de lançar uma exception.
- `Exceptions` ficam reservadas para situações **verdadeiramente excepcionais/inesperadas** (falha de infraestrutura, bug, estado inválido não previsto), e não para representar regras de negócio.
- Esse padrão evita o uso de `exceptions` como controle de fluxo (custo de performance, stack trace desnecessário, fluxo implícito) e torna explícito, na assinatura dos métodos, quais operações podem falhar e por quê.
- A camada `Api` é responsável por traduzir um `AppResult` de falha no código de status HTTP e formato de resposta apropriados (ex.: 400, 404, 403, 422), centralizando essa tradução, em vez de espalhar esse mapeamento pelos controllers.

## Multi-tenancy e isolamento de usuários

O modelo conceitual prevê:

- Um **Tenant** representa uma organização/empresa independente dentro do sistema, identificado por um **slug** único.
- Um **Usuário** é uma identidade global — o login/JWT identifica a pessoa, não um tenant específico.
- Um **Vínculo Usuário-Tenant** associa um usuário a um tenant específico, contendo o(s) **papel(éis)/permissões** daquele usuário *naquele* tenant.
- Toda consulta/operação de dados de negócio (produtos, orçamentos, pedidos) é **filtrada pelo tenant do contexto da requisição**, garantindo que dados de um tenant nunca vazem para outro.

### Fluxo de autenticação e contexto de tenant

> ⚠️ Estratégia em fase de refatoração — pode sofrer ajustes.

1. **Login**: o usuário se autentica uma única vez. O **JWT** carrega apenas a **identidade do usuário**, sem informação de tenant/role — o token é o mesmo independentemente de qual tenant será acessado depois.
2. **Contexto de tenant pela URL**: o tenant atual é definido pelo **slug na URL** do front-end (ex.: `/{tenant-slug}/...`).
3. **Resolução de contexto (`/me`)**: ao navegar para um tenant, o front-end dispara uma chamada a um endpoint `/me` (com o slug do tenant atual), que:
   - verifica se o usuário autenticado tem acesso àquele tenant;
   - retorna as **roles e permissões** do usuário *naquele* tenant específico.
4. O front-end guarda esse retorno em um **contexto** que engloba as rotas daquele tenant, controlando o que é exibido/habilitado na UI.
5. **Troca de tenant**: ao navegar para outro tenant (outro slug), o mesmo fluxo se repete — uma nova chamada a `/me` revalida o acesso e atualiza (ou bloqueia) o contexto, sem exigir novo login.
6. **Autorização real na API**: a UI serve apenas para experiência (esconder/mostrar opções); a autorização de fato acontece no backend. A cada requisição, a API identifica o **usuário (via JWT)** e o **tenant (via rota/slug)** e verifica as permissões daquele par usuário+tenant antes de processar a operação.
7. **Cache de permissões**: as permissões associadas às roles ficam **em cache no backend**, evitando releitura constante do banco. A cada requisição, a API usa esse cache para validar se o perfil do usuário naquele tenant tem a permissão necessária para a rota acessada.

Esse modelo garante que:
- o mesmo usuário pode ter papéis diferentes em cada tenant ao qual pertence;
- a troca de tenant não exige novo login;
- a validação de acesso é sempre feita no backend, mesmo que o front-end já filtre a navegação por conveniência.

### "Platform" (administração global)

Além dos tenants comuns, o projeto prevê um nível de administração acima, chamado de **"platform"**, que possui contexto próprio, à parte, destinado à administração do projeto como um todo, incluindo (entre outras responsabilidades ainda a definir):

- cadastro e gestão de tenants;
- liberação/gestão de **planos** e **módulos** por tenant (ver seção abaixo);
- suporte e operações administrativas gerais.

> Detalhes de como o acesso à "platform" será autenticado/autorizado (se via roles especiais, usuários dedicados, etc.) ainda estão em definição.

### Planos e módulos (conceito previsto, ainda não definido)

O projeto também prevê o conceito de **planos**, que determinam quais **módulos** cada tenant tem liberados. Ou seja, um tenant só tem acesso às funcionalidades (módulos) contempladas pelo plano contratado/atribuído a ele.

> Esse conceito ainda não está definido em detalhes (como planos e módulos serão modelados, como a liberação será verificada em runtime, etc.) — será detalhado conforme a implementação avançar.

## Tecnologias

- **C# .NET 10.0**
- **ASP.NET Core Web API** (camada `Api.Catalog.Api`)
- **PostgreSQL** para persistência de dados
- **Entity Framework Core** como ORM (camada `Api.Catalog.Infrastructure`)
- **JWT** para autenticação de identidade
- **Result Pattern (`AppResult`)** para tratamento de fluxo
- Cache de permissões/roles por tenant no backend (mecanismo específico a definir)

> A estratégia de autenticação/autorização multi-tenant está atualmente em refatoração.

## Estrutura de pastas

```
Api.Catalog/
├── Api.Catalog.Api/
├── Api.Catalog.Application/
├── Api.Catalog.Domain/
├── Api.Catalog.Infrastructure/
├── Api.Catalog.slnx
├── .gitattributes
└── .gitignore
```
