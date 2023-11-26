using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using ScaleformUI;

namespace Client.Events;

public static class PlayerDeadEvent{
    private static bool IsDead;
    private static bool HasBeenDead;
    private static int DiedAt;

    static PlayerDeadEvent(){
        Main.Instance.AddEventHandler("event:player_died", new Action(SpawnAfterDie));
    }

    public static async Task Tick(){
        var player = API.PlayerId();
        if (!API.NetworkIsPlayerActive(player)) return;
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
                await DoWastedScreen("You committed suicide.");
                BaseScript.TriggerEvent("event:player_died", killerType, API.GetEntityCoords(ped, true));
                BaseScript.TriggerServerEvent("event:player_died", killerType, API.GetEntityCoords(ped, true));
                HasBeenDead = true;
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

                await DoWastedScreen("");
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
            await DoWastedScreen("");
            BaseScript.TriggerEvent("event:player_wasted", API.GetEntityCoords(ped, true));
            BaseScript.TriggerServerEvent("event:player_wasted", API.GetEntityCoords(ped, true));
            HasBeenDead = true;
        }
        else if (HasBeenDead && DiedAt != -1 && DiedAt <= 0){
            HasBeenDead = false;
        }
    }

    private static async Task DoWastedScreen(string reason){
        MenuHandler.CloseAndClearHistory();
        Debug.WriteLine("X: " + MenuHandler.IsAnyMenuOpen);
        Debug.WriteLine("Y: " + MenuHandler.CurrentMenu);
        Debug.WriteLine("Z: " + MenuHandler.CurrentPauseMenu);
        API.PlaySoundFrontend(-1, "Bed", "WastedSounds", true);
        API.StartScreenEffect("DeathFailMPDark", 0, false);
        await BaseScript.Delay(300);
        API.StartScreenEffect("DeathFailMPIn", 0, false);
        API.ShakeGameplayCam("DEATH_FAIL_IN_EFFECT_SHAKE", 1.200f);

        int ScaleForm = API.RequestScaleformMovie("MP_BIG_MESSAGE_FREEMODE");
        while (!API.HasScaleformMovieLoaded(ScaleForm))
            await BaseScript.Delay(1);

        API.BeginScaleformMovieMethod(ScaleForm, "SHOW_SHARD_WASTED_MP_MESSAGE");
        API.PushScaleformMovieMethodParameterString("~r~Wasted~w~");
        API.PushScaleformMovieMethodParameterString(reason);
        API.PushScaleformMovieMethodParameterInt(5);
        API.EndScaleformMovieMethod();

        int time = API.GetGameTimer();
        while (time + 6000 > API.GetGameTimer()){
            API.DrawScaleformMovieFullscreen(ScaleForm, 255, 255, 255, 255, 0);
            await BaseScript.Delay(0);
        }

        await BaseScript.Delay(1000);
        API.DoScreenFadeOut(800);
        while (!API.IsScreenFadedOut())
            await BaseScript.Delay(1);

        API.StopScreenEffect("DeathFailMPIn");
        API.StopScreenEffect("DeathFailMPDark");
    }


    private static int GetPlayerByEntityID(int id){
        for (var i = 0; i < Var.MaxPlayers; i++){
            if (API.NetworkIsPlayerActive(i) && API.GetPlayerPed(i) == id)
                return i;
        }

        return -1;
    }

    public static Dictionary<string, List<string>> DeathMessages = new Dictionary<string, List<string>>(){
        {
            "melee",
            new List<string>{ "melee killed you.", "beat you down.", "battered you.", "whacked you.", "murdered you." }
        }, //
        { "molotov", new List<string>{ "torched you.", "flambeed you.", "barbecued you." } },
        { "knife", new List<string>{ "knifed you.", "stabbed you.", "eviscerated you." } },{
            "pistol",
            new List<string>{ "pistoled you.", "popped you.", "blasted you.", "bust a cap in you.", "plugged you." }
        },
        { "smg", new List<string>{ "submachine gunned you.", "riddled you.", "drilled you.", "finished you." } }, //
        { "rifle", new List<string>{ "rifled you.", "shot you down.", "ended you.", "floored you." } }, //
        { "mg", new List<string>{ "machine gunned you.", "sprayed you.", "ruined you." } },
        { "shotgun", new List<string>{ "shotgunned you.", "pulverized you.", "devastated you." } }, //
        { "sniper", new List<string>{ "sniped you.", "scoped you.", "picked you off." } }, //
        { "heavy", new List<string>{ "destroyed you.", "erased you.", "annihilated you." } },
        { "minigun", new List<string>{ "ripped you apart.", "shredded you.", "wiped you out.", "owned you." } },
        { "explosive", new List<string>{ "blew you up.", "bombed you.", "exploded you." } }, //
        { "rotor", new List<string>{ "mowed you down." } },
        { "flatten", new List<string>{ "flattened you." } }
    };

    private static async void SpawnAfterDie(){
        const float posX = 341.4725f;
        const float posY = -1396.971f;
        const float posZ = 32.49817f;
        while (API.IsScreenFadingOut())
            await BaseScript.Delay(1);

        Debug.WriteLine("You died and will be respawned!");
        await BaseScript.Delay(3000);
        int ped = API.PlayerPedId();
        int player = API.PlayerId();
        API.ClearPedTasksImmediately(ped);
        API.ClearPlayerWantedLevel(player);
        API.NetworkResurrectLocalPlayer(posX, posY, posZ, 48.188797f, true, false);
        API.SetEntityHealth(player, 100);
        API.SetEntityCoordsNoOffset(ped, posX, posY, posZ, false, false, true);
        API.RequestCollisionAtCoord(posX, posY, posZ);

        MenuHandler.CloseAndClearHistory();
        await BaseScript.Delay(500);
        API.DoScreenFadeIn(1500);
    }
}