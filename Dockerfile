# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# 1. Kopiere die Solution-Datei (aus dem Root)
COPY *.sln .

# 2. Kopiere die Projektdatei (aus dem VCC_Projekt-Ordner)
COPY VCC_Projekt/*.csproj ./VCC_Projekt/

# 3. Führe dotnet restore für die Solution aus
RUN dotnet restore

# 4. Kopiere den gesamten restlichen Code
COPY VCC_Projekt/. ./VCC_Projekt/

# 5. Build
RUN dotnet build -c Release --no-restore

# 6. Publish
RUN dotnet publish "VCC_Projekt/VCC_Projekt.csproj" -c Release -o /app/publish --no-build

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VCC_Projekt.dll"]
