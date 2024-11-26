using MaReSy2.Services;

var builder = WebApplication.CreateBuilder(args);

// Services hinzufügen
builder.Services.AddRazorPages(options =>
{
    //options.Conventions.AuthorizeFolder("/"); // Schützt alle Seiten
    //options.Conventions.AllowAnonymousToPage("/Login"); // Login explizit freigeben
});
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Login"; // Pfad zur Login-Seite
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpClient("API", client => { client.BaseAddress = new Uri("https://localhost:7162/api"); });
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
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

