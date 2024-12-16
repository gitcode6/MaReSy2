using MaReSy2.Services;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Services hinzufügen
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/"); // Schützt alle Seiten
    options.Conventions.AllowAnonymousToPage("/Login"); // Login explizit freigeben
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
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<SetService>();
builder.Services.AddScoped<SingleProductService>();
builder.Services.AddControllers();

var app = builder.Build();

// Middleware konfigurieren
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
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

