using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Streamable;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client {

	/* Documentation:
	 * API.ChangeFakeMpCash(10, 20); - for changing a cash in bank or wallet
	 */
	public class Main : BaseScript {

		IDictionary<int, int> playerBlips = new Dictionary<int, int>();
		Boolean isRaderExtended = false;

		public Main() {
			//Tick += OnTick;
			Tick += PlayerBlipTick;
			Tick += PlayerMinimapTick;
			
			EventHandlers["event:test"] += new Action<int, List<object>, string>(OnClientTest);

			
			Streamer.createMarker(-1381.218f, 737.7022f, 183.0317f);
			Streamer.create3dText("~bold~Testujeme", -1381.218f, 737.7022f, 183.0317f);
			Streamer.createBlip("~bold~Testujeme", -1381.218f, 737.7022f, 183.0317f, 255, 1);


			Streamer.createMarker(-1393.996f, 742.8211f, 182.9561f);
			Streamer.create3dText("~r~Testujeme", -1393.996f, 742.8211f, 182.9561f);

			new Streamer();
			Tick += Renderer;

		}

		private Task Renderer() {
			Streamer.render();
			return Task.FromResult(0);
		}

		private void OnClientTest(int src, List<object> args, string raw) {

		if (args != null && args.Count >= 1) {

		if (args[0].Equals("bank"))
			API.ShowHudComponentThisFrame(3);
		if (args[0].Equals("cash"))
			API.ShowHudComponentThisFrame(4);
		if (args[0].Equals("nui")) {
			API.BeginTextCommandThefeedPost("String");
			API.AddTextComponentSubstringPlayerName("PlayerName");
			API.ThefeedNextPostBackgroundColor(1);
			//API.EndTackCommandTheFeedPostMessagetext("CHAR_AMMUNATION");
			API.EndTextCommandThefeedPostTicker(false, true);
		}
		if (args[0].Equals("mypos")) {
			SendMessage("X: " + Player.Local.Character.Position.X);
			SendMessage("Y: " + Player.Local.Character.Position.Y);
			SendMessage("Z: " + Player.Local.Character.Position.Z);
		}
		if (args[0].Equals("nui2")) {
			API.SendNuiMessage(JsonConvert.SerializeObject(new {
				type = "open"
			}));
		}

	
			} else SendMessage("Only allowed args are: <bank, cash, nui, mypos, nui2>");
		}

		private async Task PlayerMinimapTick() {
			if (API.IsControlJustPressed(0, 20) && !isRaderExtended) {
				isRaderExtended = true;
				API.SetRadarBigmapEnabled(true, false);
			{ // Show Bank and Wallet state
				API.ShowHudComponentThisFrame(3); // Bank state
				API.ShowHudComponentThisFrame(4); // Wallet state
			}
			await Delay(5000);
				API.SetRadarBigmapEnabled(false, false);
				isRaderExtended = false;
			}

		//API.SetRadarBigmapEnabled(false, false);
		}

		private Task PlayerBlipTick() {

			foreach (var player in API.GetActivePlayers()) {
				var playerPed = API.GetPlayerPed(player);
				if (API.NetworkIsPlayerActive(player) && API.GetPlayerPed(player) != API.GetPlayerPed(-1)) {
					var blip = API.GetBlipFromEntity(playerPed);
					if (!API.DoesBlipExist(blip)) {
						blip = API.AddBlipForEntity(playerPed);
						API.SetBlipSprite(blip, 1);
						Function.Call(Hash.SHOW_HEADING_INDICATOR_ON_BLIP, blip, true);
					} else {
						API.SetBlipNameToPlayerName(blip, player);
						API.SetBlipScale(blip, 0.85f);
						//API.SetBlipCoords(blip, playerPed.x, playerPed.y, playerPed.c);
					}
				}
			
		}
		return Task.FromResult(0);
	
		}
		//[Tick]
		public void testTick() {
		}

		private async Task OnTick() {
		//Game.PlayerPed.Weapons.RemoveAll();
			foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash))) {
				if (!Game.PlayerPed.Weapons.HasWeapon(weapon))
					Game.PlayerPed.Weapons.Give(weapon, 20000, false, true);

			}
			await Delay(5000);


		API.DrawRect(0.5f, 0.5f, 5f, 5f, 255, 255, 255, 150);
		}

		public static void SendMessage(string text) {
			TriggerEvent("chat:addMessage", new {
				color = new[] { 100, 100, 100 },
				multiline = false,
				args = new[] { text }
			}); ;
		}
	}
}
