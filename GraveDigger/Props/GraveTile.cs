using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Props;

public class GraveTile : Prop, IDailyUpdatable
{
    private int daysSinceConditionChange = 0;

    public int DecayInterval { get; set; }
    public GraveSiteState State
    {
        get;
        set
        {
            if (field == value)
                return;

            field = value;
            daysSinceConditionChange = 0;
            UpdateVisuals();
        }
    }

    public GraveTile(string name) : base(name)
    {
        CastShadow = false;
        Mode = SortingMode.Fixed;
    }
    
    public override void Start()
    {
        base.Start();
        UpdateVisuals();
    }

    public void AdvanceDay(int day)
    {
        daysSinceConditionChange++;

        if (daysSinceConditionChange < DecayInterval)
            return;

        daysSinceConditionChange = 0;
        DecreaseCondition();
    }

    private void DecreaseCondition()
    {
        State = State switch
        {
            GraveSiteState.Intact => GraveSiteState.Broken,
            _ => State
        };
    }

    private void UpdateVisuals()
    {
        ChangeSprite(State switch {
            GraveSiteState.DugOut => "grave_digged",
            GraveSiteState.Broken => "grave_broken",
            _ => "grave_earth"
        });
    }
}