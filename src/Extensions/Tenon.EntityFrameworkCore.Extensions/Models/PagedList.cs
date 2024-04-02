namespace Tenon.EntityFrameworkCore.Extensions.Models;

public sealed class PagedList<T>
{
    public PagedList(IEnumerable<T> items, int pageIndex, int pageSize, int totalCount)
    {
        TotalCount = totalCount;
        PageSize = pageSize;
        CurrentPage = pageIndex;
        TotalPages = (int)Math.Ceiling(totalCount * 1.0 / pageSize);
        Data = items ?? throw new ArgumentNullException(nameof(items));
    }


    public IEnumerable<T> Data { get; }


    public int CurrentPage { get; set; }


    public int TotalPages { get; }


    public int PageSize { get; }

    public int TotalCount { get; }


    public int CurrentCount => Data.Count();


    public bool HasPrev => CurrentPage > 1;


    public bool HasNext => CurrentPage < TotalPages;
}