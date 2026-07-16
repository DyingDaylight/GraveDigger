using System;
using GraveDigger.Data;
using GraveDigger.Items;
using GraveDigger.Props;

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
    
    public event Action<Tombstone> OnGraveDug;
    public event Action<Tombstone> OnGraveRepaired;
}