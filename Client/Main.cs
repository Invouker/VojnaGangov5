using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using System;

namespace Client {
	public class Main : BaseScript {

        public Main() {
            Screen.ShowNotification("Notification??");
			Screen.Effects.Start(ScreenEffect.MpCoronaSwitch);
            TriggerServerEvent("player:join");
			EventHandlers["player:load:data"] += new Action<long, long, float, float, float, int>(LoadPlayerData);
		}

		private void LoadPlayerData(long money, long bankMoney, float x, float y, float z, int dimension) {
			API.StartPlayerTeleport(API.GetPlayerIndex(), x, y,z, 0,false, false, true);
			Debug.WriteLine("Load Player Data!");
		}
	}
}
