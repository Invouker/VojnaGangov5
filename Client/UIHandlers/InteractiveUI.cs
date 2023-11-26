using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Streamable;
using ScaleformUI;
using ScaleformUI.Menu;

namespace Client.UIHandlers;

public static class InteractiveUI{
    private static readonly Dictionary<int, bool> EngineState = new Dictionary<int, bool>();

    public static UIMenu GetInteractiveUI(){
        UIMenu interactiveMenu = new UIMenu("Interaction Menu", "Interaction menu for player", new PointF(20, 20),
                                            "commonmenu", "interaction_bgd"){
            BuildingAnimation = MenuBuildingAnimation.NONE,
            EnableAnimation = false,
            MaxItemsOnScreen = 6,
            ScrollingType = ScrollingType.CLASSIC,
        };

        #region Walking Style

        UIMenuListItem walkingStyle =
            new UIMenuListItem("Walking Style", Utils.AnimWalkingList, Var.WalkingStyle, "Change your walking style.");

        interactiveMenu.AddItem(walkingStyle);

        walkingStyle.OnListChanged += (sender, index) => {
            string selectedIndex = Utils.AnimWalkingListIndex.ToArray()[index];
            Utils.SetWalkingAnimToPed(selectedIndex);
            Var.WalkingStyle = index;
            BaseScript.TriggerServerEvent("player:interactive:walkingstyle", index);
        };

        #endregion

        #region Quick GPS

        List<dynamic> quickGPSList = new List<dynamic>{ "None" };


        foreach (IStreamer streamer in Streamer.Streamed){
            if (streamer is not MapBlip{ QuickGPS: true } mapBlip) continue;
            quickGPSList.Add(mapBlip.BlipName);
            Route.QuickRouteGps.Add(mapBlip);
        }

        UIMenuListItem quickGPS = new UIMenuListItem("Quick GPS", quickGPSList, Var.GPSRoute,
                                                     "Select your quick GPS. You should be in vehicle to activate it.");
        interactiveMenu.AddItem(quickGPS);

        quickGPS.OnListChanged += (item, index) => {
            if (index == 0){
                Route.IsRouteSelected = false;
                Route.IsRouteFinished = true;
                Var.GPSRoute = 0;
                API.ClearAllBlipRoutes();
                return;
            }

            MapBlip mapBlip = Route.QuickRouteGps.ToArray()[index + 1];
            API.SetBlipRoute(mapBlip.Id, true);
            Route.IsRouteSelected = true;
            Route.IsRouteFinished = false;
            Route.BlipRoute = mapBlip.Id;
            Route.BlipRoutePosition = new Vector3(mapBlip.x, mapBlip.y, mapBlip.z);
            Var.GPSRoute = index;
        };

        #endregion

        UIMenuItem killYourself = new UIMenuItem("Kill yourself", "You will lose a 5% of your wallet.");
        interactiveMenu.AddItem(killYourself);

        #region Vehicle

        VehicleSubMenu(interactiveMenu);
        //Open

        #endregion

        killYourself.Activated += (sender, item) => { Game.Player.Character.Kill(); };

        return interactiveMenu;
    }

    private static void VehicleSubMenu(UIMenu interactiveMenu){
        int vehicle = API.GetVehiclePedIsIn(API.PlayerPedId(), false);
        UIMenuItem ToVehicleItem = new UIMenuItem("Vehicle interaction",
                                                  "Control your vehicle with this menu, the condition is to be the chauffeur in the vehicle!");
        // Check if player is in vehicle and if is driver.
        if (!API.IsPedInAnyVehicle(API.PlayerPedId(), false)){
            ToVehicleItem.SetLeftBadge(BadgeIcon.LOCK);
            ToVehicleItem.Enabled = false;
        }
        else if (API.GetPedInVehicleSeat(vehicle, (int)Enums.SeatPosition.SF_FrontDriverSide) != API.PlayerPedId()){
            //Check if is driver
            ToVehicleItem.SetLeftBadge(BadgeIcon.LOCK);
            ToVehicleItem.Enabled = false;
        }

        var vehClass = API.GetVehicleClass(vehicle);
        switch (vehClass){
            case 0: //compacts
            case 1: //sedan
            case 2: //suv's
            case 3: //coupes
            case 4: //muscle
            case 5: //sport classic
            case 6: //sport
            case 7: //super
            case 8: //motorcycle
            case 9: // offroad
            case 10: //industrial
            case 11: //utility ????
            case 12:{ } //vans
                break;
            default:{ // Others like: bicycles, boats, Helicopters, plane, service, emergency, military
                ToVehicleItem.SetLeftBadge(BadgeIcon.LOCK);
                ToVehicleItem.Enabled = false;
                break;
            }
        }

        ToVehicleItem.SetRightLabel(">>");
        interactiveMenu.AddItem(ToVehicleItem);

        UIMenu vehicleMenu = new UIMenu("Vehicle menu", "Control of vehicle", new PointF(20, 20), "commonmenu",
                                        "interaction_bgd"){
            BuildingAnimation = MenuBuildingAnimation.NONE,
            EnableAnimation = false,
            MaxItemsOnScreen = 6,
            ScrollingType = ScrollingType.CLASSIC,
        };

        UIMenuCheckboxItem lockVehicleItem =
            new UIMenuCheckboxItem("Lock Vehicle", UIMenuCheckboxStyle.Tick, false,
                                   "Prevent players to enter into vehicle.");
        vehicleMenu.AddItem(lockVehicleItem);

        UIMenuItem ejectPlayers = new UIMenuItem("Eject players",
                                                 "Halt all players in the vehicle and eject them from the vehicle.");
        vehicleMenu.AddItem(ejectPlayers);

        UIMenuCheckboxItem engineControl = new UIMenuCheckboxItem("Start/Stop the engine", UIMenuCheckboxStyle.Tick,
                                                                  !EngineState.TryGetValue(vehicle, out bool isOn) ||
                                                                  isOn,
                                                                  "Easily start or stop a engine at your vehicle.");
        vehicleMenu.AddItem(engineControl);

        List<dynamic> DoorsName = (from Enums.DoorIndex doorIndex in Enum.GetValues(typeof(Enums.DoorIndex))
                                   select Enums.GetAttributeDoorIndex(doorIndex).Name).Cast<dynamic>().ToList();
        UIMenuListItem doorInteract = new UIMenuListItem("Interact specific door", DoorsName, 0,
                                                         "Manipulate with specific door on your vehicle.");
        vehicleMenu.AddItem(doorInteract);

        UIMenuCheckboxItem controlDoor = new UIMenuCheckboxItem("Interact with all doors", UIMenuCheckboxStyle.Tick,
                                                                false, "Open or close all doors on vehicle.");
        vehicleMenu.AddItem(controlDoor);

        UIMenuItem back = new UIMenuItem("Return to Interaction", "Switch to interaction menu.");
        vehicleMenu.AddItem(back);

        doorInteract.OnListSelected += (sender, index) => { InteractVehicleSpecificDoor(index); };
        engineControl.CheckboxEvent += async (sender, @checked) => {
            if (!@checked){
                API.BringVehicleToHalt(vehicle, 400, 3000, true);
            }
            else{
                API.StopBringVehicleToHalt(vehicle);
            }

            await BaseScript.Delay(1500);
            API.SetVehicleEngineOn(vehicle, @checked, true, true);
            EngineState[vehicle] = @checked;
        };

        controlDoor.CheckboxEvent += (sender, @checked) => InteractVehicleDoor();
        ejectPlayers.Activated += (sender, item) => EjectAllPlayersFromVehicle();
        lockVehicleItem.CheckboxEvent += (sender, @checked) =>
            Utils.SetVehicleLockStatus(Enums.CarLock.CARLOCK_UNLOCKED, @checked);
        interactiveMenu.OnItemSelect += (sender, item, index) => interactiveMenu.SwitchTo(vehicleMenu);
        back.Activated += (sender, item) => vehicleMenu.SwitchTo(interactiveMenu);

        Main.Instance.AddEventHandler("event:entered_vehicle", new Action<int, int, string, int>((vehicle, seat,
                                                   vehicleName, netID) => {
                                                   ToVehicleItem.Enabled = true;
                                                   ToVehicleItem.SetLeftBadge(BadgeIcon.NONE);
                                               }));
        Main.Instance.AddEventHandler("event:left_vehicle", new Action<int, int, string, int>((vehicle, seat,
                                                   vehicleName, netID) => {
                                                   ToVehicleItem.Enabled = false;
                                                   ToVehicleItem.SetLeftBadge(BadgeIcon.LOCK);

                                                   if (vehicleMenu.Visible){
                                                       vehicleMenu.SwitchTo(interactiveMenu);
                                                   }
                                               }));
    }

    private static void InteractVehicleDoor(){
        foreach (Enums.DoorIndex door in Enum.GetValues(typeof(Enums.DoorIndex))){ // Itearate trought all doors
            int id = (int)door;
            InteractVehicleSpecificDoor(id);
        }
    }

    private static void InteractVehicleSpecificDoor(int id){
        int vehicle = API.GetVehiclePedIsIn(API.PlayerPedId(), false);
        if (vehicle == 0) return;

        float angleRation = API.GetVehicleDoorAngleRatio(vehicle, id);
        if (angleRation == 0f){ // If door on vehicle is fully opened
            API.SetVehicleDoorOpen(vehicle, id, false, false);
        }
        else // closed
            API.SetVehicleDoorShut(vehicle, id, false);
    }

    private static void EjectAllPlayersFromVehicle(){
        int vehicle = API.GetVehiclePedIsIn(API.PlayerPedId(), false);
        if (vehicle == 0) return;
        foreach (Enums.SeatPosition seatPosition in Enum.GetValues(typeof(Enums.SeatPosition))){
            int id = (int)seatPosition;
            if (id == -1) continue; // Skip driver
            if (API.IsVehicleSeatFree(vehicle, id)) continue;
            int ped = API.GetPedInVehicleSeat(vehicle, id);
            API.TaskLeaveVehicle(ped, vehicle, 0);
        }
    }

    public static async Task Tick(){
        if (!Route.IsRouteSelected || Route.IsRouteFinished) return;
        if (Route.BlipRoutePosition.IsZero) return;

        if (Route.BlipRoutePosition.DistanceToSquared2D(Player.Local.Character.Position) <= 2300){
            Route.IsRouteFinished = true;
            API.SetBlipRoute(Route.BlipRoute, false);
        }

        await BaseScript.Delay(1000); // Tick every second
    }

    private static class Route{
        public static readonly List<MapBlip> QuickRouteGps = new List<MapBlip>();

        public static bool IsRouteSelected;
        public static int BlipRoute;
        public static Vector3 BlipRoutePosition;
        public static bool IsRouteFinished;
    }
}