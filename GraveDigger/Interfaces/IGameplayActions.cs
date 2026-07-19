using System;
using System.Collections.Generic;
using GraveDigger.Characters;
using GraveDigger.Data;
using GraveDigger.GraveSites;
using GraveDigger.Items;
using GraveDigger.Props;
using GraveDigger.Systems;

namespace Interfaces;

public interface IGameplayActions
{ 
    void OpenTombstone(GraveSite graveSite);
    void DigGrave(GraveSite graveSite);
    void RepairGrave(GraveSite graveSite);
    
    void PickupItem(ItemData itemData);
    void SellItem(ItemData itemData, int amount);
    void BuyItem(ItemData itemData, int amount);
    void UseItem(ItemData itemData, int amount);
    void DiscardItem(ItemData itemData, int amount);
    
    void RecalculateReputation(IEnumerable<Prop> props);
    void ShowMarket(Merchant merchant);
}