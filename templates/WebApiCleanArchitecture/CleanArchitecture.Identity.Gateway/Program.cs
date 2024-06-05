using CleanArchitecture.Identity.Gateway.Authentication;
using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Swashbuckle.AspNetCore.Filters;
using Tenon.AspNetCore.Configuration;
using Tenon.AspNetCore.Extensions;
using Tenon.Infra.Swagger.Extensions;

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
        builder.Services.AddDataProtection();
        builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", false, true);
        builder.Services.AddOcelot();
        builder.Services.AddHttpContextAccessor();
        //builder.Services.AddAuthentication()
        //    .AddJwtBearer("gw", options =>
        //    {
        //        var bearerConfig = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
        //        options.TokenValidationParameters = bearerConfig.GenerateTokenValidationParameters();
        //    });
        builder.Services.ConfigureJwtBearerAuthenticationOptions<IdentityAuthenticationHandler>(
            builder.Configuration.GetSection("Jwt"), options => { });
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Gateway Api",
                Version = "v1"
            });

            c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
            c.AddBearerAuthorizationHeader();
        });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            var apis = new List<string> { "identity-svc" };
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "gateway");
                apis.ForEach(m => { c.SwaggerEndpoint($"/{m}/swagger/swagger.json", m); });
               // c.RoutePrefix = "";
            });
            app.UseSwagger();
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