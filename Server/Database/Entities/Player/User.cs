namespace Server.Database.Entities.Player;

public class User{
    public const string TABLE_NAME = "users";

    public int Id{ get; set; }

    public string Name{ get; set; }
    public string License{ get; set; }

    public string Ip{ get; set; }
    public string Token{ get; set; }
    //public string ScName{ get; set; } // https://docs.fivem.net/natives/?_0x198D161F458ECC7F

    public User(){ }

    public User(string name, string license, string ip, string token){
        Name = name;
        License = license;
        Ip = ip;
        Token = token;
    }
}