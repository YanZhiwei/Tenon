using CleanArchitecture.Identity.Application.Services;
using CleanArchitecture.Identity.Application.Services.Impl;
using CleanArchitecture.Identity.Repository;
using CleanArchitecture.Identity.Repository.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        builder.Services.AddDbContext<UserIdentityDbContext>(opt =>
        {
            var connStr = builder.Configuration.GetSection("Sqlite:ConnectionString").Value;

            opt.UseSqlite(connStr);
        });
        builder.Services.AddDataProtection();
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