using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;

namespace Client.Streamable {

	class Streamer {

		private readonly static List<Marker> markers = new List<Marker>();
		private readonly static List<WorldText> worldTexts = new List<WorldText>();
		private readonly static List<MapBlip> blips = new List<MapBlip>();

		public Streamer() {
			if(blips.Count <= 0) 
				Console.Write("Cannot load blips! Because blips list is empty!");
			
			foreach (MapBlip blip in blips) {
				blip.drawBlip();
				Debug.Write("Drawing blip");
			}
		}
		public static void createMarker(float x, float y, float z, int type = 1) {
			Marker markable = new Marker(type, x, y, z);
			markers.Add(markable);
		}

		public static void create3dText(string text, float x, float y, float z, int red = 255, int green = 255, int blue = 255, int fontType = 0) {
			WorldText worldText = new WorldText(text, x, y,z, red, green, blue, fontType);
			worldTexts.Add(worldText);
		}
		public static void createBlip(string blipName, float posX, float posY, float posZ, int colour = 0, int blipSprite = 84, int blipDisplay = 2, float blipScale = 1f, bool showAsShortRange = false) {
			MapBlip mapBlip = new MapBlip(blipName, posX, posY, posZ, colour, blipSprite, blipDisplay, blipScale, showAsShortRange);
			blips.Add(mapBlip);
		}

		public static void render() {
			foreach (Marker markable in markers) {
				markable.renderMarker();
			}

			foreach (WorldText worldText in worldTexts) {
				worldText.render3DText();
			}
		}
	}

	internal class WorldText {
		private readonly int renderDistance = 45;

		private string text { get; set; }
		private float x, y, z;
		private int red, green, blue;
		private int fontType { get; set; }
		

		public WorldText(string text, float x, float y, float z, int red=255, int green=255, int blue=255, int fontType=0) {
			this.text = text;
			this.x = x;
			this.y = y;
			this.z = z;
			this.red = red;
			this.green = green;
			this.blue = blue;
			this.fontType = fontType;
		}

		public void render3DText() { // 
			Vector3 textPosition = new Vector3(x, y ,z);
			Vector2 screenPos = Utils.World3DToScreen2d(textPosition);
			Vector3 camPos = API.GetGameplayCamCoords();
			float distance = API.GetDistanceBetweenCoords(camPos.X, camPos.Y, camPos.Z, textPosition.X, textPosition.Y, textPosition.Z, true);
			if (renderDistance < distance) 
				return;

			API.SetTextScale(0.0f, 0.28f);
			API.SetTextFont(fontType); // Font.HouseScript
			API.SetTextProportional(true);
			API.SetTextColour(red, green, blue, 255);
			API.SetTextEdge(2, 0, 0, 0, 150);
			API.SetTextEntry("STRING");
			API.SetTextCentre(true);
			API.SetTextEdge(1, 0, 0, 0, 255);
			API.SetTextOutline();
			API.AddTextComponentString(text); // ~bold~[Gang HQ]\nMajitel: Invouk\nRešpekt: 0\nMajetok: 25,655,657$\n~w~Press ~INPUT_TALK~ to get info.
			API.DrawText(screenPos.X, screenPos.Y);
			API.ClearDrawOrigin();
		}
		/*
		 UNUSED CODE:
		Vector3 playerPosition = Utils.getPlayerPosition();
		API.GetDistanceBetweenCoords(textPosition.X, textPosition.Y, textPosition.Z, playerPosition.X, playerPosition.Y, playerPosition.Z, true)

					//float fov = (1 / API.GetGameplayCamFov()) * 75;
			//float scale = (1 / distance) * 4 * fov * 1;
		
			int shapeTest = API.StartShapeTestLosProbe(textPosition.X, textPosition.Y, textPosition.Z, playerPosition.X, playerPosition.Y, playerPosition.Z, -1, 0, 4);
							//GetShapeTestResult(StartShapeTestLosProbe(startcoord.x, startcoord.y, startcoord.z, endcoord.x, endcoord.y, endcoord.z, -1, yourcar, 4)
			bool collide = false;
			Vector3 endCoords = new Vector3(0, 0, 0);
			Vector3 surfaceNormal = new Vector3(0, 0, 0);
			int entityHit = 0;
			int result = API.GetShapeTestResult(shapeTest, ref collide, ref endCoords, ref surfaceNormal, ref entityHit);
		 */
	}

	internal class Marker {

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

		public void renderMarker() {
			API.DrawMarker(markerType, posX, posY, posZ - 1.5f, 0,0,0, 0,0,0,1.75f,1.75f,1.75f, red, green, blue, 255, false, true, 2, true, null, null, false);
			  //DrawMarker(0, 117.14,  -19,  20,   7513, 0,0,0, 0,0,0, 0.75, 0.75, 0.75, 204, 204, 0,       100, false, true, 2, false, false, false, false)
		}

		public Vector3 getMarkerPosition() {
			return new Vector3(this.posX, this.posY, this.posZ);
		}

	}
	internal class MapBlip {

		private float posX, posY, posZ;
		private int blipSprite = 84; // 1 change it
		private int blipDisplay = 2;
		private float blipScale = 1f;
		private int colour;
		private bool showAsShortRange = false;

		private String blipName;

		public MapBlip(string blipName, float posX, float posY, float posZ, int colour = 0, int blipSprite = 84, int blipDisplay = 2, float blipScale = 1f,  bool showAsShortRange = false) {
			this.posX = posX;
			this.posY = posY;
			this.posZ = posZ;
			this.blipSprite = blipSprite;
			this.blipDisplay = blipDisplay;
			this.blipScale = blipScale;
			this.colour = colour;
			this.showAsShortRange = showAsShortRange;
			this.blipName = blipName;
		}

		public void drawBlip() {
			int blip = API.AddBlipForCoord(posX, posY, posZ);
			API.SetBlipSprite(blip, blipSprite);
			API.SetBlipDisplay(blip, 2); // default 2 
			API.SetBlipScale(blip, 1f);
			API.SetBlipColour(blip, 1);
			API.SetBlipAsShortRange(blip, false);
			API.BeginTextCommandSetBlipName("STRING");
			API.AddTextComponentString(blipName);
			API.EndTextCommandSetBlipName(blip);

		}

		public Vector3 getMarkerPosition() {
			return new Vector3(this.posX, this.posY, this.posZ);
		}

	}
}
