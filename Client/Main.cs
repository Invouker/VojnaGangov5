using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client {
    public class Main : BaseScript {

        IDictionary<int, int> playerBlips = new Dictionary<int, int>();
        Boolean isRaderExtended = false;

        public Main() {
			Tick += OnTick;
			Tick += PlayerBlipTick;
            Tick += PlayerMinimapTick;
        }

		private async Task PlayerMinimapTick() {
            if(API.IsControlJustPressed(0, 20)){
                isRaderExtended = true;
                API.SetRadarBigmapEnabled(true, false);
                await Delay(3500);
                API.SetRadarBigmapEnabled(false, false);
                isRaderExtended = false;
            }

            API.SetRadarBigmapEnabled(false, false);
        }

		private async Task PlayerBlipTick() {
            
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

            //Function.Call(Hash.SHOW_HEADING_INDICATOR_ON_BLIP, 2, false);
            //API.AddBlipForEntity()
            //SetBlipAsFriendly(Blip blip, bool toggle);
        
        
        }


		private async Task OnTick(){

            Game.PlayerPed.Weapons.RemoveAll();
		    foreach(WeaponHash weapon in Enum.GetValues(typeof(WeaponHash))) {
                if (!Game.PlayerPed.Weapons.HasWeapon(weapon))
					Game.PlayerPed.Weapons.Give(weapon, 200, false, true);
            }
            SendMessage("test");
            await Delay(5000);
        }

		private void SendMessage(string text){
            TriggerEvent("chat:addMessage", new {
                color = new[] { 100, 100, 100 },
                multiline = false,
                args = new[] { text }
            }); ;
		}
	}
}
