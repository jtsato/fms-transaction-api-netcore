FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY ["global.json", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["src/main/Core/Core.csproj", "./Core/"]
COPY ["src/main/Infra.PostgreSql/Infra.PostgreSql.csproj", "./Infra.PostgreSql/"]
COPY ["src/main/EntryPoint.WebApi/EntryPoint.WebApi.csproj", "./EntryPoint.WebApi/"]

RUN dotnet restore "./EntryPoint.WebApi/EntryPoint.WebApi.csproj" --force --no-cache

COPY ./src/main/Core/. ./Core/
COPY ./src/main/Infra.PostgreSql/. ./Infra.PostgreSql/
COPY ./src/main/EntryPoint.WebApi/. ./EntryPoint.WebApi/

WORKDIR /source/EntryPoint.WebApi
FROM build AS publish
RUN dotnet publish "EntryPoint.WebApi.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app

ENV COMPlus_EnableDiagnostics=0 \
    ASPNETCORE_URLS=http://*:8000

COPY --from=publish --chown=app:app /app/publish .

EXPOSE 8000
USER app

ENTRYPOINT ["dotnet", "EntryPoint.WebApi.dll"]
