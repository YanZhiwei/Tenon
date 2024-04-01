using AutoMapper;
using Tenon.Mapper.Abstractions;

namespace Tenon.Mapper.AutoMapper;

/// <summary>
///     https://automapper.org/
/// </summary>
public sealed class AutoMapperObject : IObjectMapper
{
    private readonly IMapper _mapper;

    public AutoMapperObject(IMapper mapper)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public TDestination Map<TDestination>(object source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        return _mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source) where TSource : class where TDestination : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return _mapper.Map<TSource, TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        where TSource : class where TDestination : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (destination == null)
            throw new ArgumentNullException(nameof(destination));
        return _mapper.Map(source, destination);
    }
}