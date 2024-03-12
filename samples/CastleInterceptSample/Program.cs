using Castle.DynamicProxy;

namespace CastleInterceptSample;

public interface IProductRepository
{
    [OperateLog(LogName = "新增产品")]
    void Add(string name);
}

public class ProductRepository : IProductRepository
{
    public void Add(string name)
    {
        Console.WriteLine($"新增产品：{name}");
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var generator = new ProxyGenerator();
        IProductRepository productRepo = new ProductRepository();
        var loggerIntercept = (IAsyncInterceptor)new OperateLogAsyncInterceptor();
        var proxy = generator.CreateInterfaceProxyWithTarget(productRepo, loggerIntercept);
        proxy.Add("大米");

        Console.Read();
    }
}