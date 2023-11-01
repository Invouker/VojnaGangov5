using CitizenFX.Core;
using CitizenFX.Core.Native;
using Dapper;
using Server.Database;
using Server.Database.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Services {
	class PlayerService {

        public Dictionary<Player, VGPlayer> Players = new Dictionary<Player, VGPlayer>();

        public async void PlayerJoin([FromSource] Player player) {
			Debug.Write($"Joining player {player.Name}({player.Handle}) to the server!");
			await LoadPlayer(player);
        
			Players.TryGetValue(player, out VGPlayer vgp);
			//Debug.Write("Load Data of Player: " + vgp.ToString());
        }

        public void OnPlayerDropped([FromSource] Player player, string reason) {
			Debug.Write($"Player left {player.Name}");
			UpdatePlayer(player);
        }

        public void OnResourceStop(string resource) {
			if (resource != "vojnagangov5")
				return;
			Debug.Write($"Resource {resource} has stopped");
			PlayerList playerList = new PlayerList();
			foreach (Player player in playerList) {
				Debug.Write($"Saving of {player.Name}.");
				UpdatePlayer(player, true);
				Debug.Write("Update player - save player data!");
			}
        }

		public void OnResourceStarting(string resource) {
			/*Debug.WriteLine($"Resource {resource} has started");
			PlayerList playerList = new PlayerList();
			foreach (Player player in playerList) {
			//var success = await LoadPlayer(player);
			}*/
		}

		

		public async void LoadPlayer(Player player) {
			if (Players.ContainsKey(player)) {
				UpdatePlayer(player);
				player.Drop("Multiple account connected by one");
				return;
			}
			
			using (var connection = Connector.GetConnection()) {
				await connection.OpenAsync();

				VGPlayer vgPlayer = await connection.QueryFirstOrDefaultAsync<VGPlayer>($"SELECT * FROM {VGPlayer.TABLE_NAME} WHERE Licence = @licence;", new { licence = GetLicense(player)}, null);

				if (vgPlayer == null) 
					UpdatePlayer(player);
				else {
					API.SetPlayerWantedLevel(player.Handle, vgPlayer.WantedLevel, false);
					player.TriggerEvent("player:load:data", vgPlayer.Money, vgPlayer.BankMoney, vgPlayer.PosX, vgPlayer.PosY, vgPlayer.PosZ, vgPlayer.Dimension);
				}

				Players.Add(player, vgPlayer);
				await connection.CloseAsync();
			}
        }
      
        public async void InsertPlayer(Player player) {
			using (var connection = Connector.GetConnection()) {
				await connection.OpenAsync();

				string insertQuery = $@"INSERT INTO {VGPlayer.TABLE_NAME} (Name, Licence, WantedLevel, Money, BankMoney, Level, Xp, PosX, PosY, PosZ, Dimension)
											VALUES (@Name, @Licence, @WantedLevel, @Money, @BankMoney, @Level, @Xp, @PosX, @PosY, @PosZ, @Dimension)";
			VGPlayer vgPlayer = new VGPlayer(player.Name, GetLicense(player), 0, 0, 0, 1, 0, 0, 0, 0, 0)
			{
				PosX = player.Character.Position.X,
				PosY = player.Character.Position.Y,
				PosZ = player.Character.Position.Z
			};
			await connection.ExecuteAsync(insertQuery, vgPlayer);
				Players.Add(player, vgPlayer);
				await connection.CloseAsync();
			}
        }

        public async void UpdatePlayer(Player player, bool remove = false) {
			using (var connection = Connector.GetConnection()) {
			await connection.OpenAsync();
			string updateQuery = $@"UPDATE {VGPlayer.TABLE_NAME}
						   SET Name = @Name, Licence = @Licence, WantedLevel = @WantedLevel,
							   Money = @Money, BankMoney = @BankMoney, Level = @Level, Xp = @Xp,
							   PosX = @PosX, PosY = @PosY, PosZ = @PosZ, Dimension = @Dimension
						   WHERE Licence = @Licence";
			if (Players.ContainsKey(player)) {
				Players.TryGetValue(player, out VGPlayer vgPlayer);
				vgPlayer.PosX = player.Character.Position.X;
				vgPlayer.PosY = player.Character.Position.Y;
				vgPlayer.PosZ = player.Character.Position.Z;
				vgPlayer.Dimension = API.GetPlayerRoutingBucket(player.Handle);
				//Debug.WriteLine("UpdatePlayer: " + vgPlayer.ToString());
				await connection.ExecuteAsync(updateQuery, vgPlayer);
			} else
				InsertPlayer(player);

			await connection.CloseAsync();
			}

			if(remove) {
				Players.Remove(player);
				Debug.WriteLine($"\nRemoving of {player.Name}.\n");
			}
        }

        public async Task<bool> CheckIfPlayerExists(Player player) {
			using (var connection = Connector.GetConnection()) {
				await connection.OpenAsync();
				string checkExistenceQuery = $@"SELECT COUNT(*) FROM {VGPlayer.TABLE_NAME} WHERE Licence = @Licence";
				int recordCount = await connection.QueryFirstAsync<int>(checkExistenceQuery, new { Licence = GetLicense(player) });
				if (recordCount > 0)
					return true;
				await connection.CloseAsync();
				}
			return false;
        }

        public string GetLicense(Player player) {
			return API.GetPlayerIdentifierByType(player.Handle, "license");
        }


    }
}
