using System.IdentityModel.Tokens.Jwt;
using Microsoft.OpenApi.Models;
using Tenon.AspNetCore.Authentication.Bearer;
using Tenon.AspNetCore.Extensions;
using Tenon.AspNetCore.Filters;
using WebApiJwtBearerSample.Authentication;
using WebApiJwtBearerSample.Models;

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
        builder.Services.AddScoped<UserContext>();
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
        builder.Services.ConfigureJwtBearerAuthenticationOptions<SampleJwtBearerAuthenticationHandler>(
            builder.Configuration.GetSection("Jwt"), options => options.OnTokenValidated =
                context =>
                {
                    var userContext = context.HttpContext.RequestServices.GetService<UserContext>() ??
                                      throw new NullReferenceException(nameof(UserContext));

                    var principal = context.Principal ?? throw new NullReferenceException(nameof(context.Principal));
                    var claims = principal.Claims;
                    userContext.Id = long.Parse(claims.First(x =>
                        x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value);
                    userContext.Account = claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName).Value;
                    userContext.Name = claims.First(x => x.Type == JwtRegisteredClaimNames.Name).Value;
                    userContext.RoleIds = claims.First(x => x.Type == "roleids").Value;
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
        app.UseAuthentication(); // ï¿½ï¿½ï¿?ï¿½ï¿½ï¿½ï¿½ï¿½Ö¤ï¿½Ð¼ï¿½ï¿½
        // ï¿½ï¿½ï¿?ï¿½ï¿½È¨ï¿½Ð¼ï¿½ï¿?
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}