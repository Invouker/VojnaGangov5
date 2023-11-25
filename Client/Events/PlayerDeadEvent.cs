using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using ScaleformUI;

namespace Client.Events;

public static class PlayerDeadEvent{
    private static bool IsDead;
    private static bool HasBeenDead;
    private static int DiedAt;

    public static Task Tick(){
        var player = API.PlayerId();
        if (!API.NetworkIsPlayerActive(player)) return Task.FromResult(true);
        var ped = API.PlayerPedId();

        if (API.IsPedFatallyInjured(ped) && !IsDead){
            IsDead = true;
            if (DiedAt == 0)
                DiedAt = API.GetGameTimer();

            uint killerWeapon = 0;
            var killer = API.NetworkGetEntityKillerOfPlayer(player, ref killerWeapon);
            var killerEntityType = API.GetEntityType(killer);
            var killerType = -1;
            var killerInVehicle = false;
            var killerVehicleName = "";
            var killerVehicleSeat = 0;
            if (killerEntityType == 1){
                killerType = API.GetPedType(killer);
                if (API.IsPedInAnyVehicle(killer, false)){
                    killerInVehicle = true;
                    killerVehicleName =
                        API.GetDisplayNameFromVehicleModel((uint)API.GetEntityModel(API.GetVehiclePedIsUsing(killer)));
                    killerVehicleSeat = (int)Utils.GetSeatByPed(killer);
                }
            }

            var killerId = GetPlayerByEntityID(killer);
            if (killer != ped && killerId != -1 && API.NetworkIsPlayerActive(killerId)){
                killerId = API.GetPlayerServerId(killerId);
            }
            else{
                killerId = -1;
            }

            if (killer == ped || killer == -1){
                BaseScript.TriggerEvent("event:player_died", killerType, API.GetEntityCoords(ped, true));
                BaseScript.TriggerServerEvent("event:player_died", killerType, API.GetEntityCoords(ped, true));
                HasBeenDead = true;
                Debug.WriteLine("MenuHandler.IsAnyMenuOpen: " + MenuHandler.IsAnyMenuOpen);
            }
            else{
                Vector3 killerPos = API.GetEntityCoords(ped, true);
                var eventData = new{
                    killerType,
                    weaponhash = killerWeapon,
                    killerinveh = killerInVehicle,
                    killervehseat = killerVehicleSeat,
                    killervehname = killerVehicleName,
                    killerpos = killerPos
                };
                BaseScript.TriggerEvent("event:player_killed", killerId, eventData);
                BaseScript.TriggerServerEvent("event:player_killed", killerId, eventData);
                HasBeenDead = true;
            }
        }
        else if (!API.IsPedFatallyInjured(ped)){
            IsDead = false;
            DiedAt = -1;
        }

        if (!HasBeenDead && DiedAt > 0){
            BaseScript.TriggerEvent("event:player_wasted", API.GetEntityCoords(ped, true));
            BaseScript.TriggerServerEvent("event:player_wasted", API.GetEntityCoords(ped, true));
            HasBeenDead = true;
        }
        else if (HasBeenDead && DiedAt != -1 && DiedAt <= 0){
            HasBeenDead = false;
        }

        return Task.FromResult(true);
    }


    private static int GetPlayerByEntityID(int id){
        for (var i = 0; i < Var.MaxPlayers; i++){
            if (API.NetworkIsPlayerActive(i) && API.GetPlayerPed(i) == id)
                return i;
        }

        return -1;
    }
}