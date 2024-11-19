
global using MaReSy2_Api.Models;
global using MaReSy2_Api.Models.DTO;
global using Microsoft.EntityFrameworkCore;
using MaReSy2_Api.Services;

namespace MaReSy2_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Configuration.AddJsonFile("appsettings.json");


            builder.Services.AddDbContext<MaReSyDbContext>();

            builder.Services.AddScoped<IUserManagementService, UserManagementService>();

            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddScoped<ISingleProductService, SingleProductService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
