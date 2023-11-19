using System.Globalization;

namespace Client;

public static class Utils{
    public static bool IsNumberInArray(int[] array, int number){
        foreach (var numberArray in array){
            if (numberArray == number)
                return true;
        }

        return false;
    }

    public static int CountDigits(int number){
        return number.ToString().Length;
    }

    public static string FormatWithDotSeparator(int number){
        return number.ToString("N0", CultureInfo.InvariantCulture);
    }
}