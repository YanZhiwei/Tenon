using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.MediatR.Extensions.EventBus.Extensions;

namespace MediatREventBusSample;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug))
            .AddSingleton<IOrderService, OrderService>()
            .AddMediatREventBus(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly))
            .BuildServiceProvider();

        var orderService = serviceProvider.GetRequiredService<IOrderService>();
        await orderService.CreateOrder(1);
        Console.WriteLine("Hello, World!");
    }
}