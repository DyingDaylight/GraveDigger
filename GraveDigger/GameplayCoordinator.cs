using System;
using GraveDigger.Data;
using GUI;

namespace GraveDigger;

public class GameplayCoordinator
{
    public Gui Gui { get; }

    public GameplayCoordinator(Gui gui)
    {
        Gui = gui;
    }

    public void TombstoneSubscriber(TombstoneData obj)
    {
        Gui.WindowManager.OpenTombstoneInfoWindow(obj);
    }

    public void TombstoneActions(object obj)
    {
        
    }
}