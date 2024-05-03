using System.Text;
using Tenon.Serialization.Abstractions;
using Tenon.Serialization.Json;

namespace Tenon.Puppeteer.Extensions.Models;

public class PerformRequest<T>
{
    protected readonly ISerializer Serializer;

    public PerformRequest()
    {
        Serializer = new SystemTextJsonSerializer();
    }

    public string FunctionName { get; set; }

    public T? FunctionParameter { get; set; }

    public virtual string GenerateScript()
    {
        var evaluateScript = new StringBuilder();
        evaluateScript.Append(FunctionName);
        evaluateScript.Append("('");
        if (FunctionParameter != null)
            evaluateScript.AppendFormat($"{Serializer.SerializeObject(FunctionParameter)}");
        evaluateScript.Append("')");
        return evaluateScript.ToString();
    }
}