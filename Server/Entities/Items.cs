using Inventory.InventorySlot;
using Server.Utils;

namespace Server.Entities;

public static class Items {
    //If u adding new item, dont forgot to add into "enum" in Inventory.InventorySlot.ItemID
    
    public static readonly Item Bread =
        new Item(ItemID.BREAD, 10, "Bread","You can eat it.", () =>
        {
            Trace.Log("Use BREAD");
        });

    public static readonly Item Cola =
        new Item(ItemID.COLA, 5, "Cola's coke", "You can drink it.", () =>
        {
            Trace.Log("Use COLA");
        });
    
    public static readonly Item Cigarettes =
        new Item(ItemID.CIGARETTE, 20, "Cigarettes", "You can smoke it.", () =>
        {
            Trace.Log("U are smoking a cigarette");
        });
    
}