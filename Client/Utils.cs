using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client {
	public class Utils {

		/*public static void SendMessage(string text) {
			API.TriggerEvent("chat:addMessage", new {
				color = new[] { 100, 100, 100 },
				multiline = false,
				args = new[] { text }
			}); ;
		}*/

		public static Vector3 getPlayerPosition() {
			return new Vector3(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z);
		}

		public static Vector2 World3DToScreen2d(Vector3 pos) {
			var x2dp = new OutputArgument();
			var y2dp = new OutputArgument();
			Function.Call<bool>(Hash.GET_SCREEN_COORD_FROM_WORLD_COORD, pos.X, pos.Y, pos.Z, x2dp, y2dp);
			return new Vector2(x2dp.GetResult<float>(), y2dp.GetResult<float>());
		}


	}
}
