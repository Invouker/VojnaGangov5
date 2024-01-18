using Server.Database.Entities.Player.PlayerInventory;

namespace Server.Testable;

public class InventoryTest{
    public InventoryTest(){
        Debug.WriteLine(":) static");
        // EventDispatcher.Mount("player:inventory:add_item", new Action<Player>(AddItem));
        // EventDispatcher.Mount("player:inventory:take_item", new Action<Player>(TakeItem));

         EventDispatcher.Mount("afterLoad", new Action<string>(HandleThis));
    }

    public static void HandleThis(string playerName){
        //Player player = Main.Instance.PlayerList()[playerName];
        Debug.WriteLine("HT1");
        AddItem(playerName);
        Debug.WriteLine("HT2");
        TakeItem(playerName);
        Debug.WriteLine("HT3");

        UseItem(playerName);
    }

    private static void TakeItem(string player){
        //VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        Inventory.TakeItem(player, Items.Bread, 2);
        Inventory.InventorySlot item = Inventory.GetItemFromInventory(player, ItemID.BREAD);
        bool hasItem = Inventory.CheckIfPlayerHasItem(player, ItemID.BREAD);
        Debug.WriteLine($"HasItem: {hasItem}, Item: {item}");
    }

    private static void UseItem(string player) {
        Inventory.UseItem(player, ItemID.COLA);
    }

    private static void AddItem(string player){
        //VGPlayer vgPlayer = PlayerService.GetVgPlayerByPlayer(player);
        Inventory.AddItem(player, Items.Bread, 10);
        Inventory.AddItem(player, Items.Cola, 2);

        Inventory.InventorySlot bread = Inventory.GetItemFromInventory(player, ItemID.BREAD);
        Inventory.InventorySlot cola = Inventory.GetItemFromInventory(player, ItemID.COLA);
        bool hasItemBread = Inventory.CheckIfPlayerHasItem(player, ItemID.BREAD);
        bool hasItemCola = Inventory.CheckIfPlayerHasItem(player, ItemID.COLA);
        Debug.WriteLine($"Bread: HasItem: {hasItemBread}, Item: {bread}");
        Debug.WriteLine($"Cola: HasItem: {hasItemCola}, Item: {cola}");
    }
}