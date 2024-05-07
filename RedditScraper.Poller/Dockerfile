#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RedditPoller.Poller/RedditPoller.Poller.csproj", "RedditPoller.Poller/"]
RUN dotnet restore "./RedditPoller.Poller/./RedditPoller.Poller.csproj"
COPY . .
WORKDIR "/src/RedditPoller.Poller"
RUN dotnet build "./RedditPoller.Poller.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RedditPoller.Poller.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RedditPoller.Poller.dll"]