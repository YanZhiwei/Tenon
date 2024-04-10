namespace Tenon.Caching.Interceptor.Castle.Exceptions;

internal sealed class InvocationException(string message, Exception innerException)
    : Exception(message, innerException);