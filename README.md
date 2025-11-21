# 🚀 Desafio Técnico – API de Pacientes (.NET 9 / C#)

Este repositório contém a implementação do desafio técnico solicitado: uma API em .NET 9 / C# que retorna dados de pacientes a partir de um banco InMemory e demonstra boas práticas de arquitetura, injeção de dependência, seeding de dados e execução em Docker.  
Além disso, foi implementado opcionalmente um lock distribuído com Redis, garantindo segurança em cenários de concorrência.

---

## 📖 Documentação do Processo

### 1. Abertura do Chamado
- Primeiros passos: análise detalhada do enunciado e definição dos requisitos mínimos.  
- Pesquisa inicial: revisão das novidades do .NET 9, boas práticas de Clean Architecture e uso de Entity Framework Core InMemory.  
- Ferramentas utilizadas:
  - .NET 9 SDK
  - Entity Framework Core
  - StackExchange.Redis
  - xUnit + FluentAssertions para testes
  - Docker / Docker Compose para containerização

---

### 2. Solução Escolhida
- Arquitetura em camadas (Clean Architecture):
  - Domain → Entidades e abstrações
  - Application → Casos de uso e serviços
  - Infrastructure → Persistência e concorrência
  - Api → Controllers, configuração e ponto de entrada
- DbContext InMemory: escolhido para simplificar setup e permitir testes rápidos.  
- Seeding de dados: pacientes iniciais são carregados automaticamente ao subir a aplicação.  
- Injeção de dependência: todos os serviços e repositórios são registrados via ServiceCollectionExtensions.  
- Lock distribuído (opcional): implementado com Redis para garantir exclusividade em processos concorrentes.  
- Alternativas descartadas:
  - Banco relacional real (ex.: SQL Server/Postgres) → descartado por aumentar complexidade sem necessidade no desafio.
  - Lock em memória local → descartado por não funcionar em múltiplos containers.

---

### 3. Possíveis Problemas na Execução
- Configuração do Redis: ajustes de conexão entre containers no Docker Compose.  
  - Solução: uso de IConnectionMultiplexer com string de conexão configurável via appsettings.json.  
- Validação do lock distribuído: garante que apenas um relatório seja processado por vez.
  - Solução: o teste chama o caso de uso em sequência, usando um FakeLockService que simula pegar e liberar o lock, confirmando que o controle de concorrência funciona corretamente.
  - Complemento: também foram implementados testes de concorrência real com Task.WhenAll, garantindo que apenas uma execução ocorra em chamadas simultâneas.
- Seeding no InMemory: garantir que os dados sejam carregados apenas uma vez.  
  - Solução: método SeedDatabase() chamado no Program.cs.

---

### 4. Resultado Final
- API funcional com rota principal que retorna uma view com dados do banco InMemory.  
- CRUD completo de pacientes.  
- Endpoint /processar-relatorio que:
  - Aguarda 5 segundos simulando processo demorado.
  - Usa lock distribuído para impedir execução concorrente.
  - Retorna mensagens claras em português:
    - "Processo concluído"
    - "Recurso ocupado. Tente novamente mais tarde."
- Logs estruturados:
  - Tentando adquirir o lock
  - Lock adquirido
  - Executando o processo
  - Lock liberado
- Testes unitários e de concorrência implementados.  
- Projeto containerizado com Docker, incluindo Redis via Docker Compose.  
- Deploy documentado para servidor Linux.

---

## ⚙️ Execução

### 1. Clonar o repositório
git clone: https://github.com/renanguedesgs/desafio-tecnico-API.NET-9

acessar pasta: cd desafio-t-cnico-API.NET-9 
acessar pasta: cd Infraestrutura

### 2. Subir com Docker Compose
docker-compose up -d

### 3. Acessar a API
Rota principal: http://localhost:8081

---

## 🐳 Docker

- `.dockerignore`: evita que arquivos desnecessários (logs, testes, configs locais) sejam incluídos na imagem, mantendo o build limpo e leve.

- `.env`: centraliza variáveis de ambiente como conexões e portas, facilitando a troca entre ambientes sem alterar o código.

- `docker-compose.yml`: orquestra os serviços da aplicação e do Redis, permitindo subir toda a stack com um único comando.

- `Dockerfile`: define o build da aplicação .NET 9 com camadas otimizadas, garantindo portabilidade e execução consistente.

---

## 📦 Como fazer Deploy em Servidor Linux

### 1. Instalar Docker e Docker Compose  
Documentação oficial:  
- https://docs.docker.com/engine/install/  
- https://docs.docker.com/compose/install/linux/

Comandos:  
`sudo apt update`  
`sudo apt install docker.io docker-compose -y`

### 2. Instalar Git (caso não esteja disponível)  
Comando:  
`sudo apt install git -y`

### 3. Criar diretório do projeto e clonar o repositório  
Comandos:  
`mkdir -p /var/www`  
`cd /var/www`  
`git clone https://github.com/renanguedesgs/desafio-t-cnico-API.NET-9`  
`cd desafio-t-cnico-API.NET-9`

### 4. Editar variáveis de ambiente  
Comando:  
`nano .env`

### 5. Subir containers em modo produção  
Comando:  
`docker-compose up -d`

> ⚠️ Como a API já está exposta via porta local (`8081`, `8082`), não é necessário configurar proxy reverso como Nginx, exceto se desejar expor via domínio público ou aplicar HTTPS.

---

## 📁 Estrutura de Pastas

Api/  
 ├── Extensions/  
 ├── Seed/  
 └── Program.cs  
Application/  
 ├── DTOs/  
 ├── Services/  
 └── UseCases/  
Domain/  
 ├── Abstractions/  
 └── Entities/  
Infrastructure/  
 ├── Concurrency/  
 └── Locks/  
 └── Persistence/  
Tests/  
 └── UnitTests.cs

---

## ✅ Critérios Atendidos

[x] Organização impecável de código e arquitetura  
[x] Disponível em repositório GitHub com URL  
[x] Implementado com Docker e Docker Compose  
[x] Arquitetura em camadas (Clean Architecture)  
[x] Exemplo funcional de Injeção de Dependência  
[x] Descritivo de deploy em servidor Linux  
[x] DbContext configurado com InMemory  
[x] Seeding de dados iniciais  
[x] Lock distribuído com Redis (opcional)

---
