using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FxEvents.Shared;
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
    
    private async void AddDefaultItems([FromSource] Player player) {
        string playerName = player.Name;
        AddItem(playerName, Items.Bread, 0);
        AddItem(playerName, Items.Cola, 0);
        AddItem(playerName, Items.Cigarettes, 0);

        await LoadInventoryPlayer(playerName);
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
    
    public async Task LoadInventoryPlayer(string playerName)
    {
        if (await CheckIfInventoryPlayerExists(playerName))
        {
            PlayerInventory playerInventory = await GetInventoryRecord(playerName);
            Trace.Log($"PlayerInventory from LoadInventoryPlayer: {playerInventory.Inventory}");

            // Deserialize the JSON string into a list of InventoryJsonConverter
            List<PlayerInventory.InventoryJsonConverter> inventoryList = JsonConvert.DeserializeObject<List<PlayerInventory.InventoryJsonConverter>>(playerInventory.Inventory);

            if (inventoryList != null)
            {
                foreach (var inventoryItem in inventoryList)
                {
                    switch (inventoryItem.ItemId)
                    {
                        case 0:
                        {
                            // BREAD
                            AddItem(playerName, Items.Bread, inventoryItem.Amount);
                            break;
                        }
                        case 1:
                        {
                            // COLA
                            AddItem(playerName, Items.Cola, inventoryItem.Amount);
                            break;
                        }
                        case 2:
                        {
                            // CIGARETTES
                            AddItem(playerName, Items.Cigarettes, inventoryItem.Amount);
                            break;
                        }
                    }
                }
            }
        }
    }

    
/*
    public async Task LoadInventoryPlayer(string playerName) {
        if (await CheckIfInventoryPlayerExists(playerName)) {
            PlayerInventory playerInventory = await GetInventoryRecord(playerName);
            Trace.Log($"PlayerInventory from LoadInventoryPlayer: {playerInventory.Inventory}");
            object inventoryObject = JsonConvert.DeserializeObject(playerInventory.Inventory);
            JArray arrayInventory = (JArray)inventoryObject;
            if (arrayInventory != null)
                foreach (var slot in arrayInventory) {
                    
                    //slot[0]//amount
                    //slot[1]//itemId
                    switch ((int)slot[1])
                    {
                        case 0:
                        {
                            // BREAD
                            AddItem(playerName, Items.Bread, (int)slot[0]);
                            break;
                        }
                        case 1:
                        {
                            // COLA
                            AddItem(playerName, Items.Cola, (int)slot[0]);
                            break;
                        }
                        case 2:
                        {
                            // CIGARETTES
                            AddItem(playerName, Items.Cigarettes, (int)slot[0]);
                            break;
                        }
                    }
                }

            //List<PlayerInventory.InventoryJsonConverter> playerInventoryList = playerInventory.Inventory.FromJson<List<PlayerInventory.InventoryJsonConverter>>();
            //Trace.Log($"{playerInventoryList}");
            /*foreach (PlayerInventory.InventoryJsonConverter inventoryJsonConverter in playerInventoryList) {
                switch( inventoryJsonConverter.ItemId) {
                    case 0: { // BREAD
                        AddItem(playerName, Items.Bread, inventoryJsonConverter.Amount);
                        break;
                    }
                    case 1: { // COLA
                        AddItem(playerName, Items.Cola, inventoryJsonConverter.Amount);
                        break;
                    }
                    case 2: { // CIGARETTES
                        AddItem(playerName, Items.Cigarettes, inventoryJsonConverter.Amount);
                        break;
                    }

                }
            }*
        }
    }*/
    
    public async Task UpdateInventoryPlayer(string playerName) {
        //Player player = Main.Instance.PlayerList()[playerName];
        List<InventorySlot> inventory = Inventories[playerName];
        List<PlayerInventory.InventoryJsonConverter> convertedJson = 
            inventory.Select(inventorySlot => new PlayerInventory.InventoryJsonConverter(inventorySlot.Amount, (int)inventorySlot.Item.Id)).ToList();
        
        //string inventoryJson = ConvertInventoryOfPlayerToJson(playerName);
        int AccId = PlayerService.GetVgPlayerByName(playerName).Id;
        
        PlayerInventory playerInventory = new PlayerInventory(AccId, playerName, convertedJson.ToJson());
        if (await CheckIfInventoryPlayerExists(playerName)) 
            await UpdateInventoryRecord(playerInventory);
        else
            await InsertInventoryRecord(playerInventory);
    }

    private static async Task UpdateInventoryRecord(PlayerInventory playerInventory) {
        await using MySqlConnection connection = DatabaseConnector.GetConnection();
        await connection.OpenAsync();
        const string updateQuery = @"UPDATE inventory SET Inventory = @Inventory WHERE PlayerName = @PlayerName";
        await connection.ExecuteAsync(updateQuery, playerInventory);
        await connection.CloseAsync();
    }

    private static async Task<bool> CheckIfInventoryPlayerExists(string playerName) {
        await using MySqlConnection connection = DatabaseConnector.GetConnection();
        await connection.OpenAsync();
        const string checkExistenceQuery = "SELECT COUNT(*) FROM inventory WHERE PlayerName = @PlayerName";
        int recordCount = await connection.QueryFirstAsync<int>(checkExistenceQuery, new { PlayerName = playerName });
        await connection.CloseAsync();
        return recordCount > 0;
    }

    private static async Task<PlayerInventory> GetInventoryRecord(string playerName) {
        await using MySqlConnection connection = DatabaseConnector.GetConnection();
        await connection.OpenAsync();
        const string selectQuery = "SELECT * FROM inventory WHERE PlayerName = @PlayerName";
        var result = await connection.QueryFirstOrDefaultAsync<PlayerInventory>(selectQuery, new { PlayerName = playerName });
        await connection.CloseAsync();
        return result;
    }
    
    private static async Task InsertInventoryRecord(PlayerInventory inventory) {
        await using MySqlConnection connection = DatabaseConnector.GetConnection();
        await connection.OpenAsync();
        const string insertQuery = @"INSERT INTO inventory (AccId, PlayerName, Inventory) VALUES (@AccId, @PlayerName, @Inventory)";
        await connection.ExecuteAsync(insertQuery, inventory);
        await connection.CloseAsync();
    }
    
}