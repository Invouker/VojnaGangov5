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
    
}