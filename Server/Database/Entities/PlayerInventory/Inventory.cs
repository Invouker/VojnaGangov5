using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Database.Entities.PlayerInventory;

public class Inventory{
    public static readonly Dictionary<VGPlayer, List<InventoryItem>> Inventories =
        new Dictionary<VGPlayer, List<InventoryItem>>();

    public List<InventoryItem> Items = new List<InventoryItem>();

    public class InventoryItem{
        public int Amount{ get; set; }
        public int MaxAmount;
        public string ItemName;
        public Item Item{ get; set; }

        public InventoryItem(Item item, int amount){
            Item = item;
            Amount = amount;
            ItemName = item.ItemName;
            MaxAmount = item.MaxAmount;
        }
    }

    public static void UseItem(VGPlayer vgPlayer, ItemID id, int amount = 1){
        if (!Inventories.ContainsKey(vgPlayer))
            Inventories.Add(vgPlayer, new List<InventoryItem>());
        Inventories.TryGetValue(vgPlayer, out List<InventoryItem> inventoryItems);
        if (inventoryItems == null) return;
        foreach (InventoryItem inventoryItem in inventoryItems.Where(inventoryItem => inventoryItem.Item.Id == id)){
            if (inventoryItem.Amount < amount)
                return;

            inventoryItem.Item?.Use();
            inventoryItem.Amount -= amount;
        }
    }

    public static void AddItem(VGPlayer vgPlayer, Item item, int amount){
        if (!Inventories.TryGetValue(vgPlayer, out List<InventoryItem> inventoryItems)){
            Inventories[vgPlayer] = new List<InventoryItem>();
            inventoryItems = Inventories[vgPlayer];
        }

        if (!CheckIfPlayerHasItem(vgPlayer, item.Id)){ // Player doesn't have the item, add a new InventoryItem
            if (inventoryItems.Sum(x => x.Amount) + amount > item.MaxAmount){
                // Prevent adding more items than MaxAmount
                return;
            }

            inventoryItems.Add(new InventoryItem(item, amount));
        }
        else{
            // Player already has the item, increment the amount
            InventoryItem existingItem = inventoryItems.FirstOrDefault(i => i.Item.Id == item.Id);
            if (existingItem == null) return;
            if (existingItem.Amount + amount > item.MaxAmount){ // Prevent adding more items than MaxAmount
                return;
            }

            existingItem.Amount += amount;
        }
    }

    public static void TakeItem(VGPlayer vgPlayer, Item item, int amount){
        if (!Inventories.TryGetValue(vgPlayer, out List<InventoryItem> inventoryItems)){
            Inventories[vgPlayer] = new List<InventoryItem>();
            return;
        }

        // Check if the player has the item and consume the amount if available
        foreach (InventoryItem inventoryItem in
                 inventoryItems.Where(inventoryItem => inventoryItem.Item.Id == item.Id)){
            if (amount <= 0)
                break; // No more amount needed to be consumed

            if (inventoryItem.Amount >= amount){
                inventoryItem.Amount -= amount;
                amount = 0; // Consumed all needed amount
            }
            else{
                amount -= inventoryItem.Amount;
                inventoryItem.Amount = 0; // Fully consume this stack
            }
        }
    }


    public static bool CheckIfPlayerHasItem(VGPlayer vgPlayer, ItemID id){
        if (Inventories.TryGetValue(vgPlayer, out List<InventoryItem> inventoryItems))
            return inventoryItems.Any(inventoryItem => inventoryItem.Item.Id == id);
        Inventories[vgPlayer] = new List<InventoryItem>();
        return false;
    }

    /*
    public void AddItem(int amount, Item item) {
        items.Add(item);
    }

    public void RemoveItem(ItemID id) {
        Item itemToRemove = items.Find(item => item.Id == id);
        if (itemToRemove == null) return;
        items.Remove(itemToRemove);
        Console.WriteLine($"Removed {itemToRemove.ItemName} from inventory.");
    }*/


    public class Item{
        public ItemID Id{ get; }
        public string ItemName{ get; }
        private Action UseAction{ get; }
        public int MaxAmount{ get; set; }

        public Item(ItemID id, string itemName, Action useAction){
            Id = id;
            ItemName = itemName;
            UseAction = useAction;
        }

        public void Use(){
            Console.WriteLine($"Using {ItemName}...");
            UseAction?.Invoke();
        }
    }
}