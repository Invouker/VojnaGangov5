using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Server.Database.Entities.Player.PlayerInventory;

public class Inventory{
    private static readonly Dictionary<string, List<InventorySlot>> Inventories = new Dictionary<string, List<InventorySlot>>();
    
    public class InventorySlot(Item item, int amount) {
        private static int _id;
        public int Amount{ get; set; } = amount;
        public Item Item{ get; set; } = item;
        public int SlotId { get; } = _id++;
        
        public override string ToString(){
            return $"Slot: {SlotId}, Amount: {Amount}, Item: {Item}";
        }
    }

    public static string ConvertInventoryOfPlayerToJson(string playerName){
        bool success = Inventories.TryGetValue(playerName, out List<InventorySlot> items);
        return success ? JsonConvert.SerializeObject(items) : "{}";
    }
    
    public static void UseItem(string playerName, ItemID id, int amount = 1){
        if (!Inventories.TryGetValue(playerName, out List<InventorySlot> inventorySlots)){
            Inventories[playerName] = new List<InventorySlot>();
            inventorySlots = Inventories[playerName];
        }

        if (inventorySlots == null) return;
        foreach (InventorySlot slot in inventorySlots.Where(inventorySlot => inventorySlot.Item.Id == id)){
            if (slot.Amount < amount)
                return;

            slot.Item?.Use();
            slot.Amount -= amount;
        }
    }

    public static void AddItem(string playerName, Item item, int amount){
        if (!Inventories.TryGetValue(playerName, out List<InventorySlot> inventorySlots)){
            Inventories[playerName] = new List<InventorySlot>();
            inventorySlots = Inventories[playerName];

            Trace.Log("If non inventory for player");
        }

        if (!CheckIfPlayerHasItem(playerName, item.Id)){ // Player doesn't have the item, add a new InventorySlot
            Trace.Log($"DEBUG MaxAmount: [{item}], Amount: {amount}");

            if (amount > item.MaxAmount) {
                Trace.Log("More item then MaxAmount, return;");
                return;
            }
            
            Trace.Log("Add InventorySlot");
            inventorySlots.Add(new InventorySlot(item, amount));
        }else{
            // Player already has the item, increment the amount
            InventorySlot existingItem = inventorySlots.FirstOrDefault(i => i.Item.Id == item.Id);
            if (existingItem == null) return;
            if (existingItem.Amount + amount > item.MaxAmount){ // Prevent adding more items than MaxAmount
                return;
            }

            existingItem.Amount += amount;
            Trace.Log("+= amount: " + amount);
        }
    }

    public static void TakeItem(string playerName, Item item, int amount){
        if (!Inventories.TryGetValue(playerName, out List<InventorySlot> inventorySlots)){
            Inventories[playerName] = new List<InventorySlot>();
            return;
        }

        // Check if the player has the item and consume the amount if available
        foreach (InventorySlot inventorySlot in inventorySlots.Where(inventorySlot => inventorySlot.Item.Id == item.Id)){
            if (amount <= 0)
                break; // No more amount needed to be consumed

            if (inventorySlot.Amount >= amount){
                inventorySlot.Amount -= amount;
                amount = 0; // Consumed all needed amount
            }
            else{
                amount -= inventorySlot.Amount;
                inventorySlot.Amount = 0; // Fully consume this stack
            }
        }
    }


    public static InventorySlot GetItemFromInventory(string playerName, ItemID id){
        return CheckIfPlayerHasItem(playerName, id) ? Inventories[playerName].Find(item => item.Item.Id == id) : null;
    }

    public static bool CheckIfPlayerHasItem(string playerName, ItemID id){
        if (Inventories.TryGetValue(playerName, out List<InventorySlot> inventorySlots))
            return inventorySlots.Any(inventorySlot => inventorySlot.Item.Id == id);
        Inventories[playerName] = new List<InventorySlot>();
        return false;
    }

    public class Item(ItemID id, int maxAmount, string itemName, Action useAction) {
        public ItemID Id{ get; } = id;
        public string ItemName{ get; } = itemName;
        private Action UseAction{ get; } = useAction;
        public int MaxAmount{ get; set; } = maxAmount;

        public void Use(){
            Console.WriteLine($"Using {ItemName}...");
            UseAction?.Invoke();
        }
        
        public override string ToString() {
            return $"Item ID: {Id}, Name: {ItemName}, Max Amount: {MaxAmount}";
        }
        
    }
}