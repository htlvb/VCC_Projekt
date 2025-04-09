using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VCC_Projekt.Components;
using VCC_Projekt.Components.Account;
using VCC_Projekt.Data;
using Pomelo.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddKeyPerFile("/run/secrets", optional: true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<EmailSender>();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddServerSideBlazor().AddHubOptions(options => { options.MaximumReceiveMessageSize = null; });

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? BuildConnectionStringFromSecrets(builder.Configuration)
    ?? throw new InvalidOperationException("Database connection not configured");

static string? BuildConnectionStringFromSecrets(IConfiguration config)
{
    try
    {
        return $"Server={config["DB_SERVER"]};" +
               $"Database={config["DB_NAME"]};" +
               $"User Id={config["DB_USER"]};" +
               $"Password={config["DB_PASSWORD"]};" +
               "Connection Timeout=200;" +
               "Default Command Timeout=60";
    }
    catch
    {
        return null;
    }
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 23)), mysqloptions => mysqloptions.EnableRetryOnFailure(int.MaxValue, TimeSpan.FromSeconds(5), null)),
    ServiceLifetime.Scoped);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// MailOptions with Secrets priority
builder.Services.Configure<MailOptions>(options =>
{
    // Email Configuration (Secrets First)
    options.Email = builder.Configuration["MAIL_EMAIL"]
        ?? builder.Configuration["MailOptions:Email"];

    // Password Configuration (Secrets First)
    options.Password = builder.Configuration["MAIL_PASSWORD"]
        ?? builder.Configuration["MailOptions:Password"];

    // Server Configuration (Secrets First)
    options.SmptServer = builder.Configuration["SMTP_SERVER"]
        ?? builder.Configuration["MailOptions:SmptServer"]
        ?? "smtp.office365.com"; // Default fallback

    options.ImapServer = builder.Configuration["IMAP_SERVER"]
        ?? builder.Configuration["MailOptions:ImapServer"]
        ?? "outlook.office365.com"; // Default fallback

    // Azure AD Configuration (Secrets First)
    options.TenantId = builder.Configuration["TENANT_ID"]
        ?? builder.Configuration["MailOptions:TenantId"];

    options.ClientId = builder.Configuration["CLIENT_ID"]
        ?? builder.Configuration["MailOptions:ClientId"];

    options.ClientSecret = builder.Configuration["CLIENT_SECRET"]
        ?? builder.Configuration["MailOptions:ClientSecret"];
});

builder.Services.AddControllers();
var frontendUrl = builder.Configuration["FRONTEND_URL"]
    ?? builder.Configuration["Frontend:Url"];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins(frontendUrl)
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

builder.Services.AddMudServices();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseCors("AllowSpecificOrigin");

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
