namespace CentreAppel.Web.Application.Extensions;

// Unique fuseau du projet (France, cf. CLAUDE.md "Conversion en heure locale à l'affichage
// uniquement") : tout est stocké en UTC (colonnes timestamptz), converti seulement à la saisie
// (formulaire) et à l'affichage. Id IANA "Europe/Paris" : résoluble par TimeZoneInfo aussi bien
// sous Windows (dev) que Linux (Render), contrairement à l'id Windows "Romance Standard Time".
public static class DateTimeExtensions
{
    private static readonly TimeZoneInfo FuseauFrance = TimeZoneInfo.FindSystemTimeZoneById("Europe/Paris");

    public static DateTime HeureLocaleVersUtc(this DateTime heureLocale) =>
        TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(heureLocale, DateTimeKind.Unspecified), FuseauFrance);

    public static DateTime UtcVersHeureLocale(this DateTime heureUtc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(heureUtc, DateTimeKind.Utc), FuseauFrance);
}
