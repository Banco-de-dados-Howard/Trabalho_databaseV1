# Sistema de Reservas de Motel IFODE

Sistema de gerenciamento de reservas desenvolvido em C# com PostgreSQL para controle completo de suítes, serviços adicionais (jobs) e reservas de um motel.

## Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Funcionalidades](#funcionalidades)
- [Tecnologias Utilizadas](#tecnologias-utilizadas)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração do Banco de Dados](#configuração-do-banco-de-dados)
- [Como Usar](#como-usar)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Autores](#autores)
- [Problemas Conhecidos](#problemas-conhecidos)

## Sobre o Projeto

Sistema console desenvolvido como projeto acadêmico da disciplina de Banco de Dados, ministrada pelo Professor Howard Roatti. O sistema permite gerenciar reservas de suítes de motel, incluindo cadastro de usuários, administradores, suítes, serviços adicionais e geração de relatórios.

## Funcionalidades

### Módulo Usuário
- Cadastro e login de usuários
- Visualização de suítes disponíveis
- Criação de reservas com serviços adicionais (jobs)
- Consulta de reservas realizadas
- Alteração de reservas futuras
- Cancelamento de reservas ativas

### Módulo Administrador
- Login administrativo
- Cadastro de suítes (nome, descrição, preço, capacidade)
- Cadastro de jobs/serviços (acompanhantes, extras)
- Alteração e exclusão de suítes e jobs
- Relatórios:
  - Listagem de suítes cadastradas
  - Listagem de jobs disponíveis
  - Relatório de reservas
  - Relatório completo com agregação de dados

### Funcionalidades Gerais
- Sistema de autenticação (usuário/administrador)
- Paginação de resultados
- Cálculo automático de valores totais
- Controle de datas de reserva
- Contagem de registros no sistema

## Tecnologias Utilizadas

- **Linguagem**: C# (.NET 8.0+)
- **Banco de Dados**: PostgreSQL
- **ORM/Driver**: Npgsql
- **Padrão**: CRUD com classes separadas
- **Interface**: Console Application

## Pré-requisitos

Antes de começar, você precisa ter instalado:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) ou superior
- [PostgreSQL 12](https://www.postgresql.org/download/) ou superior
- Editor de código (recomendado: [Visual Studio](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/))

## Instalação

### 1. Clone o repositório
```bash
git clone https://github.com/seu-usuario/sistema-reserva-motel.git
cd sistema-reserva-motel
```

### 2. Restaure as dependências
```bash
dotnet restore
```

## Configuração do Banco de Dados

### Opção 1: Configuração Automática
O sistema cria automaticamente as tabelas na primeira execução através do arquivo `Banco.sql`.

### Opção 2: Configuração Manual
Execute o script SQL contido em `Banco.sql` no seu PostgreSQL:

### Estrutura das Tabelas
O banco de dados contém as seguintes tabelas:
- `ADM` - Administradores do sistema
- `USUARIO` - Usuários/clientes
- `SUITE` - Suítes disponíveis
- `JOB` - Serviços adicionais
- `RESERVA` - Reservas realizadas
- `RESERVA_JOB` - Relacionamento N:N entre reservas e jobs

## Como Usar

### Executar o sistema
```bash
dotnet run
```

### Fluxo de Uso

#### 1. Primeira Execução
- O sistema exibe estatísticas das tabelas do banco
- Mostra os créditos dos desenvolvedores

#### 2. Menu Principal
```
[1] - Fazer login
[2] - Fazer seu cadastro
[0] - Sair do sistema
```

#### 3. Login como Administrador
```
Email: admin@motel.com
Senha: ****
```
**Credenciais padrão**:
- Email: `lab@Database.com`
- Senha: `lab@Database2025`

#### 4. Login como Usuário
Cadastre-se primeiro ou use credenciais existentes.

### Exemplo de Uso - Criar Reserva

1. Faça login como usuário
2. Selecione `[1] - Fazer Reserva`
3. Escolha uma suíte pelo ID
4. Selecione jobs (opcional) separados por vírgula
5. Informe as datas no formato `DD-MM-YYYY`
6. Confirme a reserva

```
Data de início (DD-MM-YYYY): 25-10-2025
Data de fim (DD-MM-YYYY): 27-10-2025
```

## Estrutura do Projeto

```
SistemaReserva/
│
├── Sources/
│   │
│   ├── Connection/
│   │   └── Conection.cs       # Gerenciamento de conexão com PostgreSQL
│   │
│   ├── Controller/            # Operações CRUD
│   │   ├── Job.cs             # CRUD de Jobs/Serviços
│   │   ├── Reserva_job.cs     # CRUD de ReservaJob (N:N)
│   │   ├── Reserva.cs         # CRUD de Reservas
│   │   └── Suite.cs           # CRUD de Suítes
│   │
│   ├── Model/
│   │   └── Models.cs          # Entidades (Suite, Job, Reserva, ReservaJob)
│   │
│   ├── Principal/
│   │   └── Program.cs         # Ponto de entrada do sistema
│   │
│   ├── Reports/
│   │   └── Relatorios.cs      # Geração de relatórios (Reservas, Jobs, Completo)
│   │
│   ├── SQL/
│   │   ├── AdmLogin.cs        # Autenticação de administradores
│   │   ├── ConfigBasicas.cs   # Configuração inicial do banco
│   │   ├── ContagemTabelas.cs # Contagem de registros nas tabelas
│   │   └── UsuarioLogin.cs    # Autenticação e cadastro de usuários
│   │
│   └── Utils/
│       ├── MenuAdm.cs         # Interface do menu administrativo
│       └── MenuUser.cs        # Interface do menu de usuário
│
├── Banco.sql                  # Script SQL de criação das tabelas
```

### Descrição dos Diretórios

**Connection**: Gerencia a conexão com o banco de dados PostgreSQL

**Controller**: Implementa as operações CRUD (Create, Read, Update, Delete) para cada entidade

**Model**: Define as classes de modelo/entidades do sistema

**Principal**: Contém o método Main e lógica principal da aplicação

**Reports**: Geração de relatórios com consultas e agregações

**SQL**: Classes relacionadas a autenticação, configuração e operações SQL

**Utils**: Utilitários e interfaces de menu para usuários e administradores

## Autores

Desenvolvido por:
- **Danton Barbosa**
- **Eduardo Guedes**
- **Gessiele Lima**
- **João Gabriel Bastos**
- **Lucas Paton**
- **Matheus Alves**

**Disciplina**: Banco de Dados  
**Professor**: Howard Roatti  
**Instituição**: FAESA