using CleanArchitecture.Identity.Application;
using CleanArchitecture.Identity.Application.Dtos.Validators;
using CleanArchitecture.Identity.Application.Services;
using CleanArchitecture.Identity.Application.Services.Impl;
using CleanArchitecture.Identity.Repository;
using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Tenon.AspNetCore.Identity.EfCore.Sqlite.Extensions.Extensions;
using Tenon.FluentValidation.AspNetCore.Extensions.Extensions;
using Tenon.Mapper.AutoMapper.Extensions;
using Tenon.Repository.EfCore;

namespace CleanArchitecture.Identity.Api;

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
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}