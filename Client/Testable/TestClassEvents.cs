
namespace Client.Testable;

public static class TestClassEvents{
    public static void Handle(){
         EventDispatcher.Mount("event:entering_vehicle",
                                      new Action<int, int, string, int>((vehicle, seat, vehicleName, netID) => {
                                          Debug
                                             .WriteLine($"[PlayerEnteringVehicle] Vehicle: {vehicle}, Seat: {seat}, VehicleName: {vehicleName}, NetID: {netID}");
                                      }));
         EventDispatcher.Mount("event:entered_vehicle",
                                      new Action<int, int, string, int>((vehicle, seat, vehicleName, netID) => {
                                          Debug
                                             .WriteLine($"[PlayerEnteredVehicle] Vehicle: {vehicle}, Seat: {seat}, VehicleName: {vehicleName}, NetID: {netID}");
                                      }));
         EventDispatcher.Mount("event:left_vehicle",
                                      new Action<int, int, string, int>((vehicle, seat, vehicleName, netID) => {
                                          Debug
                                             .WriteLine($"[PlayerLeftVehicle] Vehicle: {vehicle}, Seat: {seat}, VehicleName: {vehicleName}, NetID: {netID}");
                                      }));
         EventDispatcher.Mount("event:entering_vehicle_aborted",
                                      new Action(() => {
                                          Debug.WriteLine($"[PlayerEnteringAbortedIntoVehicle] NoneArguments");
                                      }));
    }
}