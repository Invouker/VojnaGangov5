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
        public static bool IsPlayerInCreator = false;
        public static int CameraCreator = -1;
        public static PositionOfCamera CameraPos = PositionOfCamera.Main;

        public enum PositionOfCamera{
            Left = 0,
            Main = 1,
            Right = 2
        }


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

            API.RequestCollisionAtCoord(112.6813f, -618.4352f, 206.0344f);
            API.SetEntityCoordsNoOffset(playerPed, 112.6813f, -618.4352f, 206.0344f, false, false, true);
            API.NetworkResurrectLocalPlayer(112.6813f, -618.4352f, 206.0344f, 229.6063f, true, true);

            API.ClearPedTasksImmediately(playerPed);
            API.RemoveAllPedWeapons(playerPed, true);
            API.ClearPlayerWantedLevel(player);

            API.ShutdownLoadingScreen();
            API.DoScreenFadeIn(500);

            if (!API.IsEntityVisible(playerPed))
                API.SetEntityVisible(playerPed, true, false);

            CameraCreator = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
            // Vector3 camOffset = API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), 1f, 0.3f, 0.65f); // left-side
            //Vector3 camOffset = API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), -1.8f, -0.5f, 0.65f); // right-side
            Vector3 camOffset =
                API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), -0.7f, 2.26f,
                                                     0.65f); // main (zoom 0.5 2,1 ) (unzoom 0.6 2.2 )
            Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
            API.SetCamCoord(CameraCreator, camOffset.X, camOffset.Y, camOffset.Z);
            API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
            API.SetCamActive(CameraCreator, true);
            API.RenderScriptCams(true, false, 1, true, true);


            if (API.IsEntityVisible(playerPed))
                API.SetEntityVisible(API.PlayerPedId(), false, false);

            IsPlayerInCreator = true;
            API.DisplayRadar(false);
            API.SetPedHeadBlendData(API.GetPlayerPed(-1), 0, 0, 0, 0, 0, 0, 0, 0f, 0f, false);
            CharacterCreatorUI.createUI();
        }

        private const Control ToLeft = Control.FrontendLb; // Q
        private const Control ToRight = Control.FrontendRb; // E

        public static async Task tick(){
            if (IsPlayerInCreator)
                API.BlockWeaponWheelThisFrame();

            if (Game.IsControlJustPressed(0, ToLeft) && CameraPos == PositionOfCamera.Main){
                Vector3 camOffset =
                    API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), 1f, 0.3f, 0.65f); // left-side
                Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
                API.SetCamCoord(CameraCreator, camOffset.X, camOffset.Y, camOffset.Z);
                API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
                //API.SetCamActive(CameraCreator, true);
                API.RenderScriptCams(true, true, 1000, true, true);

                CameraPos = PositionOfCamera.Right;
                await BaseScript.Delay(1000);
            }

            if (Game.IsControlJustPressed(0, ToRight) && CameraPos == PositionOfCamera.Main){
                Vector3 camOffset =
                    API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), -1.8f, -0.5f, 0.65f); // right-side
                Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
                API.SetCamCoord(CameraCreator, camOffset.X, camOffset.Y, camOffset.Z);
                API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
                //API.SetCamActive(CameraCreator, true);
                API.RenderScriptCams(true, true, 1000, true, true);

                CameraPos = PositionOfCamera.Left;
                await BaseScript.Delay(1000);
            }

            if ((Game.IsControlJustPressed(0, ToLeft) || Game.IsControlJustPressed(0, ToRight)) &&
                CameraPos != PositionOfCamera.Main){ // 108 - NUMPAD 4  ----  109 - NUMPAD 6
                //CameraCreator = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
                Vector3 camOffset =
                    API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), -0.7f, 2.26f,
                                                         0.65f); // main (zoom 0.5 2,1 ) (unzoom 0.6 2.2 )
                Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
                API.SetCamCoord(CameraCreator, camOffset.X, camOffset.Y, camOffset.Z);
                API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
                //API.SetCamActive(CameraCreator, true);
                API.RenderScriptCams(true, true, 1000, true, true);

                CameraPos = PositionOfCamera.Main;
                await BaseScript.Delay(1000);
            }
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