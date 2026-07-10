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

    public void RepairGrave(Tombstone tombstoneData)
    {
        Console.WriteLine("Repairing Grave");
    }
}