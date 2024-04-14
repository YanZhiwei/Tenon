namespace Tenon.Infra.Windows.Form.Common;

public class ColorConsole
{
    #region Methods

    /// <summary>
    ///     写入一行错误信息【红色】
    /// </summary>
    /// <param name="text">文本</param>
    public static void WriteError(string text)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    /// <summary>
    ///     写入一行信息【绿色】
    /// </summary>
    /// <param name="text">文本</param>
    public static void WriteInfo(string text)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    /// <summary>
    ///     写入一行
    /// </summary>
    /// <param name="text">文本</param>
    /// <param name="color">文本颜色</param>
    public static void WriteLine(string text, ConsoleColor color)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = color;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    /// <summary>
    ///     写入一行警告信息【红色】
    /// </summary>
    /// <param name="text">文本</param>
    public static void WriteWarn(string text)
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(text);
        Console.ResetColor();
    }

    #endregion Methods
}