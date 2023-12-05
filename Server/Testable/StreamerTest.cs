using System;
using CitizenFX.Core;
using Server.Services;

namespace Server.Testable{
    public static class StreamerTest{
        public static void Init(){
            Main.Instance.AddEventHandler("playerConnected", new Action<Player>(PlayerPostJoin));
            Main.Instance.AddEventHandler("player:interact:marker", new Action<int>(OnMarkerInteract));
        }

        public static void PlayerPostJoin(Player player){
            //StreamerService.CreateBlip("Gang: Alt", -470.547f, -1719.703f, 18.67876f, 59, 255, 1, 2, 1f, false);
            StreamerService.Create3dText("test\n :*~bold~ huhu 1\n :)\n\nPress ~INPUT_PICKUP~ to interact.", -470.547f,
                                         -1719.703f, 18.67876f, 255, 30, 10, 0);
            StreamerService.CreateMarker(0, -470.547f, -1719.703f, 18.67876f, 1, 255, 255, 255, true);


            //StreamerService.CreateBlip("Gang: Alt", -460.547f, -1719.703f, 18.67876f, 59, 255, 5, 2, 1f, false);
            StreamerService.Create3dText("test\n :*~bold~ huhu 1\n :)\n\nPress ~INPUT_PICKUP~ to interact.", -460.547f,
                                         -1719.703f, 18.67876f, 255, 30, 10, 0);
            StreamerService.CreateMarker(1, -460.547f, -1719.703f, 18.67876f, 1, 255, 0, 0, false);

            StreamerService.CreateBlip("Town Hall", -544.6154f, -204.8176f, 38.21021f, 39, 255, 79, quickGps: true);
            StreamerService.CreateBlip("Bank", 248.0571f, 222.422f, 106.2836f, 2, 255, 108, quickGps: true);
            StreamerService.CreateBlip("Property", -15.98242f, 240.1714f, 109.5524f, 14, 255, 500, quickGps: false);
        }

        public static void OnMarkerInteract(int id){
            //sendMessage($"OnMarker Interact with id: {id}");
            Debug.WriteLine($"OnMarker Interact with id: {id}");
        }
    }
}