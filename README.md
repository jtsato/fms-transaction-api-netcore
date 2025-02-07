# Transactions API .NET Core

Este projeto implementa os princípios da Clean Architecture utilizando .NET Core 8.0. Trata-se de um serviço responsável por registrar, armazenar e gerenciar lançamentos, garantindo consistência por meio de um banco de dados relacional PostgreSQL. Além disso, expõe APIs REST para facilitar a interação com os lançamentos de forma eficiente e escalável.

[![CI](https://github.com/jtsato/fms-transaction-api-netcore/actions/workflows/continuous-integration.yml/badge.svg)](https://github.com/jtsato/fms-transaction-netcore/actions/workflows/continuous-integration.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=jtsato_transactions-api-netcore&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=jtsato_transactions-api-netcore)

[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=jtsato_transactions-api-netcore&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=jtsato_transactions-api-netcore)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=jtsato_transactions-api-netcore&metric=coverage)](https://sonarcloud.io/summary/new_code?id=jtsato_transactions-api-netcore)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=jtsato_transactions-api-netcore&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=jtsato_transactions-api-netcore)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=jtsato_transactions-api-netcore&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=jtsato_transactions-api-netcore)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=jtsato_transactions-api-netcore&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=jtsato_transactions-api-netcore)

**Índice de Conteúdos**

* [Tasks](#tasks)
* [Technology stack](#technology-stack)
* [Prerequisites](#prerequisites)
* [Solution Structure](#solution-structure)
* [Testing Strategy](#testing-strategy)
* [Mutation Reports](#mutation-reports)
* [Building and Running the solution](#building-and-running-the-solution)
* [Resources](#resources)

***
## Tasks
| Tarefa | Status |
|---------|-------|
| Macro architecture | ✅ |
| Definição de Framework | ✅ |
| Design do Modelo de Domínio | ✅ |
| Criação/configuração do Repositório de Código | ✅ |
| Definição e nomeação das camadas | ✅ |
| Diagramas C4 [design + pipeline] | ✅ |
| Tratamento de Exceções [exception filter] | ✅ |
| Solução para lidar com nulls [nullable vs optional] | ✅ |
| Internacionalização [suporte a múltiplos idiomas] | ✅ |
| Endpoints de Health Check | ⏳ |
| Integração com sonarcloud | ⏳ |
| Static Application Security Testing (SAST) | ❌ |
| Integração Contínua (Artifact Deploy) | ⏳ |
| Estratégia de geração Swagger | ⏳ |
| Testes unitários de Arquitetura (ArchUnit) | ❌ |
| Implementação de UseCases | ⏳ |
| Implementação de Infra | ⏳ |
| Implementação de Entrypoint | ⏳ |
| Injeção de Dependência | ⏳ |
| Estratégia de Testes Unitários e de Integração | ⏳ |
| Mutation Testing [configuração + pipeline] | ⏳ |
| Provisionamento de Configuration Manager | ❌ |
| Provisionamento de Secrets Manager | ❌ |
| Reserva de DNS (Domain Name System) | ❌ |
| Continuous Deployment (manifests, configuração e aprovações) | ❌ |
| Smoke Test | ⏳ |
| Cobertura de Testes Unitários/Integração em 100% | ⏳ |
| Cobertura de Mutation Testing em 100% | ⏳ |
| Testes de Performance | ⏳ |
| Observabilidade | ⏳ | 
| Documentação Técnica (README.md, etc...) | ⏳ |

## Technology stack

![.Net](https://img.shields.io/badge/.NET-5C2D91?logo=.net&logoColor=white)
![c-sharp](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)
![Web Api](https://img.shields.io/badge/Web%20Api-grey?logo=dotnet&logoColor=white)
![swagger](https://img.shields.io/badge/Swagger-85EA2D?logo=Swagger&logoColor=white)
![Shell Script](https://img.shields.io/badge/shell_script-%23121011.svg?logo=gnu-bash&logoColor=white)
![GitHub Actions](https://img.shields.io/badge/githubactions-%232671E5.svg?logo=githubactions&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-9ECAFA.svg?logo=docker)

## Prerequisites

* [.NET 8](https://dotnet.microsoft.com/download)
* [Docker](https://docs.docker.com/get-docker)
* [Docker compose](https://docs.docker.com/compose/install/)

## Solution Structure

##### Core: Entities
* Representam seu objeto de domínio.
* Aplicam apenas a lógica que é aplicável de forma geral a toda a entidade.

##### Core: Use Cases
* Representam as ações de negócio; o que é possível fazer com a aplicação. Espere um use case para cada ação de negócio.
* Lógica de negócio pura.
* Definem interfaces para os dados de que precisam para aplicar alguma lógica. Um ou mais providers irão implementar essas interfaces, mas o use case não sabe de onde os dados vêm.
* O use case não sabe quem o acionou nem como os resultados serão apresentados.
* Lança exceções de negócio.

##### Providers
* Recuperam e armazenam dados de diferentes fontes (banco de dados, dispositivos de rede, sistema de arquivos, terceiros etc.).
* Implementam as interfaces definidas pelo use case.
* Usam o framework que for mais apropriado (vão estar isolados aqui de qualquer forma).
* Observação: se estiver usando um ORM para acesso ao banco, aqui você teria outro conjunto de objetos para representar o mapeamento para as tabelas (não use as entidades do core, pois elas podem ser muito diferentes).

##### Entrypoints
* São as formas de interagir com a aplicação e, geralmente, envolvem um mecanismo de entrega (por exemplo, REST APIs, jobs agendados, GUI, outros sistemas).
* Acionam um use case e convertem o resultado para o formato adequado ao mecanismo de entrega.
* Em uma GUI, usaríamos MVC (ou MVP); o controller acionaria um use case.

##### Configuration
* Faz a ligação de tudo.
* Os frameworks (por exemplo, para injeção de dependência) ficam isolados aqui.
* Possui os “detalhes sujos” como a classe Main, configuração do servidor web, configuração de datasources, etc.

## Testing Strategy
##### Unit Tests
* Para TDD (ou seja, testes primeiro, para conduzir o design).
* Cobrir todos os pequenos detalhes, buscando 100% de cobertura.
* “Documentação de Dev para Dev”: O que essa classe deve fazer?
* Testa classes individuais de forma isolada, muito rápido.

##### Integration Tests
* Testam a integração com partes lentas (http, banco de dados, etc.).
* “Documentação para Dev”: Isto funciona conforme o esperado?
* Testa uma camada de forma isolada (por exemplo, somente endpoint REST ou somente data provider). É mais lento.
* Use qualquer biblioteca que facilite isso.

## Mutation Reports
* [Core](https://jtsato.github.io/fms-transaction-api-netcore/mutation-reports/Core/mutation-report.html)
* [EntryPoint.WebApi](https://jtsato.github.io/fms-transaction-api-netcore/mutation-reports/EntryPoint.WebApi/mutation-report.html)
* [Infra.PostgreSql](https://jtsato.github.io/fms-transaction-api-netcore/mutation-reports/Infra.PostgreSql/mutation-report.html)

***

## Building and Running the solution
* Limpando a solução:
```
dotnet clean
```
* Construindo a solução:
```
dotnet build
```
* Executando testes unitários:
```
dotnet test --nologo -v q
```
* Executando testes de mutação:
```
cd UnitTest.Core
dotnet new tool-manifest
dotnet tool install dotnet-stryker --version 4.5.1
dotnet stryker
```
* Iniciando a solução:
```
cd EntryPoint.WebApi/bin/Debug/net8.0
dotnet EntryPoint.WebApi.dll
```
***

## Resources
##### Blogs & Artigos
* The Clean Architecture https://blog.8thlight.com/uncle-bob/2012/08/13/the-clean-architecture.html
* Screaming Architecture http://blog.8thlight.com/uncle-bob/2011/09/30/Screaming-Architecture.html
* NODB https://blog.8thlight.com/uncle-bob/2012/05/15/NODB.html
* Hexagonal Architecture http://alistair.cockburn.us/Hexagonal+architecture

##### Vídeos & Apresentações
* Clean Architecture https://www.youtube.com/results?search_query=clean+architecture
* Screaming Architecture http://www.infoq.com/presentations/Screaming-Architecture

***