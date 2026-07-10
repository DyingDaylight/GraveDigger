using System;
using GraveDigger.Data;
using GraveDigger.Props;
using GUI;
using Interfaces;

namespace GraveDigger;

public class GameplayCoordinator : IGameplayActions
{
    public IGameWindowService WindowService { get; }

    public GameplayCoordinator(IGameWindowService windowService)
    {
        WindowService = windowService;
    }

    public void OpenTombstone(Tombstone tombstoneData)
    {
        WindowService.OpenTombstoneWindow(tombstoneData);
        
    }

    public void DigGrave(Tombstone tombstoneData)
    {   
        Console.WriteLine("Digging Grave");
    }

    public void RepairGrave(Tombstone tombstone)
    {
        Console.WriteLine("Repairing Grave");
        // TODO: check if we have resources
        bool repaired = tombstone.Repair();
        if (repaired)
        {
            WindowService.UpdateTombstoneWindow();
        }
    }
}