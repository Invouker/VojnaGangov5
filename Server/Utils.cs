using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Server{
    public static class Utils{
        public static string GetLicense(Player player){
            return API.GetPlayerIdentifierByType(player.Handle, "license");
        }
    }
}