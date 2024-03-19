using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Tenon.Infra.RabbitMq;
using Tenon.Infra.RabbitMq.Extensions;
using Tenon.Infra.RabbitMq.Models;

namespace RabbitMqDirectExchangeSample;

internal class Program
{
    private static IServiceProvider _serviceProvider;

    private static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .Build();
        _serviceProvider = new ServiceCollection()
            .AddLogging(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Debug)).AddRabbitMq(configuration.GetSection("RabbitMq"))
            .AddSingleton<RabbitMqConsumer, SampleRabbitMqConsumer>()
            .AddSingleton<RabbitMqProducer, SampleRabbitMqProducer>()
            .BuildServiceProvider();

        try
        {
            var scope = _serviceProvider.CreateScope();
            var mqProducer = scope.ServiceProvider.GetService<RabbitMqProducer>();
            List<LogMsg> msgList = new();
            for (var i = 1; i < 100; i++)
            {
                if (i % 4 == 0)
                    msgList.Add(new LogMsg { LogType = "info", Msg = $"info第{i}条信息" });
                if (i % 4 == 1)
                    msgList.Add(new LogMsg { LogType = "debug", Msg = $"debug第{i}条信息" });
                if (i % 4 == 2)
                    msgList.Add(new LogMsg { LogType = "warn", Msg = $"warn第{i}条信息" });
                if (i % 4 == 3)
                    msgList.Add(new LogMsg { LogType = "error", Msg = $"error第{i}条信息" });
            }

            Console.WriteLine("生产者发送100条日志信息");
            //发送日志信息
            foreach (var item in msgList)
                mqProducer.PublishBasicMessage(new RabbitMqMessage(item.Msg, item.LogType));

            var mqConsumer = scope.ServiceProvider.GetService<RabbitMqConsumer>();
            mqConsumer.BasicConsumer();


        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Console.ReadLine();
        }
    }
}

internal class LogMsg
{
    public string LogType { get; set; }
    public string Msg { get; set; }
}