using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using ScaleformUI;

namespace Client.Testable;

public class TestClassEvents{
    public static void Handle(){
        Main.Instance.AddEventHandler("event:entering_vehicle",
                                      new Action<int, int, string, int>((vehicle, seat, vehicleName, netID) => {
                                          Debug
                                             .WriteLine($"[PlayerEnteringVehicle] Vehicle: {vehicle}, Seat: {seat}, VehicleName: {vehicleName}, NetID: {netID}");
                                      }));
        Main.Instance.AddEventHandler("event:entered_vehicle",
                                      new Action<int, int, string, int>((vehicle, seat, vehicleName, netID) => {
                                          Debug
                                             .WriteLine($"[PlayerEnteredVehicle] Vehicle: {vehicle}, Seat: {seat}, VehicleName: {vehicleName}, NetID: {netID}");
                                      }));
        Main.Instance.AddEventHandler("event:left_vehicle",
                                      new Action<int, int, string, int>((vehicle, seat, vehicleName, netID) => {
                                          Debug
                                             .WriteLine($"[PlayerLeftVehicle] Vehicle: {vehicle}, Seat: {seat}, VehicleName: {vehicleName}, NetID: {netID}");
                                      }));
        Main.Instance.AddEventHandler("event:entering_vehicle_aborted",
                                      new Action(() => {
                                          Debug.WriteLine($"[PlayerEnteringAbortedIntoVehicle] NoneArguments");
                                      }));


        Main.Instance.AddEventHandler("event:player_died", new Action(SpawnAfterDie));
    }

    private static async void SpawnAfterDie(){
        while (API.IsScreenFadingOut())
            await BaseScript.Delay(1);

        Debug.WriteLine("You died and will be respawned!");
        await BaseScript.Delay(3000);
        int ped = API.PlayerPedId();
        int player = API.PlayerId();
        API.ClearPedTasksImmediately(ped);
        API.ClearPlayerWantedLevel(player);
        API.NetworkResurrectLocalPlayer(341.4725f, -1396.971f, 32.49817f, 48.188797f, true, false);
        API.SetEntityHealth(player, 100);
        API.SetEntityCoordsNoOffset(ped, 341.4725f, -1396.971f, 32.49817f, false, false, true);
        API.RequestCollisionAtCoord(341.4725f, -1396.971f, 32.49817f);

        MenuHandler.CloseAndClearHistory();
        await BaseScript.Delay(500);
        API.DoScreenFadeIn(1500);
    }
}