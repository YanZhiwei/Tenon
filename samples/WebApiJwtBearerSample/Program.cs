using Microsoft.OpenApi.Models;
using Tenon.AspNetCore.Authentication.Bearer;
using Tenon.AspNetCore.Extensions;
using Tenon.AspNetCore.Filters;
using WebApiJwtBearerSample.Authentication;

namespace WebApiJwtBearerSample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers(options => { options.Filters.Add<GlobalExceptionsFilter>(); })
            .ConfigureJsonOptions()
            .ConfigureInvalidModelStateResponse();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "JwtBearerSample.Api",
                Version = "v1"
            });
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = BearerDefaults.AuthenticationScheme
                }
            };
            c.AddSecurityDefinition(BearerDefaults.AuthenticationScheme, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            });
        });
        //builder.Services.AddAuthorization<SampleJwtBearerAuthenticationHandler, BearRequirement>();
        //builder.Services
        //    .AddAuthentication(BearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer<SampleJwtBearerAuthenticationHandler>(options => options.OnTokenValidated = context =>
        //    {
        //        Console.WriteLine("OnTokenValidated");
        //        return Task.CompletedTask;
        //    }, builder.Configuration.GetSection("Jwt"));
        builder.Services.ConfigureJwtBearerAuthenticationOptions<SampleJwtBearerAuthenticationHandler>(
            builder.Configuration.GetSection("Jwt"), options => options.OnTokenValidated =
                context =>
                {
                    Console.WriteLine("OnTokenValidated");
                    return Task.CompletedTask;
                });
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();
        app.UseAuthentication(); // 添加 身份认证中间件
        // 添加 授权中间件
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}