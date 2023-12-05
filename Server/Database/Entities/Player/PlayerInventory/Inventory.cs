using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using Newtonsoft.Json;

namespace Server.Database.Entities.Player.PlayerInventory;

public class Inventory{
    private static readonly Dictionary<string, List<InventoryItem>> Inventories =
        new Dictionary<string, List<InventoryItem>>();

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

        public static InventoryItem DeserializeFromJson(string json){
            return JsonConvert.DeserializeObject<InventoryItem>(json);
        }

        public string SerializeToJson(){
            return JsonConvert.SerializeObject(this);
        }

        public override string ToString(){
            return $"Amount: {Amount}, MaxAmount: {MaxAmount}, ItemName: {ItemName}";
        }
    }

    private static string ConvertInventoryToJson(string playerName){
        bool success = Inventories.TryGetValue(playerName, out List<InventoryItem> items);
        return success ? JsonConvert.SerializeObject(items) : "{}";
    }

    public void sendInventoryToPlayer(CitizenFX.Core.Player player){
        string playerJsonInventory = ConvertInventoryToJson(player.Name);
        player.TriggerEvent(playerJsonInventory);
    }

    public static void UseItem(string playerName, ItemID id, int amount = 1){
        if (!Inventories.TryGetValue(playerName, out List<InventoryItem> inventoryItems)){
            Inventories[playerName] = new List<InventoryItem>();
            inventoryItems = Inventories[playerName];
        }

        if (inventoryItems == null) return;
        foreach (InventoryItem inventoryItem in inventoryItems.Where(inventoryItem => inventoryItem.Item.Id == id)){
            if (inventoryItem.Amount < amount)
                return;

            inventoryItem.Item?.Use();
            inventoryItem.Amount -= amount;
        }
    }

    public static void AddItem(string playerName, Item item, int amount){
        if (!Inventories.TryGetValue(playerName, out List<InventoryItem> inventoryItems)){
            Inventories[playerName] = new List<InventoryItem>();
            inventoryItems = Inventories[playerName];

            Debug.WriteLine("If non inventory for player");
        }

        if (!CheckIfPlayerHasItem(playerName, item.Id)){ // Player doesn't have the item, add a new InventoryItem
            if (inventoryItems.Sum(x => x.Amount) + amount > item.MaxAmount){
                // Prevent adding more items than MaxAmount
                Trace.Log("More item then MaxAmount, return;");
                return;
            }

            Trace.Log("Add InventoryItem");
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
            Trace.Log("+= amount: " + amount);
        }
    }

    public static void TakeItem(string playerName, Item item, int amount){
        if (!Inventories.TryGetValue(playerName, out List<InventoryItem> inventoryItems)){
            Inventories[playerName] = new List<InventoryItem>();
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


    public static InventoryItem GetItemFromInventory(string playerName, ItemID id){
        return CheckIfPlayerHasItem(playerName, id) ? Inventories[playerName].Find(item => item.Item.Id == id) : null;
    }

    public static bool CheckIfPlayerHasItem(string playerName, ItemID id){
        if (Inventories.TryGetValue(playerName, out List<InventoryItem> inventoryItems))
            return inventoryItems.Any(inventoryItem => inventoryItem.Item.Id == id);
        Inventories[playerName] = new List<InventoryItem>();
        return false;
    }

    public class Item{
        public ItemID Id{ get; }
        public string ItemName{ get; }
        private Action UseAction{ get; }
        public int MaxAmount{ get; set; }

        public Item(ItemID id, int maxAmount, string itemName, Action useAction){
            Id = id;
            ItemName = itemName;
            UseAction = useAction;
            MaxAmount = maxAmount;
        }

        public void Use(){
            Console.WriteLine($"Using {ItemName}...");
            UseAction?.Invoke();
        }
    }
}