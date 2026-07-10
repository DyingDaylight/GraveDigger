using System;
using GraveDigger.Data;
using GraveDigger.Props;
using GUI;
using Interfaces;

namespace GraveDigger;

public class GameplayCoordinator : IGameplayActions
{
    public IGameWindowService WindowService { get; }
    
    private ReputationsSystem ReputationsSystem { get; }

    public GameplayCoordinator(IGameWindowService windowService, ReputationsSystem reputationsSystem)
    {
        WindowService = windowService;
        ReputationsSystem = reputationsSystem;
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
        // TODO: check if we have resources
        bool repaired = tombstone.Repair();
        if (repaired)
        {
            ReputationsSystem.AddReputation(tombstone.Value);
            WindowService.UpdateTombstoneWindow();
        }
    }
}