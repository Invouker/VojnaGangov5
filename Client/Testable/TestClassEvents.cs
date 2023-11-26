using System;
using CitizenFX.Core;

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
    }
}