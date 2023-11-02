using CitizenFX.Core;
using CitizenFX.Core.Native;
using Server.Database.Entities;
using Server.Services;
using System;
using System.Collections.Generic;

namespace Server {
	public class Main : BaseScript {
		public Main() {
			new ServiceManager();
			EventHandlers["player:join"] += new Action<Player>(ServiceManager.playerService.PlayerJoin);
			EventHandlers["playerDropped"] += new Action<Player, string>(ServiceManager.playerService.OnPlayerDropped);
			EventHandlers["onResourceStart"] += new Action<string>(ServiceManager.playerService.OnResourceStarting);
			EventHandlers["onResourceStop"] += new Action<string>(ServiceManager.playerService.OnResourceStop);
			registerCommands();
		}


		private void registerCommands() {
			API.RegisterCommand("get", new Action<int, List<object>, string>(( source, args, rawCommand) => {
				Player player = Players[source];
				ServiceManager.playerService.Players.TryGetValue(player, out VGPlayer vgPlayer);
				
				 player.TriggerEvent("chat:addMessage", new {
					 color = new[] { 16, 43, 76 },
					 args = new[] { "[Server]", $"Player get: {vgPlayer.ToString()}" }
				 });
			}), false );

			API.RegisterCommand("save", new Action<int, List<object>, string>((source, args, rawCommand) => {
				Player player = Players[source];
				ServiceManager.playerService.UpdatePlayer(player);
				player.TriggerEvent("chat:addMessage", new {
					color = new[] { 16, 43, 76 },
					args = new[] { "[Server]", $"Player saved" }
				});
			}), false);
			
			API.RegisterCommand("load", new Action<int, List<object>, string>((source, args, rawCommand) => {
				Player player = Players[source];
				ServiceManager.playerService.LoadPlayer(player);
				player.TriggerEvent("chat:addMessage", new {
					color = new[] { 16, 43, 76 },
					args = new[] { "[Server]", $"Player saved" }
				});
			}), false);
		}

		public static void sendMessage(string message) {
			TriggerEvent("chat:addMessage", new {
				color = new[] { 16, 43, 76 },
				args = new[] { "[Server]", message }
			});
		}
	}
}
