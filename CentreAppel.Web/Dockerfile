# Build (SDK, multi-arch — produit une image arm64 quand construit sur la VM Oracle Ampere).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CentreAppel.Web.csproj .
RUN dotnet restore "CentreAppel.Web.csproj"

COPY . .
RUN dotnet publish "CentreAppel.Web.csproj" -c Release -o /app/publish --no-restore

# Runtime (ASP.NET, image plus légère que le SDK).
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# PORT est fourni par l'hébergeur au démarrage du conteneur (Render : 10000 par défaut ;
# docker-compose : fixé explicitement à 8080 dans le fichier compose) — pas connu à la build,
# donc résolu dans l'ENTRYPOINT plutôt que figé en dur ici.
ENV PORT=10000
EXPOSE 10000

# Désactive le rechargement à chaud d'appsettings.json (FileSystemWatcher/inotify) : inutile en
# conteneur immuable, et la limite inotify du noyau est partagée par UID entre tous les conteneurs
# d'un même hôte — sur un hébergeur mutualisé (ex. plan gratuit Render), elle peut être épuisée par
# d'autres conteneurs et faire planter l'appli au démarrage (System.IO.IOException: "configured
# user limit (128) on the number of inotify instances").
ENV DOTNET_HOSTBUILDER__RELOADCONFIGONCHANGE=false

ENTRYPOINT ["/bin/sh", "-c", "ASPNETCORE_URLS=http://+:${PORT} exec dotnet CentreAppel.Web.dll"]
