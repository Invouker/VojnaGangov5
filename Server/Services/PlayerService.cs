using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using Server.Database;
using Server.Database.Entities.Player;
using Server.Database.Entities.Player.PlayerInventory;
using Server.Utils;

namespace Server.Services{
    public class PlayerService : IService{
        private static readonly Dictionary<string, Data> PlayerData = new Dictionary<string, Data>(); // nickname, Data

        public readonly Dictionary<string, PlayerSlot> PlayerSlots = new Dictionary<string, PlayerSlot>();

        public PlayerService(){
            Trace.Log("PlayerService initializing....");
             EventDispatcher.Mount("playerConnected", new Action<Player>(OnPlayerConnected));
             EventDispatcher.Mount("playerDropped", new Action<Player, string>(OnPlayerDropped));
             EventDispatcher.Mount("resourceStop", new Action<string>(OnResourceStop));
             EventDispatcher.Mount("player:interactive:walkingstyle",
                                          new Action<Player, int>(([FromSource] player, id) => {
                                              VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
                                              vgPlayer.WalkingStyle = id;
                                          }));


            new Thread(() => { new AutoSaver(); }).Start();
        }

        public static VGPlayer GetVgPlayerByPlayer(Player player){
            return GetVgPlayerByName(player.Name);
        }

        public static VGPlayer GetVgPlayerByName(string Name){
            return PlayerData[Name].VGPlayer;
        }

        #region Reputation

        public static int GetReputationToLevel(int level){
            return level * 827 + 1734 + level * 86;
        }

        private static void UpdateLevel(Player player){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);

            int nextLevel = vgPlayer.Level + 1;
            int requiredXPForNextLevel = GetReputationToLevel(nextLevel);
            while (vgPlayer.Xp >= requiredXPForNextLevel){
                vgPlayer.Xp -= requiredXPForNextLevel;
                vgPlayer.Level++;

                nextLevel++;
                requiredXPForNextLevel = GetReputationToLevel(nextLevel);
            }

            EventDispatcher.Send(player, "player:hud:update:xp", vgPlayer.Xp, vgPlayer.Level);
        }

        public static int GetXP(Player player){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            return vgPlayer.Xp;
        }

        public static void GiveXP(Player player, uint value){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            vgPlayer.Xp += (int)value;
            UpdateLevel(player);
            EventDispatcher.Send(player, "player:hud:update:xp", vgPlayer.Xp, vgPlayer.Level);
            EventDispatcher.Send(player, "player:hud:update:show:rank", value);
        }

        public static void TakeXP(Player player, uint value){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            vgPlayer.Xp -= (int)value;
            UpdateLevel(player);
            EventDispatcher.Send(player, "player:hud:update:xp", vgPlayer.Xp, vgPlayer.Level);
        }

        public static int GetLevel(Player player){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            return vgPlayer.Level;
        }

        public static void GiveLevel(Player player, uint value){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            vgPlayer.Level += (int)value;
            EventDispatcher.Send(player, "player:hud:update:xp", vgPlayer.Xp, vgPlayer.Level);
        }

        public static void TakeLevel(Player player, uint value){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            vgPlayer.Level -= (int)value;
            EventDispatcher.Send(player, "player:hud:update:xp", vgPlayer.Xp, vgPlayer.Level);
        }

        #endregion


        private static async Task<VGPlayer> LoadVGPlayer(Player player) {
            await using MySqlConnection connection = DatabaseConnector.GetConnection();
            await connection.OpenAsync();
            const string LoadPlayer = $"SELECT * FROM {VGPlayer.TABLE_NAME} WHERE Name = @Name;";
            VGPlayer vgPlayer =
                await connection.QueryFirstOrDefaultAsync<VGPlayer>(LoadPlayer,
                    new{ Name = player.Name });

            API.SetPlayerWantedLevel(player.Handle, vgPlayer.WantedLevel, true);
Trace.Log("player:load:data");
            EventDispatcher.Send(player, "player:load:data",
                vgPlayer.Dimension, vgPlayer.Hp, vgPlayer.MaxHp, vgPlayer.Armour,
                vgPlayer.MaxArmour, vgPlayer.Level, vgPlayer.Xp, vgPlayer.WalkingStyle);
            Debug.WriteLine($"Loaded data: {vgPlayer}");

            EventDispatcher.Send(player, "player:hud:update:money", 0, vgPlayer.Money);
            EventDispatcher.Send(player, "player:hud:update:money", 1, vgPlayer.BankMoney);

            await connection.CloseAsync();
            return vgPlayer;
        }

        private static async Task<User> LoadUser(Player player) {
            await using MySqlConnection connection = DatabaseConnector.GetConnection();
            await connection.OpenAsync();
            const string LoadPlayer = $"SELECT * FROM {User.TABLE_NAME} WHERE License = @License;";
            User user =
                await connection.QueryFirstOrDefaultAsync<User>(LoadPlayer,
                    new{ License = Util.GetLicense(player) });

            await connection.CloseAsync();
            return user;
        }

        private static async Task<VGPlayer> InsertVGPlayer(Player player) {
            await using MySqlConnection connection = DatabaseConnector.GetConnection();
            await connection.OpenAsync();
            const string insertQuery =
                $"""
                 INSERT INTO {VGPlayer.TABLE_NAME} (Name, License, WantedLevel, Money, BankMoney, Level, Xp, WalkingStyle, PosX, PosY, PosZ, Dimension)
                 											VALUES (@Name, @License, @WantedLevel, @Money, @BankMoney, @Level, @Xp, @WalkingStyle, @PosX, @PosY, @PosZ, @Dimension)
                 """;
            VGPlayer vgPlayer =
                new VGPlayer(player.Name, 100, 100, 0, 100, 0, 0, 0, 1, 0, 0, 0, 0, 0){
                    PosX = player.Character.Position.X,
                    PosY = player.Character.Position.Y,
                    PosZ = player.Character.Position.Z
                };

            await connection.ExecuteAsync(insertQuery, vgPlayer);
            await connection.CloseAsync();
            return vgPlayer;
        }

        private static async Task<User> InsertUser(Player player) {
            await using MySqlConnection connection = DatabaseConnector.GetConnection();
            await connection.OpenAsync();
            const string insertQuery =
                $"""
                 INSERT INTO {User.TABLE_NAME} (Name, License, Ip, Token)
                 											VALUES (@Name, @License, @Ip, @Token)
                 """;
            User user = new User(player.Name, Util.GetLicense(player), Util.GetIP(player),
                API.GetPlayerToken(player.Handle, 0));
            await connection.ExecuteAsync(insertQuery, user);
            await connection.CloseAsync();
            return user;
        }

        public static async void UpdateVGPlayer(Player player, string name) {
            await using MySqlConnection connection = DatabaseConnector.GetConnection();
            await connection.OpenAsync();
            const string UpdateQueryString = $"""
                                              UPDATE {VGPlayer.TABLE_NAME} SET Name = @Name,
                                                                                    Hp = @Hp, MaxHp = @MaxHp, Armour = @Armour,
                                                                                       MaxArmour = @MaxArmour, WantedLevel = @WantedLevel,
                                                                                       Money = @Money, BankMoney = @BankMoney, Level = @Level,
                                                                                       Xp = @Xp, WalkingStyle = @WalkingStyle,
                                                                                       PosX = @PosX, PosY = @PosY, PosZ = @PosZ, Dimension = @Dimension WHERE Name = @Name;
                                              """;

            if (PlayerData.TryGetValue(name, out Data playerData)){
                VGPlayer vgPlayer = playerData.VGPlayer;
                vgPlayer.PosX = player.Character.Position.X;
                vgPlayer.PosY = player.Character.Position.Y;
                vgPlayer.PosZ = player.Character.Position.Z;
                vgPlayer.Hp = API.GetEntityHealth(player.Character.Handle);
                vgPlayer.MaxHp = API.GetEntityMaxHealth(player.Character.Handle);
                vgPlayer.Armour = API.GetPedArmour(player.Character.Handle);
                vgPlayer.MaxArmour = API.GetPlayerMaxArmour(player.Handle);
                vgPlayer.Dimension = API.GetPlayerRoutingBucket(player.Handle);
                await connection.ExecuteAsync(UpdateQueryString, vgPlayer);
            }

            await connection.CloseAsync();
        }
        /*
                private static async Task<bool> CheckIfPlayerExists(Player player){
                    await using (MySqlConnection connection = DatabaseConnector.GetConnection()){
                        await connection.OpenAsync();
                        const string CheckExistenceQuery = $"SELECT COUNT(*) FROM {VGPlayer.TABLE_NAME} WHERE License = @License";
                        int recordCount = await connection.QueryFirstAsync<int>(CheckExistenceQuery, new{ License = Util.GetLicense(player) });
                        if (recordCount > 0)
                            return true;
                        await connection.CloseAsync();
                    }
                    return false;
                }*/

        private static async Task<bool> CheckIfUserExists(Player player){
            await using MySqlConnection connection = DatabaseConnector.GetConnection();
            await connection.OpenAsync();
            const string CheckExistenceQuery = $"SELECT COUNT(*) FROM {User.TABLE_NAME} WHERE License = @License";
            int recordCount =
                await connection.QueryFirstAsync<int>(CheckExistenceQuery,
                    new{ License = Util.GetLicense(player) });
            if (recordCount > 0)
                return true;
            await connection.CloseAsync();

            return false;
        }

        public void OnPlayerDropped([FromSource] Player player, string reason){
            Debug.WriteLine($"Player left {player.Name}({player.Handle}), Reason: {reason}");

            string playerName = player.Name;
            UpdateVGPlayer(player, playerName);
            if (!PlayerData.ContainsKey(playerName)) return;
            PlayerData.Remove(playerName);
            PlayerSlots.Remove(playerName);
        }

        public void OnResourceStop(string resource){
            if (resource != "vojnagangov5")
                return;
            Debug.WriteLine($"Resource {resource} has stopped");
            PlayerList playerList = Main.Instance.PlayerList();
            foreach (Player player in playerList){
                Debug.WriteLine($"Saving of {player.Name}.");
                UpdateVGPlayer(player, player.Name);
            }

            PlayerSlots.Clear();
            PlayerData.Clear();
        }

        public async void OnPlayerConnected([FromSource] Player player){
            Trace.Log($"{player.Name} connecting to the server.");
            string playerName = player.Name;

            if (PlayerData.ContainsKey(playerName)){
                player.Drop("Multiple account connected by one license.");
                return;
            }

            PlayerData.Add(player.Name, new Data());

            VGPlayer vgPlayer;
            User user;
            if (await CheckIfUserExists(player)){
                user = await LoadUser(player);
                vgPlayer = await LoadVGPlayer(player);
            }
            else{
                user = await InsertUser(player);
                vgPlayer = await InsertVGPlayer(player);
            }

            PlayerData[playerName].User = user;
            PlayerData[playerName].VGPlayer = vgPlayer;

            bool tasks = await CharacterCreatorService.CheckIfCharacterExist(player.Name);
            Character character = await CharacterCreatorService.GetCharacter(player.Name);
            if (tasks)
                EventDispatcher.Send(player, "player:spawn:to:world", character.Sex, vgPlayer.PosX, vgPlayer.PosY, vgPlayer.PosZ,
                                    110f);
            else
                EventDispatcher.Send(player, "player:spawn:to:creator");

            PlayerSlots.Add(player.Name, new PlayerSlot{
                Name = player.Name,
                Level = vgPlayer.Level,
                JobPointsText = "",
                GangName = ""
            });
            Trace.Log("afterLoad");
            EventDispatcher.Send(Main.Instance.PlayerList(),"afterLoad", player.Name);
            Debug.WriteLine($"Joining player {player.Name}({player.Handle}) to the server!");
        }

        public void Init()
        {
            Trace.Log("Initializing PlayerService");
        }
    }

    public class Data{
        public User User{ get; set; }
        public VGPlayer VGPlayer{ get; set; }
        public Inventory Inventory{ get; set; }
    }

    public class PlayerSlot{
        public string Name{ get; set; }

        public int Level{ get; set; }

        //private int Color { get; set; }
        public string JobPointsText{ get; set; }
        public string GangName{ get; set; }
    }


    internal class AutoSaver{
        public AutoSaver(){
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (s, e) => OnTimedEvent(); //new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 1000 * 60 * 5;
            timer.Enabled = true;
        }

        private static void OnTimedEvent(){
            int TotalSaved = 0;
            foreach (Player player in Main.Instance.PlayerList()){
                if (player == null)
                    continue;

                PlayerService.UpdateVGPlayer(player, player.Name);
                TotalSaved++;
            }

            Trace.Log($"[Server] Auto-save of players ({TotalSaved})");
        }
    }
}