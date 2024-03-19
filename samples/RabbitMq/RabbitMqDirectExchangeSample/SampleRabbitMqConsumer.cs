using System.Text;
using Microsoft.Extensions.Logging;
using Tenon.Infra.RabbitMq;
using Tenon.Infra.RabbitMq.Models;

namespace RabbitMqDirectExchangeSample;

public class SampleRabbitMqConsumer(RabbitMqConnection rabbitMqConnection, ILogger<RabbitMqConsumer> logger)
    : RabbitMqConsumer(rabbitMqConnection, logger)
{
    protected override RabbitMqQueue GetRabbitMqQueue()
    {
        return new RabbitMqQueue("log_all", "MyExchange", "error", autoAck: false);
    }

    public override Task<bool> BasicConsumerReceived(string exchange, string routingKey, byte[] body,
        RabbitMqQueue queue)
    {
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($"接收成功！【{message}】，发送短信通知");
        return Task.FromResult(true);
    }
}