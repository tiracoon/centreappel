# Build (SDK, multi-arch — produit une image arm64 quand construit sur la VM Oracle Ampere).
# Version figée (pas le tag flottant "10.0") : un bug connu du SDK .NET 10 (corrigé dans les
# patches récents, cf. dotnet/aspnetcore#66059/#64381/#65353) empêchait blazor.web.js d'être
# inclus dans le manifeste MapStaticAssets — un build fait avec une couche Docker mise en cache
# d'un SDK antérieur au correctif reproduit ce 404 indéfiniment tant que le cache n'est pas purgé.
FROM mcr.microsoft.com/dotnet/sdk:10.0.400 AS build
WORKDIR /src

# Un seul "dotnet publish" (restore + build + publish en une passe), sans étape de restore séparée
# suivie d'un --no-restore : ce pattern (restore avec seulement le .csproj présent, puis publish
# --no-restore une fois le reste copié) laissait la détection des static web assets fournis par le
# framework (blazor.web.js) incomplète en conteneur Linux — le --no-restore empêchait de la
# recalculer avec le contexte complet au moment du publish, d'où le 404 constaté uniquement sur
# Render alors que ça fonctionnait en local (dotnet publish local fait tout en une passe aussi).
COPY . .
RUN dotnet publish "CentreAppel.Web.csproj" -c Release -o /app/publish

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
