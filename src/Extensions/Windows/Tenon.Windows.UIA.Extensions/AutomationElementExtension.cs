using System.Windows.Automation;
using Interop.UIAutomationClient;

namespace Tenon.Windows.UIA.Extensions;

public static class AutomationElementExtension
{
    public static IEnumerable<AutomationElement> GetChildren(AutomationElement element, bool reverse = false)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        if (!reverse)
            foreach (AutomationElement e in element.FindAll(TreeScope.TreeScope_Children, Condition.TrueCondition))
                yield return e;
        for (var last = TreeWalker.ControlViewWalker.GetLastChild(element);
             last != null;
             last = TreeWalker.ControlViewWalker.GetPreviousSibling(last))
            yield return last;
    }

    public static AutomationElement? FindChildByCondition(this AutomationElement element, Condition condition)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        var result = element.FindFirst(
            TreeScope.TreeScope_Children,
            condition);

        return result;
    }

    public static void ScrollIntoView(this AutomationElement element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        if (!element.Current.IsOffscreen) return;
        if (!(bool)element.GetCurrentPropertyValue(AutomationElement.IsScrollItemPatternAvailableProperty)) return;
        var scrollItemPattern = element.GetScrollItemPattern();
        scrollItemPattern?.ScrollIntoView();
    }

    public static ScrollItemPattern? GetScrollItemPattern(this AutomationElement element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        return element.GetPattern<ScrollItemPattern>(ScrollItemPattern.Pattern);
    }

    public static T? GetPattern<T>(this AutomationElement element, AutomationPattern pattern) where T : BasePattern
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        var patternObject = element.GetCurrentPattern(pattern);
        return patternObject as T;
    }
}