namespace Tenon.Infra.Windows.ChromiumAccessibility.Options;

public sealed class ChromeOpenOption
{
    public Uri Url { get; set; }

    public string AppLocalData { get; set; }

    public HashSet<string>? Args { get; set; }

    /// <summary>
    /// 是否最大化
    /// </summary>
    public bool Maximized { get; set; }

    /// <summary>
    ///     隐身模式
    /// </summary>
    public bool Incognito { get; set; }

    public bool WaitForPageLoad { get; set; }
}