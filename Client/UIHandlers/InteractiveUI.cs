using System.Collections.Generic;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using ScaleformUI.Menu;

namespace Client.UIHandlers;

public static class InteractiveUI{
    public static UIMenu GetInteractiveUI(){
        UIMenu interactiveMenu = new UIMenu("Interaction Menu", "Interaction menu for player", new PointF(20, 20),
                                            "commonmenu", "interaction_bgd"){
            BuildingAnimation = MenuBuildingAnimation.NONE,
            EnableAnimation = false,
            MaxItemsOnScreen = 6,
            ScrollingType = ScrollingType.CLASSIC,
        };

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
        UIMenuItem killYourself = new UIMenuItem("Kill yourself", "You will lose a 5% of your wallet.");

        interactiveMenu.AddItem(killYourself);
        interactiveMenu.AddItem(walkingStyle);

        walkingStyle.OnListChanged += (sender, index) => {
            string selectedIndex = animListIndex.ToArray()[index];
            SetAnimToPed(selectedIndex);
            Var.WalkingStyle = index;
            BaseScript.TriggerServerEvent("player:interactive:walkingstyle", index);
        };
        interactiveMenu.OnItemSelect += (sender, item, index) => { Debug.WriteLine("OnItemSelect of " + item); };


        return interactiveMenu;
    }

    private static async void SetAnimToPed(string anim){
        API.RequestAnimSet(anim);
        while (!API.HasAnimSetLoaded(anim))
            await BaseScript.Delay(0);

        API.SetPedMovementClipset(API.PlayerPedId(), anim, 1f);
    }
}