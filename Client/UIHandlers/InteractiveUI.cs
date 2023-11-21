using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Streamable;
using ScaleformUI.Menu;

namespace Client.UIHandlers;

public static class InteractiveUI{
    private static List<MapBlip> quickRouteGps = new List<MapBlip>();

    public static UIMenu GetInteractiveUI(){
        UIMenu interactiveMenu = new UIMenu("Interaction Menu", "Interaction menu for player", new PointF(20, 20),
                                            "commonmenu", "interaction_bgd"){
            BuildingAnimation = MenuBuildingAnimation.NONE,
            EnableAnimation = false,
            MaxItemsOnScreen = 6,
            ScrollingType = ScrollingType.CLASSIC,
        };

        #region Walking Style

        List<dynamic> animList = new List<dynamic>{
            "Male", "Female", "Alien", "Armored", "Arogant", "Brave", "Casual",
            "Casual2", "Casual3", "Casual4", "Casual5", "Casual6", "Chichi",
            "Confident", "Cop", "Money", "Sad", "Hobo",
            "Jog", "Flee", "Muscle", "Hipster",
            "Gangster",
            "Wide", "Slow", "Sexy", "Scared", "Swagger", "Tough", "Trash",
            "Shady", "Posh", "Femme"
        };

        List<dynamic> animListIndex = new List<dynamic>{
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

        UIMenuListItem walkingStyle =
            new UIMenuListItem("Walking Style", animList, Var.WalkingStyle, "Change your walking style.");

        interactiveMenu.AddItem(walkingStyle);

        walkingStyle.OnListChanged += (sender, index) => {
            string selectedIndex = animListIndex.ToArray()[index];
            SetAnimToPed(selectedIndex);
            Var.WalkingStyle = index;
            BaseScript.TriggerServerEvent("player:interactive:walkingstyle", index);
        };

        #endregion

        #region Quick GPS

        List<dynamic> quickGPSList = new List<dynamic>{ "None" };


        foreach (IStreamer streamer in Streamer.Streamed){
            if (streamer is not MapBlip{ QuickGPS: true } mapBlip) continue;
            quickGPSList.Add(mapBlip.BlipName);
            quickRouteGps.Add(mapBlip);
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

            MapBlip mapBlip = quickRouteGps.ToArray()[index + 1];
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

        interactiveMenu.OnItemSelect += (sender, item, index) => {
            if (item == killYourself){
                API.SetEntityHealth(API.PlayerPedId(), 0);
            }

            Debug.WriteLine("OnItemSelect of " + item);
        };

        // SetBlipRoute(Blip blip, bool enabled);
        return interactiveMenu;
    }

    private static async void SetAnimToPed(string anim){ // For request, wait for load and set a ped to walking style.
        API.RequestAnimSet(anim);
        while (!API.HasAnimSetLoaded(anim))
            await BaseScript.Delay(0);

        API.SetPedMovementClipset(API.PlayerPedId(), anim, 1f);
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

    internal class Route{
        public static bool IsRouteSelected;
        public static int BlipRoute;
        public static Vector3 BlipRoutePosition;
        public static bool IsRouteFinished;
    }
}