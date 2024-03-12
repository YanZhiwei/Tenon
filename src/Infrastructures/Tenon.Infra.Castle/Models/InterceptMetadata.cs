namespace Tenon.Infra.Castle.Models;

public class InterceptMetadata
{
    public string ClassName { get; set; }
    public string MethodName { get; set; }
    public object[] Arguments { get; set; }

    public object ReturnValue { get; set; }
}