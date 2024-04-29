namespace Tenon.Puppeteer.Extensions.Models;

//ActionResult
[Serializable]
public class PerformResult<TValue>
{
    public PerformResult()
    {
    }

    public PerformResult(TValue value)
    {
        Data = value;
    }

    public long Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public int IframeIndex { get; set; } = -1;
    public TValue? Data { get; }

    public int Code { get; set; }

    public static implicit operator PerformResult<TValue>(TValue value)
    {
        return new PerformResult<TValue>(value);
    }
}

[Serializable]
public class PerformResult(object value)
{
    public long Timestamp { get; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    public int IframeIndex { get; set; } = -1;
    public object Data { get; } = value;
    public int Code { get; set; }

    public static PerformResult FromObject(object value)
    {
        return new PerformResult(value);
    }
}