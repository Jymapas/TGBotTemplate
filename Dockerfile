# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY TGBotTemplate.sln ./
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/Bot/Bot.csproj src/Bot/
COPY tests/ApplicationTests.csproj tests/

RUN dotnet restore TGBotTemplate.sln

COPY . .

RUN dotnet publish src/Bot/Bot.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/logs /app/data

ENV DOTNET_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Bot.dll"]
