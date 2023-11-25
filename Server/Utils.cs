using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Server{
    public static class Utils{
        public static string GetLicense(Player player){
            return API.GetPlayerIdentifierByType(player.Handle, "license");
        }

        public static bool IsNumberInArray(IEnumerable<int> array, int number){
            foreach (var numberArray in array){
                if (numberArray == number)
                    return true;
            }

            return false;
        }
    }
}