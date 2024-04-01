using Mapster;
using Tenon.Mapper.Abstractions;

namespace Tenon.Mapper.Mapster;

/// <summary>
///     https://github.com/MapsterMapper/Mapster
/// </summary>
public sealed class MapsterObject : IObjectMapper
{
    public TDestination Map<TDestination>(object source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return source.Adapt<TDestination>();
    }

    public TDestination Map<TSource, TDestination>(TSource source) where TSource : class where TDestination : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return source.Adapt<TDestination>();
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        where TSource : class where TDestination : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));
        return source.Adapt(destination);
    }
}