namespace Client;

public static class Utils{
    public static bool IsNumberInArray(int[] array, int number){
        foreach (var numberArray in array){
            if (numberArray == number)
                return true;
        }

        return false;
    }
}