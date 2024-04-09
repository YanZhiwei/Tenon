using System.Reflection;

namespace Tenon.Caching.Interceptor.Castle.Models;

internal sealed class InvocationMetadata
{
    public MethodInfo MethodInfo { get; set; }
    public string ClassName { get; set; }
    public object[] Arguments { get; set; }
    public Attribute? Attribute { get; set; }
}