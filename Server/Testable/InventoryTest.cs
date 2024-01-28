using Inventory.InventorySlot;
using Server.Entities;
using Server.Services;

namespace Server.Testable;

public class InventoryTest
{

    private static InventoryService InventoryService;
    public InventoryTest() {
        InventoryService = ServiceManager.InventoryService;
        EventDispatcher.Mount("player:inventory:add_item", new Action<Player>(AddItem));
        EventDispatcher.Mount("player:inventory:take_item", new Action<Player>(TakeItem));

        //EventDispatcher.Mount("afterLoad", new Action<string>(HandleThis));
    }

    public static void HandleThis(string playerName){
        Player player = Main.Instance.PlayerList()[playerName];
        Debug.WriteLine("HT1");
        AddItem(player);
    }
    
    private static void TakeItem([FromSource]Player player){
        var playerName = player.Name;
        //VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        InventoryService.TakeItem(playerName, Items.Bread, 2);
        InventorySlot item = InventoryService.GetItemFromInventory(playerName, ItemID.BREAD);
        bool hasItem = InventoryService.CheckIfPlayerHasItem(playerName, ItemID.BREAD);
        Debug.WriteLine($"HasItem: {hasItem}, Item: {item}");
    }
    private static void AddItem([FromSource]Player player){
        //VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        var playerName = player.Name;
        InventoryService.AddItem(playerName, Items.Bread, 5);
        InventoryService.AddItem(playerName, Items.Cigarettes, 3);
/*
        InventorySlot bread = InventoryService.GetItemFromInventory(playerName, ItemID.BREAD);
        InventorySlot cola = InventoryService.GetItemFromInventory(playerName, ItemID.COLA);
        bool hasItemBread = InventoryService.CheckIfPlayerHasItem(playerName, ItemID.BREAD);
        bool hasItemCola = InventoryService.CheckIfPlayerHasItem(playerName, ItemID.COLA);
        Debug.WriteLine($"Bread: HasItem: {hasItemBread}, Item: {bread}");
        Debug.WriteLine($"Cola: HasItem: {hasItemCola}, Item: {cola}");*/
    }
}