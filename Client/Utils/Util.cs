using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Client.Utils;

public static class Util{
    public static bool IsNumberInArray(int[] array, int number){
        return array.Any(numberArray => numberArray == number);
    }

    public static int CountDigits(int number){
        return number.ToString().Length;
    }

    public static string FormatWithDotSeparator(int number){
        return number.ToString("N0", CultureInfo.InvariantCulture);
    }

    public static int GetReputationToLevel(int level){
        return level * 827 + 1734 + level * 86;
    }

    public static Enums.SeatPosition GetSeatByPed(int ped){
        int vehicle = API.GetVehiclePedIsIn(API.PlayerPedId(), false);
        if (vehicle == 0) return Enums.SeatPosition.NONE;
        foreach (Enums.SeatPosition seatPosition in Enum.GetValues(typeof(Enums.SeatPosition))){
            int id = (int)seatPosition;
            if (API.IsVehicleSeatFree(vehicle, id)) continue;
            if (API.GetPedInVehicleSeat(vehicle, id) == ped)
                return seatPosition;
        }

        return Enums.SeatPosition.NONE;
    }

    public static async void SetWalkingAnimToPed(string anim){
        // For request, wait for load and set a ped to walking style.
        API.RequestAnimSet(anim);
        while (!API.HasAnimSetLoaded(anim))
            await BaseScript.Delay(0);

        API.SetPedMovementClipset(API.PlayerPedId(), anim, 1f);
    }

    public static void SetVehicleLockStatus(Enums.CarLock carLock, bool state){
        if (!API.IsPedInAnyVehicle(API.PlayerPedId(), false)) return;
        int vehicle = API.GetVehiclePedIsIn(API.PlayerPedId(), false);
        //API.SetVehicleDoorsLocked(vehicle, (int)carLock);
        API.SetVehicleDoorsLockedForAllPlayers(vehicle, state);
    }

    public static readonly List<dynamic> AnimWalkingList = new List<dynamic>{
        "Male", "Female", "Alien", "Armored", "Arogant", "Brave", "Casual",
        "Casual2", "Casual3", "Casual4", "Casual5", "Casual6", "Chichi",
        "Confident", "Cop", "Money", "Sad", "Hobo",
        "Jog", "Flee", "Muscle", "Hipster",
        "Gangster",
        "Wide", "Slow", "Sexy", "Scared", "Swagger", "Tough", "Trash",
        "Shady", "Posh", "Femme"
    };

    public static readonly List<dynamic> AnimWalkingListIndex = new List<dynamic>{
        "move_m@multiplayer", "move_f@multiplayer", "move_m@alien", "anim_group_move_ballistic",
        "move_f@arrogant@a", "move_m@brave", "move_m@casual@a",
        "move_m@casual@b", "move_m@casual@c", "move_m@casual@d", "move_m@casual@e", "move_m@casual@f",
        "move_f@chichi",
        "move_m@confident", "move_m@business@a", "move_m@money", "move_m@sad@a", "move_m@hobo@a",
        "move_m@jog@", "move_f@flee@a", "move_m@muscle@a", "move_m@hipster@a",
        "move_m@gangster@generic",
        "move_m@bag", "move_characters@jimmy@slow@", "move_f@sexy@a", "move_f@scared", "move_m@swagger",
        "move_m@tough_guy@", "clipset@move@trash_fast_turn",
        "move_m@shadyped@a", "move_m@posh@", "move_f@femme@"
    };
}