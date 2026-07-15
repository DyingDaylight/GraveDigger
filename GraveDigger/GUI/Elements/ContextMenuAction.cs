using System;

namespace GraveDigger.GUI.Elements;

public class ContextMenuAction
{
    public string Name { get; }
    public Action Callback { get; }
    
    public ContextMenuAction(string name, Action callback)
    {
        Name = name;
        Callback = callback;
    }
    
    public void Execute()
    {
        Callback?.Invoke();
    }
}