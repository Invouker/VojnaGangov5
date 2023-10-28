using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;

namespace Client.Streamable {

	class Streamer {

		private static List<Marker> markers = new List<Marker>();

		private readonly int renderRadius;

		public Streamer(int renderRadius = 10) {
			this.renderRadius = renderRadius;
		}

		public void addMarker(int x, int y, int z, int type = 1) {
			Marker markable = new Marker(type, x, y, z);
			markers.Add(markable);
		}

		private async void render() {
			foreach (Marker markable in markers) {
				if (getPlayerPosition().DistanceToSquared(markable.getMarkerPosition()) < renderRadius) {
					markable.drawMarker();
				}
			}

		}

		private Vector3 getPlayerPosition() {
			return new Vector3(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z);
		}

		public static void renderWorldText() {
			new WorldText();
		}

	}

	internal class WorldText {
		public WorldText() {
	
			Vector2 screenPos = World3DToScreen2d(new Vector3(-1380.451f, 737.6341f, 183.0269f));

			int fov = fov = (int)((1 / API.GetGameplayCamFov()) * 75); ;
			API.SetTextScale(0.0f, 0.6f);
			API.SetTextFont(1); // Font.HouseScript
			API.SetTextProportional(true);
			API.SetTextColour(255, 255, 255, 255);
			API.SetTextEntry("STRING");
			API.SetTextCentre(true);
			API.AddTextComponentString("TEST");
			API.DrawText(screenPos.X, screenPos.Y);
		}

		private Vector2 World3DToScreen2d(Vector3 pos) {
			var x2dp = new OutputArgument();
			var y2dp = new OutputArgument();
			Function.Call<bool>(Hash.GET_SCREEN_COORD_FROM_WORLD_COORD, pos.X, pos.Y, pos.Z, x2dp, y2dp);
			return new Vector2(x2dp.GetResult<float>(), y2dp.GetResult<float>());
		}
	}

	internal class Marker {

		private readonly int alpha = 155;
		private float posX;
		private float posY;
		private float posZ;

		private readonly int red, green, blue;

		private int markerType;

		public Marker(int type, float x, float y, float z, int red = 255, int green = 255, int blue = 255) {
			this.markerType = type;

			this.posX = x;
			this.posY = y;
			this.posZ = z;

			this.red = red;
			this.green = green;
			this.blue = blue;
		}

		public void drawMarker() {
			API.DrawMarker(markerType, posX, posY, posZ, 1,1,1, 0,0,0,1,1,1, red, green, blue, alpha, false, false, 2, true, "", "", false);
		}

		public Vector3 getMarkerPosition() {
			return new Vector3(this.posX, this.posY, this.posZ);
		}

	}
}
