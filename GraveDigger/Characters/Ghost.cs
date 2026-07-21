using System;
using GraveDigger.Core;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Characters;

public class Ghost : Animation, IReputationContributor
{
    public Ghost() : base("ghost")
    {
        
    }

    public override void Start()
    {
        base.Start();
        
    }

    public int GetReputationValue()
    {
        return -25;
    }
}