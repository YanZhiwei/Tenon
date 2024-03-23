using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace SnowflakeSample
{
    internal class Program
    {
        private static object _serviceProvider;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            try
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false)
                    .Build();
                _serviceProvider = new ServiceCollection()
                    .AddLogging(loggingBuilder => loggingBuilder
                        .SetMinimumLevel(LogLevel.Debug))
                    //.AddSingleton<RabbitMqConsumer, SampleRabbitMqConsumer>()
                    //.AddSingleton<RabbitMqProducer, SampleRabbitMqProducer>()
                    .BuildServiceProvider();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.WriteLine("done");
                Console.ReadLine();
            }
        }
    }
}
