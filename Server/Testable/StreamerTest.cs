using CitizenFX.Core;
using Server.Services;

namespace Server.Testable{
    public class StreamerTest{
        public static void PlayerPostJoin(Player player){
            //StreamerService.CreateBlip("Gang: Alt", -470.547f, -1719.703f, 18.67876f, 59, 255, 1, 2, 1f, false);
            StreamerService.Create3dText("test\n :*~bold~ huhu 1\n :)\n\nPress ~INPUT_PICKUP~ to interact.", -470.547f,
                                         -1719.703f, 18.67876f, 255, 30, 10, 0);
            StreamerService.CreateMarker(0, -470.547f, -1719.703f, 18.67876f, 1, 255, 255, 255, true);


            //StreamerService.CreateBlip("Gang: Alt", -460.547f, -1719.703f, 18.67876f, 59, 255, 5, 2, 1f, false);
            StreamerService.Create3dText("test\n :*~bold~ huhu 1\n :)\n\nPress ~INPUT_PICKUP~ to interact.", -460.547f,
                                         -1719.703f, 18.67876f, 255, 30, 10, 0);
            StreamerService.CreateMarker(1, -460.547f, -1719.703f, 18.67876f, 1, 255, 0, 0, false);
        }

        public static void OnMarkerInteract(int id){
            //sendMessage($"OnMarker Interact with id: {id}");
            Debug.WriteLine($"OnMarker Interact with id: {id}");
        }
    }
}