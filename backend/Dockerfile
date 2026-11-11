# Imagem da API. Build em duas etapas: o SDK compila e publica; a imagem final só tem o runtime.
#   docker build -t prescricao-api ./backend

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Primeiro só os arquivos de projeto: a camada do restore fica em cache enquanto as dependências não mudam.
COPY Directory.Build.props Directory.Packages.props ./
COPY src/PrescricaoMedica.Domain/PrescricaoMedica.Domain.csproj src/PrescricaoMedica.Domain/
COPY src/PrescricaoMedica.Application/PrescricaoMedica.Application.csproj src/PrescricaoMedica.Application/
COPY src/PrescricaoMedica.Infrastructure/PrescricaoMedica.Infrastructure.csproj src/PrescricaoMedica.Infrastructure/
COPY src/PrescricaoMedica.Api/PrescricaoMedica.Api.csproj src/PrescricaoMedica.Api/
RUN dotnet restore src/PrescricaoMedica.Api/PrescricaoMedica.Api.csproj

COPY src/ src/
RUN dotnet publish src/PrescricaoMedica.Api/PrescricaoMedica.Api.csproj -c Release --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Usuário sem privilégios, já existente nas imagens oficiais do .NET.
USER $APP_UID
ENTRYPOINT ["dotnet", "PrescricaoMedica.Api.dll"]
