using CitizenFX.Core;

namespace Client;

public static class Utils{
    public static bool IsNumberInArray(int[] array, int number){
        foreach (var numberArray in array){
            if (numberArray == number){
                Debug.WriteLine("Contains a " + number);
                return true;
            }
        }

        return false;
    }
}