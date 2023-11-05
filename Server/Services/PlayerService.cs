using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Dapper;
using Server.Database;
using Server.Database.Entities;

namespace Server.Services{
    class PlayerService : IService{
        public Dictionary<Player, VGPlayer> Players = new Dictionary<Player, VGPlayer>();

        public PlayerService(){
            //Autosaving of online players!
            new Thread(() => { new AutoSaver(); }).Start();
        }

        public void PlayerJoin([FromSource] Player player){
            Debug.WriteLine($"Joining player {player.Name}({player.Handle}) to the server!");
            if (CheckIfPlayerExists(player).Result){
                LoadPlayer(player, true);
            }
            else{
                InsertPlayer(player);
            }


            Players.TryGetValue(player, out VGPlayer vgp);
            //Debug.Write("Load Data of Player: " + vgp.ToString());
        }

        //[EventHandler("")]
        public void OnPlayerDropped([FromSource] Player player, string reason){
            Debug.WriteLine($"Player left {player.Name}");
            UpdatePlayer(player, true);
        }

        public void OnResourceStop(string resource){
            if (resource != "vojnagangov5")
                return;
            Debug.WriteLine($"Resource {resource} has stopped");
            PlayerList playerList = new PlayerList();
            foreach (Player player in playerList){
                Debug.WriteLine($"Saving of {player.Name}.");
                UpdatePlayer(player, true);
                Debug.WriteLine("Update player - save player data!");
            }
        }

        public async void LoadPlayer(Player player, bool reload = false){
            if (Players.ContainsKey(player) && reload){
                UpdatePlayer(player);
                player.Drop("Multiple account connected by one license.");
                return;
            }

            using (var connection = Connector.GetConnection()){
                await connection.OpenAsync();
                VGPlayer vgPlayer =
                    await connection.QueryFirstOrDefaultAsync<
                        VGPlayer>($"SELECT * FROM {VGPlayer.TABLE_NAME} WHERE Licence = @licence;",
                                  new{ licence = GetLicense(player) }, null);
                if (vgPlayer == null)
                    throw new NoNullAllowedException("User cant be found.");
                //UpdatePlayer(player);

                API.SetPlayerWantedLevel(player.Handle, vgPlayer.WantedLevel, false);
                player.TriggerEvent("player:load:data", vgPlayer.Money, vgPlayer.BankMoney, vgPlayer.PosX,
                                    vgPlayer.PosY, vgPlayer.PosZ, vgPlayer.Dimension, vgPlayer.Hp, vgPlayer.Max_hp,
                                    vgPlayer.Armour, vgPlayer.Max_armour);
                Debug.WriteLine("Load data and send to client!");
                Debug.WriteLine($"Data: {vgPlayer.ToString()}");

                Players.Add(player, vgPlayer);
                await connection.CloseAsync();
            }
        }

        public async void InsertPlayer(Player player){
            using (var connection = Connector.GetConnection()){
                await connection.OpenAsync();

                string insertQuery =
                    $@"INSERT INTO {VGPlayer.TABLE_NAME} (Name, Licence, WantedLevel, Money, BankMoney, Level, Xp, PosX, PosY, PosZ, Dimension)
											VALUES (@Name, @Licence, @WantedLevel, @Money, @BankMoney, @Level, @Xp, @PosX, @PosY, @PosZ, @Dimension)";
                VGPlayer vgPlayer =
                    new VGPlayer(player.Name, GetLicense(player), 100, 100, 0, 100, 0, 0, 0, 1, 0, 0, 0, 0, 0){
                        PosX = player.Character.Position.X,
                        PosY = player.Character.Position.Y,
                        PosZ = player.Character.Position.Z
                    };
                await connection.ExecuteAsync(insertQuery, vgPlayer);
                Players.Add(player, vgPlayer);
                await connection.CloseAsync();
            }
        }

        public async void UpdatePlayer(Player player, bool remove = false){
            using (var connection = Connector.GetConnection()){
                await connection.OpenAsync();
                string updateQuery = $@"UPDATE {VGPlayer.TABLE_NAME}
						   SET Name = @Name, Licence = @Licence, Hp = @Hp, Max_hp = @Max_hp, Armour = @Armour, Max_armour = @Max_armour, WantedLevel = @WantedLevel,
							   Money = @Money, BankMoney = @BankMoney, Level = @Level, Xp = @Xp,
							   PosX = @PosX, PosY = @PosY, PosZ = @PosZ, Dimension = @Dimension
						   WHERE Licence = @Licence";
                if (Players.TryGetValue(player, out VGPlayer vgPlayer)){
                    vgPlayer.PosX = player.Character.Position.X;
                    vgPlayer.PosY = player.Character.Position.Y;
                    vgPlayer.PosZ = player.Character.Position.Z;
                    vgPlayer.Hp = API.GetEntityHealth(player.Character.Handle);
                    vgPlayer.Max_hp = API.GetEntityMaxHealth(player.Character.Handle);
                    vgPlayer.Armour = API.GetPedArmour(player.Character.Handle);
                    vgPlayer.Max_armour = API.GetPlayerMaxArmour(player.Handle);
                    vgPlayer.Dimension = API.GetPlayerRoutingBucket(player.Handle);
                    await connection.ExecuteAsync(updateQuery, vgPlayer);
                }
                else
                    InsertPlayer(player);

                await connection.CloseAsync();
            }

            if (remove){
                if (Players.ContainsKey(player))
                    Players.Remove(player);
                Debug.WriteLine($"\nRemoving of {player.Name}.\n");
            }
        }

        public async Task<bool> CheckIfPlayerExists(Player player){
            using (var connection = Connector.GetConnection()){
                await connection.OpenAsync();
                string checkExistenceQuery = $@"SELECT COUNT(*) FROM {VGPlayer.TABLE_NAME} WHERE Licence = @Licence";
                int recordCount =
                    await connection.QueryFirstAsync<int>(checkExistenceQuery, new{ Licence = GetLicense(player) });
                if (recordCount > 0)
                    return true;
                await connection.CloseAsync();
            }

            return false;
        }

        public string GetLicense(Player player){
            return API.GetPlayerIdentifierByType(player.Handle, "license");
        }
    }


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

                ServiceManager.PlayerService.UpdatePlayer(player);
                Debug.WriteLine("[Server] Autosave of players [Every 5 minutes];");
            }
        }
    }
}