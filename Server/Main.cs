using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using static CitizenFX.Core.Native.API;

namespace Server {
	public class Main : BaseScript {

		public Main() {
			RegisterCommands();
		}

		private void RegisterCommands() {
			RegisterCommand("mylic", new Action<int, List<object>, string>((source, args, raw) => {
			/*Debug.WriteLine("Source: " + source);
				var id = API.GetPlayerFromIndex(source); ;
			Debug.WriteLine("ID: " + id);*/
				var lic = API.GetPlayerIdentifierByType(source + "", "licence");
			Debug.WriteLine("lic: " + lic);
				SendMessage("Player Licence: " + lic);
			}), false);

			RegisterCommand("test", new Action<int, List<object>, string>((source, args, raw) => {
			//var player = API.GetPlayerPed(source);//API.GetPlayerFromIndex(source);
			//player.TriggerEvent("event:test", source, args, raw);
			TriggerClientEvent("event:test", source, args, raw);

			}), false);
		
		RegisterCommand("teleport", new Action<int, List<object>, string>((source, args, raw) => {
		//var player = API.GetPlayerPed(source);//API.GetPlayerFromIndex(source);
		//player.TriggerEvent("event:test", source, args, raw);
		//TriggerClientEvent("event:test", source, args, raw);
			vehicle(source);
			SendMessage("Bol si teleportovaný!");
			}), false);

		}

		private async void vehicle(int source) {
			SetEntityCoords(source, -1361.641f, 735.1932f, 183.9867f, true, false, false, false);
			int vehicle = CreateVehicle((uint)GetHashKey("zentorno"), -1361.641f, 739.1932f, 183.9867f, 10, true, false);
		await Delay(10);
			TaskEnterVehicle(source, vehicle, 20000, -1, 1.5f, 1, 0);
		}

		private void SendMessage(string text) {
		TriggerEvent("chat:addMessage", new {
			color = new[] { 250, 250, 250 },
			multiline = false,
			args = new[] { text }
		}); ;
		}


	}
}

