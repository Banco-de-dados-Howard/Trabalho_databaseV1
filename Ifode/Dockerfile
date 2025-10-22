FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app

COPY . .
RUN dotnet restore

# Substituir localhost por host.docker.internal no build
RUN sed -i 's/localhost/host.docker.internal/g' Sources/Connection/Conection.cs

RUN dotnet build -c Release

CMD ["dotnet", "run", "--project", "Ifode.csproj"]