using CleanArchitecture.Identity.Application.Extensions;
using CleanArchitecture.Identity.Gateway.Authentication;
using CleanArchitecture.Identity.Repository.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Tenon.AspNetCore.Extensions;

namespace CleanArchitecture.Identity.Gateway;

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
        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddRepository(builder.Configuration);
        builder.Services.AddDataProtection();
        builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", false, true);
        builder.Services.AddOcelot();
        builder.Services.ConfigureJwtBearerAuthenticationOptions<IdentityAuthenticationHandler>(
            builder.Configuration.GetSection("Jwt"), options => { });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseRouting();
        app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
        app.UseAuthorization();
        app.MapControllers();
        app.UseOcelot().Wait();
        app.Run();
    }
}