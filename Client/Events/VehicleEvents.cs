using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client.Events;

/*
 * Recoded from BaseEvents, default resource from FiveM into C#
 * https://github.com/citizenfx/cfx-server-data/blob/0e7ba538339f7c1c26d0e689aa750a336576cf02/resources/%5Bsystem%5D/baseevents/vehiclechecker.lua#L41
 */
public static class VehicleEvents{
    private static bool IsInVehicle = false;
    private static bool IsEnteringVehicle = false;
    private static int CurrentVehicle = 0;
    private static int CurrentSeat = 0;

    public static async Task Tick(){
        VehicleNumberPlates();

        int ped = API.PlayerPedId();
        if (!IsInVehicle && !API.IsPlayerDead(ped) && Game.Player.Character.Health > 0){
            if (API.DoesEntityExist(API.GetVehiclePedIsTryingToEnter(ped)) && !IsEnteringVehicle){
                var vehicle = API.GetVehiclePedIsTryingToEnter(ped);
                var seat = API.GetSeatPedIsTryingToEnter(ped);
                var netId = API.VehToNet(vehicle);
                IsEnteringVehicle = true;
                BaseScript.TriggerServerEvent("event:entering_vehicle", vehicle, seat,
                                              API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(vehicle)),
                                              netId);
                BaseScript.TriggerEvent("event:entering_vehicle", vehicle, seat,
                                        API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(vehicle)), netId);
                // event:entering_vehicle - int,int,string,int
                await BaseScript.Delay(100);
            }
            else if (!API.DoesEntityExist(API.GetVehiclePedIsTryingToEnter(ped)) && !API.IsPedInAnyVehicle(ped, true) &&
                     IsEnteringVehicle){
                BaseScript.TriggerServerEvent("event:entering_vehicle_aborted");
                IsEnteringVehicle = false;
                await BaseScript.Delay(100);
            }
            else if (API.IsPedInAnyVehicle(ped, false)){
                IsEnteringVehicle = false;
                IsInVehicle = true;
                CurrentVehicle = API.GetVehiclePedIsUsing(ped);
                CurrentSeat = (int)Utils.GetSeatByPed(ped);
                var model = API.GetEntityModel(CurrentVehicle);
                var name = API.GetDisplayNameFromVehicleModel((uint)model);
                var netId = API.VehToNet(CurrentVehicle);
                BaseScript.TriggerServerEvent("event:entered_vehicle", CurrentVehicle, CurrentSeat, name, netId);
                BaseScript.TriggerEvent("event:entered_vehicle", CurrentVehicle, CurrentSeat, name, netId);
                // event:entered_vehicle - int,int,string,int
                await BaseScript.Delay(100);
            }
        }
        else if (IsInVehicle && (!API.IsPedInAnyVehicle(ped, false) || API.IsPlayerDead(API.PlayerId()))){
            //var model = API.GetEntityModel(CurrentVehicle);
            //var name = API.GetDisplayNameFromVehicleModel((uint)CurrentVehicle);
            var netId = API.VehToNet(CurrentVehicle);
            BaseScript.TriggerServerEvent("event:left_vehicle", CurrentVehicle, CurrentSeat,
                                          API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(CurrentVehicle)),
                                          netId);
            BaseScript.TriggerEvent("event:left_vehicle", CurrentVehicle, CurrentSeat,
                                    API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(CurrentVehicle)),
                                    netId);
            // event:left_vehicle - int, int, string, int
            IsInVehicle = false;
            CurrentVehicle = 0;
            CurrentSeat = 0;
            await BaseScript.Delay(100);
        }
    }

    private static void VehicleNumberPlates(){
        Vector3 playerPos = Game.Player.Character.Position;
        string zoneName = API.GetNameOfZone(playerPos.X, playerPos.Y, playerPos.Z)
                             .Substring(0, 2)
                             .Replace("A", "^A");
        API.SetDefaultVehicleNumberPlateTextPattern(-1, $"{zoneName} AAA11");
    }
}