namespace Server.Database.Entities.Player;

public class PlayerInventory : IPlayerMetaData
{
    public int Id { get; set; }
    public int AccId { get; set; }
    public string PlayerName { get; set; }
    public string Inventory { get; set; }

    public PlayerInventory(int accId, string playerName, string inventory) {
        AccId = accId;
        PlayerName = playerName;
        Inventory = inventory;
    }

    public PlayerInventory() {
    }

    public override string ToString() {
        return $"Id: {Id},  AccId: {AccId}, PlayerName: {PlayerName}, Inventory: {Inventory}";
    }



    internal class InventoryJsonConverter {
        public int Amount { get; set; }
        public int ItemId { get; set; }

        public InventoryJsonConverter() {
        }

        public InventoryJsonConverter(int amount, int itemId) {
            Amount = amount;
            ItemId = itemId;
        }
    }
}