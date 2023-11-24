using System;
using System.Linq;
using System.Reflection;

namespace Client;

public class Enums{
    public enum SeatPosition{
        SF_FrontDriverSide = -1,
        SF_FrontPassengerSide = 0,
        SF_BackDriverSide = 1,
        SF_BackPassengerSide = 2,
        SF_AltFrontDriverSide = 3,
        SF_AltFrontPassengerSide = 4,
        SF_AltBackDriverSide = 5,
        SF_AltBackPassengerSide = 6,
    };

    public enum CarLock{
        CARLOCK_NONE = 0,
        CARLOCK_UNLOCKED = 1,
        CARLOCK_LOCKED = 2,
        CARLOCK_LOCKOUT_PLAYER_ONLY = 3,
        CARLOCK_LOCKED_PLAYER_INSIDE = 4,
        CARLOCK_LOCKED_INITIALLY = 5,
        CARLOCK_FORCE_SHUT_DOORS = 6,
        CARLOCK_LOCKED_BUT_CAN_BE_DAMAGED = 7
    };

    /*
     *
     * 0 = Front Left Door
    1 = Front Right Door
    2 = Back Left Door
    3 = Back Right Door
    4 = Hood
    5 = Trunk
    6 = Back
    7 = Back2
     */
    public enum DoorIndex{
        [Door("Front Left Door")] FRONT_LEFT_DOOR = 0,
        [Door("Front Right Door")] FRONT_RIGHT_DOOR = 1,
        [Door("Back Left Door")] BACK_LEFT_DOOR = 2,
        [Door("Back Right Door")] BACK_RIGHT_DOOR = 3,
        [Door("Hood")] HOOD = 4,
        [Door("Trunk")] TRUNK = 5,
        [Door("Back")] BACK = 6,
        [Door("Back2")] BACK2 = 7
    }


    public class DoorAttribute : Attribute{
        public string Name{ get; private set; }

        internal DoorAttribute(string name){
            Name = name;
        }
    }

    public static DoorAttribute GetAttributeDoorIndex(DoorIndex doorIndex){
        FieldInfo fieldInfo = typeof(DoorIndex).GetField(doorIndex.ToString());
        return (DoorAttribute)fieldInfo.GetCustomAttributes(typeof(DoorAttribute), false).FirstOrDefault();
    }

    private static MemberInfo ForValue(DoorIndex p){
        return typeof(DoorIndex).GetField(Enum.GetName(typeof(DoorIndex), p));
    }
}