using System.Collections.Concurrent;

namespace Tenon.Automation.Abstractions;

public abstract class HighlightBehavior
{
    protected HighlightBehavior()
    {

    }
    public abstract bool StartHook();

    public abstract void StopHook();
}