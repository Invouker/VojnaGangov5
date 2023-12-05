using System;
using CitizenFX.Core;
using Server.Database.Entities.Player.PlayerInventory;

namespace Server.Testable;

public class InventoryTest{
    public InventoryTest(){
        Debug.WriteLine(":) static");
        //Main.Instance.AddEventHandler("player:inventory:add_item", new Action<Player>(AddItem));
        //Main.Instance.AddEventHandler("player:inventory:take_item", new Action<Player>(TakeItem));

        Main.Instance.AddEventHandler("afterLoad", new Action<string>(HandleThis));
    }

    public static void HandleThis(string playerName){
        //Player player = Main.Instance.PlayerList()[playerName];
        Debug.WriteLine("HT1");
        AddItem(playerName);
        Debug.WriteLine("HT2");
        TakeItem(playerName);
        Debug.WriteLine("HT3");
    }

    private static void TakeItem(string player){
        //VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        Inventory.TakeItem(player, Items.Bread, 2);
        Inventory.InventoryItem item = Inventory.GetItemFromInventory(player, ItemID.BREAD);
        bool hasItem = Inventory.CheckIfPlayerHasItem(player, ItemID.BREAD);
        Debug.WriteLine($"HasItem: {hasItem}, Item: {item}");
    }

    private static void AddItem(string player){
        //VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        Inventory.AddItem(player, Items.Bread, 10);

        Inventory.InventoryItem item = Inventory.GetItemFromInventory(player, ItemID.BREAD);
        bool hasItem = Inventory.CheckIfPlayerHasItem(player, ItemID.BREAD);
        Debug.WriteLine($"HasItem: {hasItem}, Item: {item}");
    }
}