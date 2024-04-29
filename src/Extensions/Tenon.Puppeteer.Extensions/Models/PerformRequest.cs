namespace Tenon.Puppeteer.Extensions.Models;

public class PerformRequest<T>
{
    public string FunctionName { get; set; }

    public T FunctionParameter { get; set; }

    public int IframeIndex { get; set; } = -1;

}