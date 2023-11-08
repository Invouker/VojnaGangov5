using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.ScaleformUI;

namespace Client.Events{
    /*
     * Starting character creator on this position:
     * -468.547, -1719.703, 18.67876
     *
     * Camera position for character creator:
     * -463.4458, -1718.268, 18.65963
     */

    public class SpawnManager{
        private const uint CreatorCamera = 10;
        public static bool isPlayerInCreator = false;

        public static async Task SpawnPlayer(){
            API.DoScreenFadeOut(500);

            var player = Player.Local.Handle;
            var playerPed = Game.PlayerPed.Handle;
            FreezePlayer(player, true);

            uint hash = (uint)API.GetHashKey("mp_m_freemode_01");
            API.RequestModel(hash);

            while (!API.HasModelLoaded(hash))
                await BaseScript.Delay(1);

            API.SetPlayerModel(player, hash);
            API.SetModelAsNoLongerNeeded(hash);

            API.SetPedDefaultComponentVariation(API.GetPlayerPed(-1));

            API.RequestCollisionAtCoord(-468.547f, -1719.703f, 18.67876f);
            API.SetEntityCoordsNoOffset(playerPed, -468.547f, -1719.703f, 18.67876f, false, false, true);
            API.NetworkResurrectLocalPlayer(-468.547f, -1719.703f, 18.67876f, 0, true, true);
            API.ClearPedTasksImmediately(playerPed);
            API.RemoveAllPedWeapons(playerPed, true);
            API.ClearPlayerWantedLevel(player);

            API.ShutdownLoadingScreen();
            API.DoScreenFadeIn(500);

            FreezePlayer(player, false);
            await Task.FromResult(true);
        }

        public static async void SpawnToCreator(){
            API.DoScreenFadeOut(500);

            var player = Player.Local.Handle;
            var playerPed = API.GetPlayerPed(-1);
            //FreezePlayer(player, true);

            uint hash = (uint)API.GetHashKey("mp_m_freemode_01"); // mp_f_freemode_01
            API.RequestModel(hash);

            while (!API.HasModelLoaded(hash))
                await BaseScript.Delay(1);

            API.SetPlayerModel(player, hash);
            API.SetModelAsNoLongerNeeded(hash);

            API.SetPedDefaultComponentVariation(API.GetPlayerPed(-1));

            API.RequestCollisionAtCoord(-468.547f, -1719.703f, 18.67876f);
            API.SetEntityCoordsNoOffset(playerPed, -468.547f, -1719.703f, 18.67876f, false, false, true);
            API.NetworkResurrectLocalPlayer(-468.547f, -1719.703f, 18.67876f, 270, true, true);
            API.ClearPedTasksImmediately(playerPed);
            API.RemoveAllPedWeapons(playerPed, true);
            API.ClearPlayerWantedLevel(player);

            API.ShutdownLoadingScreen();
            API.DoScreenFadeIn(500);

            if (!API.IsEntityVisible(playerPed))
                API.SetEntityVisible(playerPed, true, false);

            //var camera = API.CreateCamera(CreatorCamera, true);
            int camera = API.CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", -463.4458f, -1718.268f, 18.65963f, 0.0f,
                                                 0.0f, 100.0f, 90.0f * 0.4f, true, 0);
            //API.SetCamRot(camera, 0, 0, 0, 0);
            API.SetCamActive(camera, true);
            API.RenderScriptCams(true, false, 0, true, false);
            API.SetCamAffectsAiming(camera, false);

            //API.SetFollowPedCamViewMode(4);
            //API.EnableControlAction(playerPed, 0, true);
            //API.FreezePedCameraRotation(playerPed);
            API.DisableAllControlActions(0);
            Debug.WriteLine($"Entity visible: {API.IsEntityVisible(playerPed)}");

            if (API.IsEntityVisible(playerPed))
                API.SetEntityVisible(playerPed, false, false);
            Debug.WriteLine($"Entity visible: {API.IsEntityVisible(playerPed)}");
            isPlayerInCreator = true;
            API.DisplayRadar(false);
            CharacterCreatorUI.createUI();
        }

        [Tick]
        public async void HideWheelThisFrame(){
            if (isPlayerInCreator)
                API.BlockWeaponWheelThisFrame();
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

                API.NetworkSetEntityVisibleToNetwork(player, false);
            }
        }
    }
}