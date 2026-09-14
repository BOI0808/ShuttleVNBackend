## Multi-stage Dockerfile to build and run ShuttleVNBackend.Api
# Build image
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ShuttleVNBackend.Api/ShuttleVNBackend.Api.csproj ShuttleVNBackend.Api/
COPY ShuttleVNBackend.Application/ShuttleVNBackend.Application.csproj ShuttleVNBackend.Application/
COPY ShuttleVNBackend.Infrastructure/ShuttleVNBackend.Infrastructure.csproj ShuttleVNBackend.Infrastructure/
COPY ShuttleVNBackend.Core/ShuttleVNBackend.Core.csproj ShuttleVNBackend.Core/

RUN dotnet restore ShuttleVNBackend.Api/ShuttleVNBackend.Api.csproj

COPY . .

RUN dotnet publish ShuttleVNBackend.Api/ShuttleVNBackend.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

#-----------------------------------------------------

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ShuttleVNBackend.Api.dll"]
