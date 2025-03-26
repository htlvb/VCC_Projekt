# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build  # <- Geändert zu 8.0
WORKDIR /src

COPY *.sln .
COPY VCC_Projekt/*.csproj ./VCC_Projekt/
RUN dotnet restore

COPY VCC_Projekt/. ./VCC_Projekt/
RUN dotnet build -c Release --no-restore
RUN dotnet publish "VCC_Projekt/VCC_Projekt.csproj" -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0  # <- Geändert zu 8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VCC_Projekt.dll"]
