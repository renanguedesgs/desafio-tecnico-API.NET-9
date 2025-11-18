# 📌 Desafio Técnico – API .NET 9 / C#

Este repositório contém a implementação de uma API simples desenvolvida em **.NET 9**, utilizando **arquitetura organizada**, **injeção de dependência**, **Entity Framework InMemory**, **seeding de dados** e suporte a **Docker**.  
O projeto segue as instruções do desafio e inclui documentação completa do processo de desenvolvimento.

---

## 🚀 Tecnologias Utilizadas

- .NET 9 / C#
- ASP.NET Core
- Entity Framework Core (InMemory)
- Docker e Docker Compose
- Injeção de Dependência (DI)
- (Opcional) Redis/Postgres para Lock Distribuído

---

## 📂 Estrutura do Projeto

```
/src
  /Api
  /Application
  /Domain
  /Infrastructure
/docker
README.md
```

- **Domain**: Entidades e interfaces centrais  
- **Application**: Casos de uso e serviços  
- **Infrastructure**: Persistência, DbContext e implementações  
- **Api**: Controllers, rotas e configuração da aplicação  

---

# 📝 Documentação do Processo de Desenvolvimento

## 1️⃣ Abertura do Chamado

O desenvolvimento iniciou com a leitura completa dos requisitos e definição do ambiente.  
Passos iniciais:

- Estudo das demandas do desafio  
- Pesquisa sobre recursos atualizados do .NET 9  
- Criação da solução, estrutura e escolha das ferramentas principais  
- Configuração inicial da API, rota principal e arquitetura separada em camadas  

Foram utilizados .NET 9, EF Core InMemory, Docker e MVC com views.

---

## 2️⃣ Solução Escolhida

A solução foi projetada com foco em organização, extensibilidade e clareza técnica.

### ✔ Por que essa abordagem?

- Arquitetura organizada para permitir evolução  
- EF InMemory por ser requisito do desafio  
- Seeding automatizado para facilitar testes  
- Docker para padronização do ambiente  
- Controllers com views para atender a necessidade específica da rota principal  

### Alternativas consideradas:

- Minimal API → descartada para manter organização em projetos maiores  
- SQL Server / SQLite → incompatível com o requisito de banco InMemory  
- Arquiteturas complexas (DDD completo) → exagero para o escopo do desafio  

---

## 3️⃣ Possíveis Problemas Encontrados

Durante o desenvolvimento, alguns desafios surgiram:

### ⚠ Configuração do InMemory
A sincronização do seeding com o Docker exigiu ajustes no ciclo de vida do DbContext.

### ⚠ View + API
Combinar o retorno de uma view com uma API exigiu configuração adicional via MVC.

### ⚠ Docker
Ajustes no Dockerfile multi-stage foram necessários para build mais rápido e eficiente.

### ⚠ (Opcional) Lock Distribuído
Foi necessário adaptar o Docker Compose e garantir que o lock funcionasse entre várias instâncias simultâneas da API.

---

## 4️⃣ Resultado Final

A API final possui:

- ✔ Rota principal exibindo dados via view  
- ✔ Banco InMemory com seeding  
- ✔ Injeção de dependência em serviços e repositórios  
- ✔ Arquitetura organizada  
- ✔ Docker funcional tanto via Dockerfile quanto Docker Compose  
- ✔ (Opcional) Endpoint com lock distribuído funcional entre múltiplos containers  

Execução simples:

```bash
docker compose up --build
```

Acesso da rota principal:

```
http://localhost:5000
```

Endpoint opcional:

```
/processar-relatorio
```

---

# 🐳 Como rodar com Docker

### Usando Dockerfile:
```bash
docker build -t desafio-api .
docker run -p 5000:80 desafio-api
```

### Usando Docker Compose:
```bash
docker compose up --build
```

---

# 🚀 Deploy em Servidor Linux

1. Instalar Docker + Docker Compose  
2. Clonar o repositório  
3. Executar o ambiente:
   ```bash
   docker compose up -d --build
   ```  
4. (Opcional) Configurar NGINX como proxy reverso  
5. Monitorar logs do container com:
   ```bash
   docker logs -f <container>
   ```

---

# 🔐 Desafio Opcional – Lock Distribuído

- Endpoint: `/processar-relatorio`  
- Simula tarefa de 5 segundos  
- Tenta adquirir lock usando Redis ou Postgres  
- Apenas uma requisição é executada; demais retornam 423/409  
- Logs:
  - Tentando adquirir lock  
  - Lock adquirido  
  - Executando  
  - Lock liberado  
- Usado `ILockService` via DI  
- Docker Compose sobe 2 instâncias da API + Redis/Postgres para testes reais  

---

# 📄 Licença

Este projeto tem finalidade exclusivamente avaliativa.
