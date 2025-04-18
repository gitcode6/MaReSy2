using MaReSy2.pdfReports.Produkte;
using MaReSy2.pdfReports.Sets;
using MaReSy2.Services;
using Microsoft.AspNetCore.Authorization;
using QuestPDF.Infrastructure;


QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// Services hinzuf�gen
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); // Sch�tzt alle Seiten
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/Impressum");
    options.Conventions.AllowAnonymousToPage("/Datenschutz");// Login explizit freigeben
    options.Conventions.AllowAnonymousToPage("/APIError");
});

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Login"; // Pfad zur Login-Seite
        options.AccessDeniedPath = "/ZugriffVerweigert";
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminsOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("LoggedIn", policy => policy.RequireRole("Admin", "User2"));
});

builder.Services.AddHttpClient("API", client => { client.BaseAddress = new Uri("https://localhost:7162/api"); });
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<SetService>();
builder.Services.AddScoped<SingleProductService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<RentalService>();



//Produkteauszug
builder.Services.AddScoped<ProdukteAuszugPdfGenerator>();
builder.Services.AddScoped<ProduktAuszugPdfUseCase>();

//Setauszug
builder.Services.AddScoped<SetsAuszugPdfGenerator>();
builder.Services.AddScoped<SetAuszugPdfUseCase>();



builder.Services.AddControllers();

var app = builder.Build();

// Middleware konfigurieren
if (app.Environment.IsDevelopment())
{
    // Entwicklungsmodus - Zeigt detaillierte Fehler an
    app.UseDeveloperExceptionPage();
}
else
{
    // Produktionsmodus - Zeigt benutzerdefinierte Fehlerseiten an
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/APIError");
    app.UseStatusCodePagesWithReExecute("/SeiteNichtGefunden");
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Routen registrieren
app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages(); // Razor Pages zuerst
    endpoints.MapControllers(); // Controller danach
    endpoints.MapGet("/", async (context) =>
    {
        context.Response.Redirect("/Login");
    });
});

app.Run();






