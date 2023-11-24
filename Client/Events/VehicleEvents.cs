using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client.Events;

/*
 * Recoded from BaseEvents, default resource from FiveM into C#
 * https://github.com/citizenfx/cfx-server-data/blob/0e7ba538339f7c1c26d0e689aa750a336576cf02/resources/%5Bsystem%5D/baseevents/vehiclechecker.lua#L41
 */
public static class VehicleEvents{
    private static bool IsInVehicle;
    private static bool IsEnteringVehicle;
    private static int CurrentVehicle;
    private static int CurrentSeat;


    public static async Task Tick(){
        int ped = API.PlayerPedId();
        if (!IsInVehicle && !API.IsPlayerDead(ped)){
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
                await BaseScript.Delay(100);
            }
            else if (API.DoesEntityExist(API.GetVehiclePedIsTryingToEnter(ped)) && !API.IsPedInAnyVehicle(ped, true) &&
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
                //var model = API.GetEntityModel(CurrentVehicle);
                //var name = API.GetDisplayNameFromVehicleModel((uint)CurrentVehicle);
                var netId = API.VehToNet(CurrentVehicle);
                BaseScript.TriggerServerEvent("event:entered_vehicle", CurrentVehicle, CurrentSeat,
                                              API.GetDisplayNameFromVehicleModel((uint)API
                                                 .GetEntityModel(CurrentVehicle)), netId);
                BaseScript.TriggerEvent("event:entered_vehicle", CurrentVehicle, CurrentSeat,
                                        API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(CurrentVehicle)),
                                        netId);
                await BaseScript.Delay(100);
            }
        }
        else if (IsInVehicle){
            if (!API.IsPedInAnyVehicle(ped, false) && !API.IsPlayerDead(API.PlayerId())) return;
            //var model = API.GetEntityModel(CurrentVehicle);
            //var name = API.GetDisplayNameFromVehicleModel((uint)CurrentVehicle);
            var netId = API.VehToNet(CurrentVehicle);
            BaseScript.TriggerServerEvent("event:left_vehicle", CurrentVehicle, CurrentSeat,
                                          API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(CurrentVehicle)),
                                          netId);
            BaseScript.TriggerEvent("event:left_vehicle", CurrentVehicle, CurrentSeat,
                                    API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(CurrentVehicle)),
                                    netId);
            IsInVehicle = false;
            CurrentVehicle = 0;
            CurrentSeat = 0;
            await BaseScript.Delay(100);
        }
    }
}