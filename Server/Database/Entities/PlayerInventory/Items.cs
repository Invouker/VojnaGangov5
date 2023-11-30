using CitizenFX.Core;

namespace Server.Database.Entities.PlayerInventory{
    public static class Items{
        public static Inventory.Item Bread =
            new Inventory.Item(ItemID.BREAD, "Bread", () => { Debug.WriteLine("Use BREAD"); });

        public static Inventory.Item Cola =
            new Inventory.Item(ItemID.COLA, "Cola's coke", () => { Debug.WriteLine("Use COLA"); });
    }
}