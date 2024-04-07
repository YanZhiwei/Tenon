using CleanArchitecture.Identity.Api.Authentication;
using CleanArchitecture.Identity.Api.Authorization;
using CleanArchitecture.Identity.Api.Filters;
using CleanArchitecture.Identity.Api.Models;
using CleanArchitecture.Identity.Application;
using CleanArchitecture.Identity.Application.Dtos.Validators;
using CleanArchitecture.Identity.Application.Services;
using CleanArchitecture.Identity.Application.Services.Impl;
using CleanArchitecture.Identity.Repository;
using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi.Models;
using Tenon.AspNetCore.Authentication.Bearer;
using Tenon.AspNetCore.Extensions;
using Tenon.AspNetCore.Identity.EfCore.Sqlite.Extensions.Extensions;
using Tenon.DistributedId.Abstractions.Extensions;
using Tenon.DistributedId.Snowflake;
using Tenon.DistributedId.Snowflake.Configurations;
using Tenon.FluentValidation.AspNetCore.Extensions.Extensions;
using Tenon.Infra.Redis.StackExchangeProvider;
using Tenon.Mapper.AutoMapper.Extensions;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Identity.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers(options => options.Filters.Add(typeof(CustomExceptionFilter)));
        // 注册自定义的AuthorizationHandler
        builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // 配置授权策略，使用自定义的AuthorizationRequirement
        builder.Services.AddAuthorization(options =>
        {
            //options.AddPolicy("CustomPolicy", policy =>
            //{
            //    policy.Requirements.Add(new CustomRequirement());
            //});
        });
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddIdentityEfCoreSqlite<UserIdentityDbContext, User, Role, long>(
            builder.Configuration.GetSection("Sqlite"));
        builder.Services.AddDataProtection();
        builder.Services.AddSingleton<AbstractDbContextConfiguration, IdentityDbContextConfiguration>();
        builder.Services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
            options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
            options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
        });
        builder.Services.AddFluentValidationSetup(
            typeof(UserLoginDtoValidator).Assembly);
        builder.Services.AddAutoMapperSetup(typeof(AutoMapperProfile).Assembly);
        var userIdentityBuilder = new IdentityBuilder(typeof(User), typeof(Role), builder.Services);
        userIdentityBuilder.AddEntityFrameworkStores<UserIdentityDbContext>()
            .AddDefaultTokenProviders()
            .AddRoleManager<RoleManager<Role>>()
            .AddUserManager<UserManager<User>>();
        builder.Services.AddHostedService<SnowflakeWorkerNodeHostedService>();
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
            builder.Services.AddDistributedId(options =>
            {
                options.UseSnowflake(builder.Configuration.GetSection("DistributedId"));
                options.UseWorkerNode<StackExchangeProvider>(builder.Configuration.GetSection("DistributedId")
                    .GetSection("WorkerNode"));
            });
            builder.Services.ConfigureJwtBearerAuthenticationOptions<IdentityAuthenticationHandler>(
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
            c.AddSecurityDefinition(BearerDefaults.AuthenticationScheme, securityScheme);
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { securityScheme, new string[] { } }
            });
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