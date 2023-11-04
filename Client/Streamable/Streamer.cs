using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client.Streamable{
    internal static class Streamer{
        public static readonly List<IStreamer> Streamed = new List<IStreamer>();

        public static void stream(){
            foreach (IStreamer streamer in Streamed)
                if (!(streamer is MapBlip))
                    streamer.Render();
        }

        public static void CreateMarker(int id, float x, float y, float z, int type = 1, int red = 255, int green = 255,
            int blue = 255, bool allowVehicleInteract = true){
            var marker = new Marker(id, type, x, y, z, red, green, blue, allowVehicleInteract);
            Streamed.Add(marker);
        }

        public static void Create3dText(string text, float x, float y, float z, int red = 255, int green = 255,
            int blue = 255, int fontType = 0){
            var worldText = new WorldText(text, x, y, z, red, green, blue, fontType);
            Streamed.Add(worldText);
        }

        public static void CreateBlip(string blipName, float x, float y, float z, int colour = 0, int alpha = 255,
            int blipSprite = 84, int blipDisplay = 2, float blipScale = 1f, bool showAsShortRange = false){
            var mapBlip = new MapBlip(blipName, x, y, z, colour, alpha, blipSprite, blipDisplay, blipScale,
                                      showAsShortRange);
            Streamed.Add(mapBlip);
            mapBlip.Render();
        }
    }

    public interface IStreamer{
        void Render();
    }

    internal class WorldText : IStreamer{
        private const int RenderDistance = 45;
        private string Text{ get; set; }
        private readonly float _x, _y, _z;
        private readonly int _red, _green, _blue;
        private int FontType{ get; }

        public WorldText(string text, float x, float y, float z, int red = 255, int green = 255, int blue = 255,
            int fontType = 0){
            Text = text;
            _x = x;
            _y = y;
            _z = z;
            _red = red;
            _green = green;
            _blue = blue;
            FontType = fontType;
        }

        public void Render(){
            var textPosition = new Vector3(_x, _y, _z);
            var screenPos = World3DToScreen2d(textPosition);
            if (screenPos.Equals(Vector2.Zero))
                return;
            var camPos = API.GetGameplayCamCoords();
            var distance = API.GetDistanceBetweenCoords(camPos.X, camPos.Y, camPos.Z, textPosition.X, textPosition.Y,
                                                        textPosition.Z, true);
            if (RenderDistance < distance)
                return;

            API.SetTextScale(0.0f, 0.28f);
            API.SetTextFont(FontType); // Font.HouseScript
            API.SetTextProportional(true);
            API.SetTextColour(_red, _green, _blue, 255);
            API.SetTextEdge(2, 0, 0, 0, 150);
            API.SetTextEntry("STRING");
            API.SetTextCentre(true);
            API.SetTextEdge(1, 0, 0, 0, 255);
            API.SetTextOutline();
            API.AddTextComponentString(Text); // ~bold~[Gang HQ]\nMajitel: Invouk\nRešpekt: 0\nMajetok: 25,655,657$\n~w~Press ~INPUT_TALK~ to get info.
            API.DrawText(screenPos.X, screenPos.Y);
            API.ClearDrawOrigin();
            /*if(background) {
                /var factor = (text.Length) / 140;
                API.DrawRect(screenPos.X, screenPos.Y + 0.0125f, 0.057f + factor, 0.03f, 0, 0, 0, 75);
            }*/
        }

        private Vector2 World3DToScreen2d(Vector3 pos){
            float screenX = 0;
            float screenY = 0;
            var success = API.GetScreenCoordFromWorldCoord(pos.X, pos.Y, pos.Z, ref screenX, ref screenY);
            return success ? new Vector2(screenX, screenY) : Vector2.Zero;
        }
    }

    internal class Marker : IStreamer{
        private static List<int> UsedMarkers = new List<int>();
        private const int RenderDistance = 40;

        public int _id{ get; private set; }
        private int MarkerType{ get; }
        private float _x{ get; }
        private float _y{ get; }
        private float _z{ get; }
        private int Red{ get; }
        private int Green{ get; }
        private int Blue{ get; }
        public bool AllowVehicleInteract{ get; }

        public Marker(int id, int type, float x, float y, float z, int red = 255, int green = 255, int blue = 255,
            bool allowVehcleInteract = true){
            if (UsedMarkers.Contains(id))
                throw new ArgumentOutOfRangeException($"Marker already registred with id of {id}, please use another!");
            _id = id;
            MarkerType = type;
            _x = x;
            _y = y;
            _z = z;
            Red = red;
            Green = green;
            Blue = blue;
            AllowVehicleInteract = allowVehcleInteract;

            UsedMarkers.Add(id);
        }

        public Vector3 GetMarkerPosition(){
            return new Vector3(_x, _y, _z);
        }

        public void Render(){
            var playerPos = GetPlayerPosition();
            var distance = API.GetDistanceBetweenCoords(_x, _y, _z, playerPos.X, playerPos.Y, playerPos.Z, true);
            if (distance > RenderDistance)
                return;

            API.DrawMarker(MarkerType, _x, _y, _z - 1.5f, 0, 0, 0, 0, 0, 0, 1.75f, 1.75f, 1.75f, Red, Green, Blue,
                           255, false, true, 2, true, null, null, false);
        }

        private Vector3 GetPlayerPosition(){
            return new Vector3(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z);
        }
    }

    internal class MapBlip : IStreamer{
        private readonly float _x, _y, _z;
        private readonly int _blipSprite;
        private readonly int _blipDisplay;
        private readonly float _blipScale;
        private readonly int _colour;
        private readonly int _alpha;
        private readonly bool _showAsShortRange;

        private readonly String _blipName;

        public MapBlip(string blipName, float x, float y, float z, int colour = 0, int alpha = 255, int blipSprite = 1,
            int blipDisplay = 2, float blipScale = 1f, bool showAsShortRange = false){
            _x = x;
            _y = y;
            _z = z;
            _blipSprite = blipSprite;
            _blipDisplay = blipDisplay;
            _blipScale = blipScale;
            _colour = colour;
            _alpha = alpha;
            _showAsShortRange = showAsShortRange;
            _blipName = blipName;
        }

        public void Render(){
            var blip = API.AddBlipForCoord(_x, _y, _z);
            API.SetBlipSprite(blip, _blipSprite);
            API.SetBlipDisplay(blip, _blipDisplay);
            API.SetBlipScale(blip, _blipScale);
            API.SetBlipColour(blip, _colour);
            API.SetBlipAlpha(blip, _alpha);
            API.SetBlipAsShortRange(blip, _showAsShortRange);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString(_blipName);
            API.EndTextCommandSetBlipName(blip);
        }
    }
}