using System.Globalization;
using CentreAppel.Web.Application.Services;
using CentreAppel.Web.Components;
using CentreAppel.Web.Data.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddLocalization();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "obj", "dataprotection-keys")));

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/connexion";
        options.AccessDeniedPath = "/acces-refuse";
    });
builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("Default"))
    .UseSnakeCaseNamingConvention()
    .EnableDetailedErrors()
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddScoped<IAuthentificationService, AuthentificationService>();
builder.Services.AddScoped<ICampagneService, CampagneService>();
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<CanalAchatService>();
builder.Services.AddScoped<DeroulementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
    await using var context = await dbFactory.CreateDbContextAsync();
    await context.Database.MigrateAsync();
    await DbSeeder.SeedAsync(context, CancellationToken.None);
}

// Configure the HTTP request pipeline.

// Render (et la plupart des PaaS) terminent le TLS à leur edge et transmettent la requête en HTTP
// simple au conteneur. Sans ce middleware, l'appli ignore que la requête d'origine était en HTTPS :
// UseHttpsRedirection redirige alors le trafic (y compris la négociation SignalR du circuit Blazor),
// ce qui empêche la connexion websocket de s'établir et rend l'interactivité muette en production.
// KnownNetworks/KnownProxies par défaut ne font confiance qu'à 127.0.0.1 (loopback) — le proxy
// Render n'est pas en loopback, donc il faut vider ces listes (via .Clear(), pas via un initialiseur
// de collection vide "= { }" qui n'ajoute rien et laisse le défaut loopback-only intact) pour que
// les en-têtes X-Forwarded-* soient réellement pris en compte.
var forwardedHeadersOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
};
forwardedHeadersOptions.KnownIPNetworks.Clear();
forwardedHeadersOptions.KnownProxies.Clear();
app.UseForwardedHeaders(forwardedHeadersOptions);

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

var culturesSupportees = new[] { new CultureInfo("fr-FR") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("fr-FR"),
    SupportedCultures = culturesSupportees,
    SupportedUICultures = culturesSupportees,
});

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization();

app.Run();
