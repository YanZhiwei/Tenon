
using Tenon.Abstractions;
using Tenon.AspNetCore;
using Tenon.Consul.Extensions;

namespace ConsulSample
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
            var startAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            var serviceInfo = ServiceInfo.CreateInstance(startAssembly);
            builder.Services.AddConsul(builder.Configuration.GetSection("Consul"));
            builder.Services.AddSingleton<IWebServiceInfo>(serviceInfo);
            var app = builder.Build();
            app.UseConsulRegistrationCenter();
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
}
