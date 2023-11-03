using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Client.Events{
    /*
     * Starting character creator on this position:
     * -468.547, -1719.703, 18.67876
     *
     * Camera position for character creator:
     * -463.4458, -1718.268, 18.65963
     */
    public class SpawnManager{
        public async static Task SpawnPlayer(){
            API.DoScreenFadeOut(500);

            var player = Player.Local.Handle;
            var playerPed = Game.PlayerPed.Handle;
            FreezePlayer(player, true);
            var model = new Model("mp_f_freemode_01");
            await Game.Player.ChangeModel(model);

            API.RequestModel(model);
            API.SetPlayerModel(player, model);
            API.SetModelAsNoLongerNeeded(model);
            API.SetPedDefaultComponentVariation(API.GetPlayerPed(-1));

            API.RequestCollisionForModel(model);
            API.SetEntityCoordsNoOffset(playerPed, -468.547f, -1719.703f, 18.67876f, false, false, true);
            API.NetworkResurrectLocalPlayer(-468.547f, -1719.703f, 18.67876f, 0, true, true);
            API.ClearPedTasksImmediately(playerPed);
            API.RemoveAllPedWeapons(playerPed, true);
            API.ClearPlayerWantedLevel(player);

            API.ShutdownLoadingScreen();
            API.DoScreenFadeIn(500);

            FreezePlayer(player, false);
        }


        private static void FreezePlayer(int player, bool freeze){
            var ped = API.GetPlayerPed(-1);
            if (freeze){
                if (API.IsEntityVisible(ped))
                    API.SetEntityVisible(ped, false, false);
                if (API.IsPedInAnyVehicle(ped, true))
                    API.SetEntityCollision(ped, false, true);
                if (API.IsPedFatallyInjured(ped))
                    API.ClearPedTasksImmediately(ped);

                API.FreezeEntityPosition(ped, true);
                API.SetPlayerInvincible(player, true);
            }
            else{
                if (!API.IsEntityVisible(ped))
                    API.SetEntityVisible(ped, true, true);
                if (!API.IsPedInAnyVehicle(ped, true))
                    API.SetEntityCollision(ped, true, true);
                API.FreezeEntityPosition(ped, false);
                API.SetPlayerInvincible(player, false);
            }
        }
    }
}