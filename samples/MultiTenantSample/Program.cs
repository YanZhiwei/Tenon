using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using MultiTenantSample.Data;
using MultiTenantSample.Services;
using Tenon.Repository;
using Tenon.Repository.EfCore;
using Tenon.Repository.EfCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 添加控制器服务
builder.Services.AddControllers();

// 添加 OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
// 添加 Swagger 包引用
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MultiTenant Sample API", Version = "v1" });
});

// 添加 DbContext
builder.Services.AddTenantEfCore<MultiTenantDbContext>(
    builder.Configuration.GetSection("DbOptions"),
    options => options.UseSqlite(builder.Configuration.GetSection("DbOptions:ConnectionString").Value));

// 添加 HttpContextTenantResolver
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpContextTenantResolver>();
builder.Services.AddScoped<IEfTenantResolver>(provider => provider.GetRequiredService<HttpContextTenantResolver>());
builder.Services.AddScoped<ITenantResolver<long, long>>(provider => provider.GetRequiredService<HttpContextTenantResolver>());
builder.Services.AddScoped(typeof(ITenantFilterable<long>), provider => provider.GetRequiredService<HttpContextTenantResolver>());

// 添加应用服务
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<OrderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // 添加 Swagger 中间件
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MultiTenant Sample API v1"));
    
    // 初始化数据库
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<MultiTenantDbContext>();
        dbContext.Database.EnsureCreated();
        DbInitializer.InitializeAsync(app.Services).GetAwaiter().GetResult();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
