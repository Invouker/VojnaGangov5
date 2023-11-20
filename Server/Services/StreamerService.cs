using CitizenFX.Core;

namespace Server.Services{
    public class StreamerService : IService{
        /// <summary>
        /// Create Marker (checkpoint) in serverside for everyone.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="type"></param>
        public static void CreateMarker(int id, float x, float y, float z, int type = 1, int red = 255, int green = 255,
            int blue = 255, bool allowVehcleInteract = true){
            BaseScript.TriggerClientEvent("streamer:createMarker", id, x, y, z, type, red, green, blue,
                                          allowVehcleInteract);
        }

        /// <summary>
        /// Create 3dtext Label in serverside for everyone.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="fontType"></param>
        public static void Create3dText(string text, float x, float y, float z, int red = 255, int green = 255,
            int blue = 255, int fontType = 0){
            BaseScript.TriggerClientEvent("streamer:create3dText", text, x, y, z, red, green, blue, fontType);
        }

        /// <summary>
        /// Create Blip only in PostPlayerJoin event on fresh start of server.
        /// </summary>
        /// <param name="blipName"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="posZ"></param>
        /// <param name="colour"></param>
        /// <param name="alpha"></param>
        /// <param name="blipSprite"></param>
        /// <param name="blipDisplay"></param>
        /// <param name="blipScale"></param>
        /// <param name="showAsShortRange"></param>
        /// <param name="quickGps"></param>
        public static void CreateBlip(string blipName, float posX, float posY, float posZ, int colour = 0,
            int alpha = 255, int blipSprite = 84, int blipDisplay = 2, float blipScale = 1f,
            bool showAsShortRange = false, bool quickGps = false){
            BaseScript.TriggerClientEvent("streamer:createBlip", blipName, posX, posY, posZ, colour, alpha, blipSprite,
                                          blipDisplay, blipScale, showAsShortRange, quickGps);
        }
    }
}