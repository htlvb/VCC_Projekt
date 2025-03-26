# Build-Stage (SDK für Kompilierung)
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Kopiere Projektdateien und restore Abhängigkeiten
COPY VCC_Projekt/*.sln .
COPY VCC_Projekt/*.csproj ./VCC_Projekt/
RUN dotnet restore

# Kopiere gesamten Projektcode
COPY VCC_Projekt/ ./VCC_Projekt/
RUN dotnet publish VCC_Projekt -c Release -o /app/publish

# Runtime-Stage (nur ASP.NET Core Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Port für Blazor Server (standardmäßig 8080)
EXPOSE 8080

# Startbefehl (ersetze "VCC_Projekt.dll" durch Ihren Assembly-Namen)
ENTRYPOINT ["dotnet", "VCC_Projekt.dll"]
