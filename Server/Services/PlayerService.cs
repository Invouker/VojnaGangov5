using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Dapper;
using MySqlConnector;
using Server.Database;
using Server.Database.Entities;

namespace Server.Services{
    class PlayerService : IService{
        public Dictionary<string, VGPlayer> Players = new Dictionary<string, VGPlayer>();

        public PlayerService(){
            //Autosaving of online players!
            new Thread(() => { new AutoSaver(); }).Start();
        }

        public void PlayerJoin([FromSource] Player player){
            Debug.WriteLine($"Joining player {player.Name}({player.Handle}) to the server!");
            Debug.WriteLine("Players counts: " + Players.Count);

            CheckPlayer(player);
        }

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

        //[EventHandler("")]
        public void OnPlayerDropped([FromSource] Player player, string reason){
            Debug.WriteLine($"Player left {player.Name}");

            var license = Utils.GetLicense(player);
            UpdatePlayer(player, license);

            Debug.WriteLine($"[Server] Removing of {player.Name}.");
            if (Players.ContainsKey(license)){
                bool success = Players.Remove(license);
                Debug.WriteLine($"State of removing player from Players {success}");
            }
        }

        public void OnResourceStop(string resource){
            if (resource != "vojnagangov5")
                return;
            Debug.WriteLine($"Resource {resource} has stopped");
            PlayerList playerList = new PlayerList();
            foreach (Player player in playerList){
                Debug.WriteLine($"Saving of {player.Name}.");
                UpdatePlayer(player, Utils.GetLicense(player));
            }
        }

        public async void LoadPlayer(Player player, string license){
            if (Players.ContainsKey(license)){
                //UpdatePlayer(player, Utils.GetLicense(player));
                player.Drop("Multiple account connected by one license.");
                return;
            }

            using (MySqlConnection connection = Connector.GetConnection()){
                await connection.OpenAsync();
                VGPlayer vgPlayer =
                    await connection.QueryFirstOrDefaultAsync<
                        VGPlayer>($"SELECT * FROM {VGPlayer.TABLE_NAME} WHERE Licence = @licence;",
                                  new{ licence = Utils.GetLicense(player) }, null);

                API.SetPlayerWantedLevel(player.Handle, vgPlayer.WantedLevel, false);
                player.TriggerEvent("player:load:data", vgPlayer.Money, vgPlayer.BankMoney, vgPlayer.PosX,
                                    vgPlayer.PosY, vgPlayer.PosZ, vgPlayer.Dimension, vgPlayer.Hp, vgPlayer.Max_hp,
                                    vgPlayer.Armour, vgPlayer.Max_armour);
                Debug.WriteLine($"Loaded data: {vgPlayer}");

                Players.Add(license, vgPlayer);
                await connection.CloseAsync();
            }
        }

        public async Task<VGPlayer> InsertPlayer(Player player){
            using (MySqlConnection connection = Connector.GetConnection()){
                await connection.OpenAsync();
                string insertQuery =
                    $@"INSERT INTO {VGPlayer.TABLE_NAME} (Name, Licence, WantedLevel, Money, BankMoney, Level, Xp, PosX, PosY, PosZ, Dimension)
											VALUES (@Name, @Licence, @WantedLevel, @Money, @BankMoney, @Level, @Xp, @PosX, @PosY, @PosZ, @Dimension)";
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

        public async void UpdatePlayer(Player player, string license){
            using (MySqlConnection connection = Connector.GetConnection()){
                connection.OpenAsync();
                string updateQuery = $@"UPDATE {VGPlayer.TABLE_NAME}
						   SET Name = @Name, Licence = @Licence, Hp = @Hp, Max_hp = @Max_hp, Armour = @Armour, Max_armour = @Max_armour, WantedLevel = @WantedLevel,
							   Money = @Money, BankMoney = @BankMoney, Level = @Level, Xp = @Xp,
							   PosX = @PosX, PosY = @PosY, PosZ = @PosZ, Dimension = @Dimension
						   WHERE Licence = @Licence";

                if (Players.TryGetValue(license, out VGPlayer vgPlayer)){
                    vgPlayer.PosX = player.Character.Position.X;
                    vgPlayer.PosY = player.Character.Position.Y;
                    vgPlayer.PosZ = player.Character.Position.Z;
                    vgPlayer.Hp = API.GetEntityHealth(player.Character.Handle);
                    vgPlayer.Max_hp = API.GetEntityMaxHealth(player.Character.Handle);
                    vgPlayer.Armour = API.GetPedArmour(player.Character.Handle);
                    vgPlayer.Max_armour = API.GetPlayerMaxArmour(player.Handle);
                    vgPlayer.Dimension = API.GetPlayerRoutingBucket(player.Handle);
                    int rowsAffected = await connection.ExecuteAsync(updateQuery, vgPlayer);
                    Debug.WriteLine($"Rows Affected in table {rowsAffected}");

                    Debug.WriteLine("PosX: " + vgPlayer.PosX);
                    Debug.WriteLine("PosY: " + vgPlayer.PosY);
                    Debug.WriteLine("PosZ: " + vgPlayer.PosZ);
                }

                connection.CloseAsync();
            }
        }

        public async Task<bool> CheckIfPlayerExists(Player player){
            using (MySqlConnection connection = Connector.GetConnection()){
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
    /*
        internal class PlayerClone {
            public String Name{ get; set; }
            public String Licence { get; set; }
            public int Hp { get; set; }
            public int MaxHp { get; set; }
            public int Armour { get; set; }
            public int MaxArmour { get; set; }
            public int WantedLevel { get; set; }
            public long Money { get; set; }
            public long BankMoney { get; set; }
            public int Level { get; set; }
            public int Xp { get; set; }
            public float PosX { get; set; }
            public float PosY { get; set; }
            public float PosZ { get; set; }
            public int Dimension { get; set; }
           // public Player Player{ get; set; }

            public PlayerClone(Player player){
                Name = player.Name;
                Licence = Utils.GetLicense(player);
                Hp = API.GetEntityHealth(player.Character.Handle);
                MaxHp = API.GetEntityMaxHealth(player.Character.Handle);
                Armour = API.GetPedArmour(player.Character.Handle);
                MaxArmour =  API.GetPlayerMaxArmour(player.Handle);
                WantedLevel = API.GetPlayerWantedLevel(player.Handle);
                //money
                //bankMoney
                //level
                //xp
                PosX = player.Character.Position.X;
                PosY = player.Character.Position.Y;
                PosZ = player.Character.Position.Z;
                Dimension = API.GetPlayerRoutingBucket(player.Handle);
                //Player = player;
            }
        }*/

    class AutoSaver{
        public AutoSaver(){
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Elapsed += (s, e) => OnTimedEvent(); //new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 1000 * 60 * 5;
            timer.Enabled = true;
        }

        // Specify what you want to happen when the Elapsed event is raised.
        private void OnTimedEvent(){
            PlayerList players = new PlayerList();
            foreach (Player player in players){
                if (player == null)
                    continue;

                ServiceManager.PlayerService.UpdatePlayer(player, Utils.GetLicense(player));
            }

            Debug.WriteLine("[Server] Autosave of players [Every 5 minutes];");
        }
    }
}