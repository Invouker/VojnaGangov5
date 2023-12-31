using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Server{
    public static class Util{
        public static string GetLicense(Player player){
            return API.GetPlayerIdentifierByType(player.Handle, "license");
        }

        public static string GetIP(Player player){
            return API.GetPlayerEndpoint(player.Handle);
        }

        public static bool IsNumberInArray(IEnumerable<int> array, int number){
            return array.Any(numberArray => numberArray == number);
        }
    }
}