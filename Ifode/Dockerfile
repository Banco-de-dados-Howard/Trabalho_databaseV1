# Usar a imagem base do .NET SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar o arquivo de projeto e restaurar dependências
COPY *.csproj ./
RUN dotnet restore

# Copiar todo o código fonte
COPY . ./

# Compilar a aplicação
RUN dotnet publish -c Release -o out

# Usar a imagem runtime do .NET para execução
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Copiar os arquivos compilados
COPY --from=build /app/out .

# Copiar o arquivo Banco.sql
COPY Banco.sql ./

# Definir o ponto de entrada
ENTRYPOINT ["dotnet", "Ifode.dll"]