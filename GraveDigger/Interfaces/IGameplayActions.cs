using System;
using System.Collections.Generic;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Systems;

namespace Interfaces;

public interface IGameplayActions
{ 
    void OpenTombstone(Tombstone tombstone);
    void DigGrave(Tombstone tombstone);
    void RepairGrave(Tombstone tombstone);
    
    void PickupItem(ItemData itemData);
    void SellItem(ItemData itemData, int amount);
    void BuyItem(ItemData itemData, int amount);
    void UseItem(ItemData itemData, int amount);
    void DiscardItem(ItemData itemData, int amount);
    
    void CalculateInitialReputation(List<Prop> props);
    
    public event Action<Tombstone> OnGraveDug;
    public event Action<Tombstone> OnGraveRepaired;
}