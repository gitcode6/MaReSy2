using MaReSy2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Login"; // Setze den Pfad zur Login-Seite
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpClient("API", client => { client.BaseAddress = new Uri("https://localhost:7162/api"); });

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddControllers();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Füge die Authentifizierung und Autorisierung zur Pipeline hinzu
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapGet("/", async (context) =>
    {
        context.Response.Redirect("/Login");
    });
});




/*
builder.Services.AddRazorPages(options =>
{
	options.Conventions.AuthorizeFolder("/"); // Schützt alle Seiten
});
*/




app.Run();
