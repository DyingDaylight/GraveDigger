using System.Collections.Generic;
using System.Linq;
using GraveDigger.GraveSites;
using GraveDigger.Props;
using GraveDigger.Utils;

namespace GraveDigger.Systems;

public class GraveDecaySystem
{
    private const float DecayChance = 0.5f;
    private readonly RandomService random;

    public GraveDecaySystem(RandomService  random)
    {
        this.random = random;
    }

    public void DecayGraves(IReadOnlyList<GraveSite> graveSites) 
    {
        bool shouldDecay = random.Chance(DecayChance);

        if (!shouldDecay)
            return;
        
        List<GraveSite> availableGraveSites = graveSites
            .Where(graveSite => graveSite.Status == GraveSiteStatus.Occupied 
                                && graveSite.State == GraveState.Intact)
            .ToList();
        
        if (availableGraveSites.Count == 0)
            return;
        
        GraveSite graveSite = random.Pick(availableGraveSites);
        graveSite.DecreaseCondition();
    }
}