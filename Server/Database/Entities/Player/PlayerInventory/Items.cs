using CitizenFX.Core;

namespace Server.Database.Entities.Player.PlayerInventory{
    public static class Items{
        public static readonly Inventory.Item Bread =
            new Inventory.Item(ItemID.BREAD, 10, "Bread", () => { Debug.WriteLine("Use BREAD"); });

        public static readonly Inventory.Item Cola =
            new Inventory.Item(ItemID.COLA, 5, "Cola's coke", () => { Debug.WriteLine("Use COLA"); });
    }
}