using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Inventory.InventorySlot;
using MySqlConnector;
using Newtonsoft.Json;
using Server.Database;
using Server.Database.Entities.Player;
using Server.Entities;
using Server.Utils;

namespace Server.Services;

public class InventoryService: IService {
    
    private static readonly Dictionary<string, List<InventorySlot>> Inventories = new Dictionary<string, List<InventorySlot>>();
    
    public void Init() {// Events from client
        EventDispatcher.Mount("player:inventory:use", new Action<string, int>(UseItem));
        EventDispatcher.Mount("playerConnected", new Action<Player>(AddDefaultItems));
    }
    
    private void AddDefaultItems([FromSource] Player player) {
        string playerName = player.Name;
        AddItem(playerName, Items.Bread, 0);
        AddItem(playerName, Items.Cola, 0);
        AddItem(playerName, Items.Cigarettes, 0);
    }
    
    public string ConvertInventoryOfPlayerToJson(string playerName){
        bool success = Inventories.TryGetValue(playerName, out List<InventorySlot> items);
        Trace.Log(JsonConvert.SerializeObject(items));
        return success ? JsonConvert.SerializeObject(items) : "{}";
    }

    private void UseItem(string playerName, int itemId) {
        ItemID itemIdEnum = (ItemID)itemId;
        UseItem(playerName, itemIdEnum, amount: 1);
        Trace.Log($"[ServerSide]  UsingItem: {itemId} of player {playerName}");
    }
    
    public void UseItem(string playerName, ItemID id, int amount = 1) {
        Trace.Log($"[ServerSide]  UsingItem: {id} of player {playerName}");
        Player player = Main.Instance.PlayerList()[playerName];
        if (!Inventories.TryGetValue(playerName, out List<InventorySlot> inventorySlots)){
            Inventories[playerName] = new List<InventorySlot>();
            inventorySlots = Inventories[playerName];
        }

        if (inventorySlots == null) return;
        foreach (InventorySlot slot in inventorySlots.Where(inventorySlot => (int)inventorySlot.Item.Id == (int)id)){
            if (slot.Amount < amount)
                return;

            if (slot.Amount <= 0) {
                EventDispatcher.Send(player, "player:sound:playfrontend", "NO", "HUD_FRONTEND_DEFAULT_SOUNDSET"); //FIXME: Make only for specific player
            }

            slot.Item?.Use();
            slot.Amount -= amount;
        }
    }

    public void AddItem(string playerName, Item item, int amount){
        Trace.Log($"playerName: {playerName}, Item: {item}, amount: {amount}");
        if (!Inventories.TryGetValue(playerName, out List<InventorySlot> inventorySlots)){
            Inventories[playerName] = new List<InventorySlot>();
            inventorySlots = Inventories[playerName];
        }

        if (!CheckIfPlayerHasItem(playerName, item.Id)){ // Player doesn't have the item, add a new InventorySlot
            if (amount > item.MaxAmount) {
                throw new ArgumentException($"You cannot add more items({(item.MaxAmount+amount)}) then max amount({item.MaxAmount})!");
            }
            
            inventorySlots.Add(new InventorySlot(item, amount));
        }else{
            // Player already has the item, increment the amount
            InventorySlot existingItem = inventorySlots.FirstOrDefault(i => (int)i.Item.Id == (int)item.Id);
            if (existingItem == null) return;
            if (existingItem.Amount + amount > item.MaxAmount){ // Prevent adding more items than MaxAmount
                throw new ArgumentException($"You cannot add more items({(item.MaxAmount+amount)}) then max amount({item.MaxAmount})!");
            }

            existingItem.Amount += amount;
        }
    }

    public void TakeItem(string playerName, Item item, int amount){
        if (!Inventories.TryGetValue(playerName, out List<InventorySlot> inventorySlots)){
            Inventories[playerName] = new List<InventorySlot>();
            return;
        }

        // Check if the player has the item and consume the amount if available
        foreach (InventorySlot inventorySlot in inventorySlots.Where(inventorySlot => (int)inventorySlot.Item.Id == (int)item.Id)){
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


    public InventorySlot GetItemFromInventory(string playerName, ItemID id){
        return CheckIfPlayerHasItem(playerName, id) ? Inventories[playerName].Find(item => (int)item.Item.Id == (int)id) : null;
    }

    public bool CheckIfPlayerHasItem(string playerName, ItemID id){
        if (Inventories.TryGetValue(playerName, out List<InventorySlot> inventorySlots))
            return inventorySlots.Any(inventorySlot => (int)inventorySlot.Item.Id == (int)id);
        Inventories[playerName] = new List<InventorySlot>();
        return false;
    }
    
    public async Task UpdateInventoryPlayer(string playerName) {
        //Player player = Main.Instance.PlayerList()[playerName];
        //List<InventorySlot> inventory = Inventories[playerName];
        string inventoryJson = ConvertInventoryOfPlayerToJson(playerName);
        int AccId = PlayerService.GetVgPlayerByName(playerName).Id;
        
        PlayerInventory playerInventory = new PlayerInventory(AccId, playerName, inventoryJson);

        if (await CheckIfInventoryPlayerExists(playerName))
            await UpdateInventoryRecord(playerInventory);
        else
            await InsertInventoryRecord(playerInventory);
    }

    private static async Task UpdateInventoryRecord(PlayerInventory item) {
        await using MySqlConnection connection = DatabaseConnector.GetConnection();
        await connection.OpenAsync();
    
        const string updateQuery = @"UPDATE inventory SET AccId = @AccId, Inventory = @Inventory WHERE Name = @PlayerName";
    
        await connection.ExecuteAsync(updateQuery, new 
        {
            item.AccId,
            item.PlayerName,
            item.Inventory
        });

        await connection.CloseAsync();
    }

    private static async Task<bool> CheckIfInventoryPlayerExists(string playerName) {
        await using MySqlConnection connection = DatabaseConnector.GetConnection();
        await connection.OpenAsync();
    
        const string checkExistenceQuery = "SELECT COUNT(*) FROM inventory WHERE Name = @PlayerName";
        int recordCount = await connection.QueryFirstAsync<int>(checkExistenceQuery, new { PlayerName = playerName });
        await connection.CloseAsync();

        return recordCount > 0;
    }

    private static async Task<PlayerInventory> GetInventoryRecord(string playerName) {
        await using MySqlConnection connection = DatabaseConnector.GetConnection();
        await connection.OpenAsync();

        const string selectQuery = "SELECT * FROM inventory WHERE Name = @PlayerName";
    
        var result = await connection.QueryFirstOrDefaultAsync<PlayerInventory>(selectQuery,
            new { PlayerName = playerName });

        await connection.CloseAsync();

        return result;
    }
    
    private static async Task InsertInventoryRecord(PlayerInventory item) {
        await using MySqlConnection connection = DatabaseConnector.GetConnection();
        await connection.OpenAsync();
    
        const string insertQuery = @"
        INSERT INTO inventory (AccId, Name, Inventory) 
        VALUES (@AccId, @PlayerName, @Inventory)";
    
        await connection.ExecuteAsync(insertQuery, new 
        {
            AccId = item.AccId,
            PlayerName = item.PlayerName,
            Inventory = item.Inventory
        });

        await connection.CloseAsync();
    }
    
}