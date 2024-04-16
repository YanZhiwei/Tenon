using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CleanArchitecture.Identity.Api.Authentication;
using CleanArchitecture.Identity.Api.Authorization;
using CleanArchitecture.Identity.Api.Filters;
using CleanArchitecture.Identity.Api.Models;
using CleanArchitecture.Identity.Application;
using CleanArchitecture.Identity.Application.Extensions;
using CleanArchitecture.Identity.Repository.Extensions;
using Microsoft.OpenApi.Models;
using Tenon.AspNetCore.Authentication.Bearer;
using Tenon.AspNetCore.Authorization.Bearer;
using Tenon.AspNetCore.Extensions;

namespace CleanArchitecture.Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers(options => options.Filters.Add(typeof(CustomExceptionFilter)));
        builder.Services.AddAuthorization<PermissionAuthorizationHandler, BearRequirement>();
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddConsole();
            loggingBuilder.SetMinimumLevel(LogLevel.Debug);
            loggingBuilder.AddSimpleConsole(options =>
            {
                options.IncludeScopes = true;
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            });
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddRepository(builder.Configuration);
        builder.Services.AddDataProtection();
        builder.Services.AddScoped<UserContext>();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CleanArchitecture.Identity.Api",
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
        builder.Services.ConfigureJwtBearerAuthenticationOptions<IdentityAuthenticationHandler>(
            builder.Configuration.GetSection("Jwt"), options => options.OnTokenValidated =
                context =>
                {
                    var userContext = context.HttpContext.RequestServices.GetService<UserContext>() ??
                                      throw new NullReferenceException(nameof(UserContext));
                    var principal = context.Principal ??
                                    throw new NullReferenceException(nameof(context.Principal));
                    var claims = principal.Claims;
                    userContext.Id = long.Parse(claims.First(x =>
                        x.Type == ClaimTypes.NameIdentifier).Value);
                    userContext.Account = claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value;
                    userContext.Name = claims.First(x => x.Type == ClaimTypes.Name).Value;
                    userContext.RoleIds = claims.First(x => x.Type == IdentityRegisteredClaimNames.DeptId).Value;
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
        app.UseAuthorization();
        app.UseAuthentication();
        app.MapControllers();
        app.Run();
    }
}