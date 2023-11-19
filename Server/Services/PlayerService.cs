using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Dapper;
using MySqlConnector;
using Server.Database;
using Server.Database.Entities;

namespace Server.Services{
    internal class PlayerService : IService{
        private static readonly Dictionary<string, VGPlayer> Players = new();

        public PlayerService(){
            Main.Instance.AddEventHandler("player:join", new Action<Player>(PlayerJoin));
            Main.Instance.AddEventHandler("playerDropped", new Action<Player, string>(OnPlayerDropped));

            new Thread(() => { new AutoSaver(); }).Start();
        }

        public void PlayerJoin([FromSource] Player player){
            Debug.WriteLine($"Joining player {player.Name}({player.Handle}) to the server!");
            CheckPlayer(player);
        }

        public static VGPlayer GetVgPlayerByPlayer(Player player){
            return GetVgPlayerByName(player.Name);
        }

        public static VGPlayer GetVgPlayerByName(string Name){
            return (from Player in Players where Player.Value.Name.Equals(Name) select Player.Value).FirstOrDefault();
        }

        public static VGPlayer GetVgPlayerByLicense(string license){
            if (Players.TryGetValue(license, out VGPlayer vgPlayer)){
                return vgPlayer;
            }

            Debug.WriteLine($"Player with license '{license}' not found.");
            return null; // Return null if the player with the given name is not found.
        }


        #region MoneyMethods

        public enum MoneyType{
            Wallet = 0,
            Bank = 1
        }

        public static void SetMoney(Player player, MoneyType moneyType, uint value){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            switch (moneyType){
                case MoneyType.Bank:
                    vgPlayer.BankMoney = value;
                    break;
                case MoneyType.Wallet:
                    vgPlayer.Money = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(moneyType), moneyType,
                                                          "There is no other registred MoneyType than (Wallet,Bank).");
            }

            BaseScript.TriggerClientEvent(player, "player:hud:update:money", (int)moneyType, value);
        }

        public static void AddMoney(Player player, MoneyType moneyType, uint value){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            switch (moneyType){
                case MoneyType.Bank:
                    vgPlayer.BankMoney += value;
                    break;
                case MoneyType.Wallet:
                    vgPlayer.Money += value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(moneyType), moneyType,
                                                          "There is no other registred MoneyType than (Wallet,Bank).");
            }

            BaseScript.TriggerClientEvent(player, "player:hud:update:money", (int)moneyType, value);
        }

        public static void TakeMoney(Player player, MoneyType moneyType, uint value){
            VGPlayer vgPlayer = GetVgPlayerByPlayer(player);
            switch (moneyType){
                case MoneyType.Bank:
                    vgPlayer.BankMoney -= value;
                    break;
                case MoneyType.Wallet:
                    vgPlayer.Money -= value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(moneyType), moneyType, null);
            }

            BaseScript.TriggerClientEvent(player, "player:hud:update:money", (int)moneyType, value);
        }

        #endregion

        private async void CheckPlayer(Player player){
            var license = Utils.GetLicense(player);
            if (CheckIfPlayerExists(player).Result){
                LoadPlayer(player, Utils.GetLicense(player));
            }
            else{
                var vgPlayer = InsertPlayer(player);
                while (!vgPlayer.IsCompleted)
                    await BaseScript.Delay(0);

                Players.Add(license, vgPlayer.Result);
            }
        }

        public void OnPlayerDropped([FromSource] Player player, string reason){
            Debug.WriteLine($"Player left {player.Name}");

            var license = Utils.GetLicense(player);
            UpdatePlayer(player, license);

            if (!Players.ContainsKey(license)) return;
            Players.Remove(license);
        }

        public void OnResourceStop(string resource){ //Todo: Uncomment this if playerlist and autosave will be ok
            if (resource != "vojnagangov5")
                return;
            Debug.WriteLine($"Resource {resource} has stopped");
            PlayerList playerList = Main.Instance.PlayerList();
            foreach (Player player in playerList){
                Debug.WriteLine($"Saving of {player.Name}.");
                UpdatePlayer(player, Utils.GetLicense(player));
            }
        }

        private async void LoadPlayer(Player player, string license){
            if (Players.ContainsKey(license)){
                //UpdatePlayer(player, Utils.GetLicense(player));
                player.Drop("Multiple account connected by one license.");
                return;
            }

            await using (MySqlConnection connection = Connector.GetConnection()){
                await connection.OpenAsync();
                const string LoadPlayer = $"SELECT * FROM {VGPlayer.TABLE_NAME} WHERE Licence = @licence;";
                VGPlayer vgPlayer =
                    await connection.QueryFirstOrDefaultAsync<VGPlayer>(LoadPlayer,
                                                                        new{ licence = Utils.GetLicense(player) },
                                                                        null);

                API.SetPlayerWantedLevel(player.Handle, vgPlayer.WantedLevel, false);

                player.TriggerEvent("player:load:data", vgPlayer.Money, vgPlayer.BankMoney, vgPlayer.PosX,
                                    vgPlayer.PosY, vgPlayer.PosZ, vgPlayer.Dimension, vgPlayer.Hp, vgPlayer.Max_hp,
                                    vgPlayer.Armour, vgPlayer.Max_armour);
                //   (long money, long bankMoney, float x, float y, float z, int dimension, int hp, int maxHp, int armour, int maxArmour){
                //   new Action<long, long, float, float, float, int, int, int, int, int>(LoadPlayerData);
                Debug.WriteLine($"Loaded data: {vgPlayer}");

                Players.Add(license, vgPlayer);
                await connection.CloseAsync();
            }
        }

        private async Task<VGPlayer> InsertPlayer(Player player){
            await using (MySqlConnection connection = Connector.GetConnection()){
                await connection.OpenAsync();
                const string insertQuery =
                    $"""
                     INSERT INTO {VGPlayer.TABLE_NAME} (Name, Licence, WantedLevel, Money, BankMoney, Level, Xp, PosX, PosY, PosZ, Dimension)
                     											VALUES (@Name, @Licence, @WantedLevel, @Money, @BankMoney, @Level, @Xp, @PosX, @PosY, @PosZ, @Dimension)
                     """;
                VGPlayer vgPlayer =
                    new VGPlayer(player.Name, Utils.GetLicense(player), 100, 100, 0, 100, 0, 0, 0, 1, 0, 0, 0, 0, 0){
                        PosX = player.Character.Position.X,
                        PosY = player.Character.Position.Y,
                        PosZ = player.Character.Position.Z
                    };

                await connection.ExecuteAsync(insertQuery, vgPlayer);
                await connection.CloseAsync();
                return vgPlayer;
            }
        }

        public static async void UpdatePlayer(Player player, string license){
            await using (MySqlConnection connection = Connector.GetConnection()){
                await connection.OpenAsync();
                const string UpdateQuery = $"""
                                            UPDATE {VGPlayer.TABLE_NAME}
                                            						   SET Name = @Name, Licence = @Licence, Hp = @Hp, Max_hp = @Max_hp, Armour = @Armour, Max_armour = @Max_armour, WantedLevel = @WantedLevel,
                                            							   Money = @Money, BankMoney = @BankMoney, Level = @Level, Xp = @Xp,
                                            							   PosX = @PosX, PosY = @PosY, PosZ = @PosZ, Dimension = @Dimension
                                            						   WHERE Licence = @Licence
                                            """;

                if (Players.TryGetValue(license, out VGPlayer vgPlayer)){
                    vgPlayer.PosX = player.Character.Position.X;
                    vgPlayer.PosY = player.Character.Position.Y;
                    vgPlayer.PosZ = player.Character.Position.Z;
                    vgPlayer.Hp = API.GetEntityHealth(player.Character.Handle);
                    vgPlayer.Max_hp = API.GetEntityMaxHealth(player.Character.Handle);
                    vgPlayer.Armour = API.GetPedArmour(player.Character.Handle);
                    vgPlayer.Max_armour = API.GetPlayerMaxArmour(player.Handle);
                    vgPlayer.Dimension = API.GetPlayerRoutingBucket(player.Handle);
                    await connection.ExecuteAsync(UpdateQuery, vgPlayer);
                }

                await connection.CloseAsync();
            }
        }

        private static async Task<bool> CheckIfPlayerExists(Player player){
            await using (MySqlConnection connection = Connector.GetConnection()){
                await connection.OpenAsync();
                const string CheckExistenceQuery =
                    $"SELECT COUNT(*) FROM {VGPlayer.TABLE_NAME} WHERE Licence = @Licence";
                int recordCount =
                    await connection.QueryFirstAsync<int>(CheckExistenceQuery,
                                                          new{ Licence = Utils.GetLicense(player) });
                if (recordCount > 0)
                    return true;
                await connection.CloseAsync();
            }

            return false;
        }

        private async Task<bool> CheckIfPlayerCharacterExists(Player player){
            await using (MySqlConnection connection = Connector.GetConnection()){
                await connection.OpenAsync();
                string checkExistenceQuery = $@"SELECT COUNT(*) FROM {VGPlayer.TABLE_NAME} WHERE Licence = @Licence";
                int recordCount =
                    await connection.QueryFirstAsync<int>(checkExistenceQuery,
                                                          new{ Licence = Utils.GetLicense(player) });
                if (recordCount > 0)
                    return true;
                await connection.CloseAsync();
            }

            return false;
        }
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

                PlayerService.UpdatePlayer(player, Utils.GetLicense(player));
                TotalSaved++;
            }

            Debug.WriteLine($"[Server] Auto-save of players ({TotalSaved})");
        }
    }
}