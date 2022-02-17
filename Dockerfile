FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY src/IntegrationsBenchmark.WebApi/IntegrationsBenchmark.WebApi.csproj IntegrationsBenchmark.WebApi/

RUN dotnet restore IntegrationsBenchmark.WebApi/IntegrationsBenchmark.WebApi.csproj

COPY src/IntegrationsBenchmark.WebApi IntegrationsBenchmark.WebApi/
COPY src/protos protos/
WORKDIR "/src/IntegrationsBenchmark.WebApi"
RUN dotnet build "IntegrationsBenchmark.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IntegrationsBenchmark.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntegrationsBenchmark.WebApi.dll"]