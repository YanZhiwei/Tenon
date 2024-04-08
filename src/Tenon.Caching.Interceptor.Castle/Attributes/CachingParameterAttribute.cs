using Tenon.Infra.Castle.Attributes;

namespace Tenon.Caching.Interceptor.Castle.Attributes;

[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class CachingParameterAttribute : InterceptAttribute
{

}