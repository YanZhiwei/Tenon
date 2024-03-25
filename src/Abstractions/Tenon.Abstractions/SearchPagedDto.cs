namespace Tenon.Abstractions;

public abstract class SearchPagedDto : IDto
{
    private int _pageIndex;
    private int _pageSize;

    public int PageIndex
    {
        get => _pageIndex < 1 ? 1 : _pageIndex;
        set => _pageIndex = value;
    }

    public int PageSize
    {
        get
        {
            if (_pageSize < 5) _pageSize = 5;
            if (_pageSize > 100) _pageSize = 100;
            return _pageSize;
        }
        set => _pageSize = value;
    }
}