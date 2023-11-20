using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Entities;
using Client.UIHandlers;
using ScaleformUI.Menu;
using ScaleformUI.Scaleforms;

namespace Client.Events{
    public static class SpawnManager{
        private static bool IsPlayerInCreator;
        private static int CameraCreator = -1;
        private static PositionOfCamera CameraPos = PositionOfCamera.Main;

        private const Control ToLeft = Control.FrontendLb; // Q
        private const Control ToRight = Control.FrontendRb; // E
        private const Control UnZoom = Control.CharacterWheel; // L.ALT

        private enum PositionOfCamera{
            Left = 0,
            Main = 1,
            Right = 2,
            Unzoom = 3
        }

        public static async void TeleportToCreator(){
            API.DoScreenFadeOut(500);

            while (!API.IsScreenFadedOut())
                await BaseScript.Delay(1);

            var player = Player.Local.Handle;
            var playerPed = API.GetPlayerPed(-1);

            uint hash = (uint)API.GetHashKey("mp_m_freemode_01"); // mp_f_freemode_01
            API.RequestModel(hash);

            while (!API.HasModelLoaded(hash))
                await BaseScript.Delay(1);

            API.SetPlayerModel(player, hash);
            API.SetModelAsNoLongerNeeded(hash);

            API.SetPedDefaultComponentVariation(API.GetPlayerPed(-1));


            API.RequestCollisionAtCoord(113.0374f, -618.356f, 206.0344f);
            API.SetEntityCoordsNoOffset(playerPed, 113.0374f, -618.356f, 206.0344f, false, false, true);
            API.NetworkResurrectLocalPlayer(113.0374f, -618.356f, 206.0344f, 229.6063f, true, true);

            API.ClearPedTasksImmediately(playerPed);
            API.RemoveAllPedWeapons(playerPed, true);
            API.ClearPlayerWantedLevel(player);

            API.ShutdownLoadingScreen();
            API.DoScreenFadeIn(500);

            while (!API.IsScreenFadedIn())
                await BaseScript.Delay(1);

            if (!API.IsEntityVisible(playerPed))
                API.SetEntityVisible(playerPed, true, false);

            CameraCreator = API.CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
            // Vector3 camOffset = API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), 1f, 0.3f, 0.65f); // left-side
            //Vector3 camOffset = API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), -1.8f, -0.5f, 0.65f); // right-side

            //Vector3 camOffset = API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), -0.7f, 2.26f, 0.65f); // main (zoom 0.5 2,1 ) (unzoom 0.6 2.2 )
            Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
            API.SetCamCoord(CameraCreator, 114.3749f, -619.0679f, 206.64f); //center
            //API.SetCamCoord(CameraCreator, camOffset.X, camOffset.Y, camOffset.Z);
            API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
            API.SetCamActive(CameraCreator, true);
            API.RenderScriptCams(true, false, 1, true, true);

            if (API.IsEntityVisible(playerPed))
                API.SetEntityVisible(API.PlayerPedId(), false, false);

            IsPlayerInCreator = true;
            API.DisplayRadar(false);
            API.SetPedHeadBlendData(API.GetPlayerPed(-1), 0, 0, 0, 0, 0, 0, 0, 0f, 0f, false);
            UIMenu menu = CharacterCreatorUI.SelectSexUI();
            menu.Visible = true;

            API.SetPedCanHeadIk(Game.Player.Character.Handle, true);
            API.TaskStandStill(Game.Player.Character.Handle, int.MaxValue);
            Game.Player.Character.IsPositionFrozen = true;
            Game.Player.Character.IsInvincible = true;
        }

        public static async void TeleportToWorld(short sex, float posX, float posY, float posZ, float heading){
            IsPlayerInCreator = false;
            API.DoScreenFadeOut(600);

            await BaseScript.Delay(2000);
            int player = Game.Player.Handle;

            while (!API.HasPlayerTeleportFinished(player))
                await BaseScript.Delay(1);


            API.ShutdownLoadingScreen();

            uint hash = (uint)API.GetHashKey($"mp_{(sex == 0 ? "m" : "f")}_freemode_01");
            API.RequestModel(hash);

            while (!API.HasModelLoaded(hash))
                await BaseScript.Delay(1);

            API.SetPlayerModel(player, hash);
            var playerPed = API.PlayerPedId();
            API.SetPedDefaultComponentVariation(playerPed);
            API.SetModelAsNoLongerNeeded(hash);

            //-2246.927f, 269.0242f, 174.6095f
            API.RequestCollisionAtCoord(posX, posY, posZ);
            API.SetEntityCoordsNoOffset(playerPed, posX, posY, posZ, false, false, true);
            API.NetworkResurrectLocalPlayer(posX, posY, posZ, heading, true, false);
            API.ClearPedTasksImmediately(playerPed);

            API.ClearPedTasksImmediately(playerPed);
            API.RemoveAllPedWeapons(playerPed, true);
            API.ClearPlayerWantedLevel(player);

            API.StartPlayerTeleport(player, posX, posY, posZ, heading, false, false, false);

            BaseScript.TriggerServerEvent("player:spawn:to:world:server", playerPed);
            API.DoScreenFadeIn(1000);
            while (!API.IsScreenFadedIn())
                await BaseScript.Delay(1);

            API.DisplayRadar(true);
            API.RenderScriptCams(false, false, 1, true, true);

            API.SetPedCanHeadIk(Game.Player.Character.Handle, false);
            API.TaskStandStill(Game.Player.Character.Handle, 0);
            Game.Player.Character.IsPositionFrozen = false;
            Game.Player.Character.IsInvincible = false;

            Var.HideAllHud = false;
        }


        public static async Task CreatorTick(){
            if (IsPlayerInCreator){
                API.BlockWeaponWheelThisFrame();
                //API.DisableAllControlActions(0);

                InstructionalButtonsScaleform buttons = ScaleformUI.Main.InstructionalButtons;
                List<InstructionalButton> button = new List<InstructionalButton>{
                    new InstructionalButton(new List<Control>{ ToLeft, ToRight }, "Rotate Right/Left"),
                    new InstructionalButton(Control.FrontendRdown, "Select"),
                    new InstructionalButton(UnZoom, "Change zoom")
                };
                buttons.SetInstructionalButtons(button);

                if (Game.IsControlJustPressed(0, UnZoom) && CameraPos == PositionOfCamera.Main){
                    API.SetCamCoord(CameraCreator, 115.7208f, -620.0972f, 206.84f); //left
                    API.SetCamFov(CameraCreator, 70f);
                    Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
                    API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
                    API.RenderScriptCams(true, true, 1000, true, true);

                    CameraPos = PositionOfCamera.Unzoom;
                    await BaseScript.Delay(1000);
                }

                if (Game.IsControlJustPressed(0, ToRight) && CameraPos == PositionOfCamera.Main){
                    //Vector3 camOffset = API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), 1f, 0.3f, 0.65f); // left-side
                    //API.SetCamCoord(CameraCreator, camOffset.X, camOffset.Y, camOffset.Z);

                    API.SetCamCoord(CameraCreator, 114.0502f, -616.6601f, 206.64f); //left
                    Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
                    API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
                    API.RenderScriptCams(true, true, 1000, true, true);

                    CameraPos = PositionOfCamera.Right;
                    await BaseScript.Delay(1000);
                }

                if (Game.IsControlJustPressed(0, ToLeft) && CameraPos == PositionOfCamera.Main){
                    //Vector3 camOffset = API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), -1.8f, -0.5f, 0.65f); // right-side
                    //API.SetCamCoord(CameraCreator, camOffset.X, camOffset.Y, camOffset.Z);
                    API.SetCamCoord(CameraCreator, 112.0204f, -619.8864f, 206.64f); //right
                    Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
                    API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
                    //API.SetCamActive(CameraCreator, true);
                    API.RenderScriptCams(true, true, 1000, true, true);

                    CameraPos = PositionOfCamera.Left;
                    await BaseScript.Delay(1000);
                }

                if ((Game.IsControlJustPressed(0, ToLeft) || Game.IsControlJustPressed(0, ToRight) ||
                     Game.IsControlJustPressed(0, UnZoom)) &&
                    CameraPos != PositionOfCamera.Main){ // 108 - NUMPAD 4  ----  109 - NUMPAD 6
                    //Vector3 camOffset = API.GetOffsetFromEntityInWorldCoords(API.PlayerPedId(), -0.7f, 2.26f, 0.65f); // main (zoom 0.5 2,1 ) (unzoom 0.6 2.2 )
                    //API.SetCamCoord(CameraCreator, camOffset.X, camOffset.Y, camOffset.Z);
                    API.SetCamCoord(CameraCreator, 114.3749f, -619.0679f, 206.64f); //center
                    Vector3 playerPosition = API.GetEntityCoords(API.PlayerPedId(), true);
                    API.PointCamAtCoord(CameraCreator, playerPosition.X, playerPosition.Y, playerPosition.Z + 0.65f);
                    //API.SetCamActive(CameraCreator, true);
                    API.RenderScriptCams(true, true, 1000, true, true);

                    CameraPos = PositionOfCamera.Main;
                    await BaseScript.Delay(1000);
                }
            }
        }

        private static void FreezePlayer2(int player, bool freeze){
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

        public static void AssignCharacterData(string data){
            int playerPed = API.PlayerPedId();
            CharacterCreatorData character = CharacterCreatorData.DeserializeFromJson(data);
            API.SetPedHeadBlendData(playerPed, character.Mother, character.Father, 0,
                                    character.Mother, character.Father, 0,
                                    character.ParentFaceShapePercent, character.ParentSkinTonePercent, 0, true);

            API.SetPedFaceFeature(playerPed, 0, character.NoseWidth);
            API.SetPedFaceFeature(playerPed, 1, character.NosePeak);
            API.SetPedFaceFeature(playerPed, 2, character.NoseLength);
            API.SetPedFaceFeature(playerPed, 3, character.NoseBoneCurvness);
            API.SetPedFaceFeature(playerPed, 4, character.NoseTip);
            API.SetPedFaceFeature(playerPed, 5, character.NoseBoneTwist);
            API.SetPedFaceFeature(playerPed, 6, character.Eyebrow);
            API.SetPedFaceFeature(playerPed, 7, character.Eyebrow2);
            API.SetPedFaceFeature(playerPed, 8, character.CheekBones);
            API.SetPedFaceFeature(playerPed, 9, character.CheekSidewaysBoneSize);
            API.SetPedFaceFeature(playerPed, 10, character.CheekBonesWidth);
            API.SetPedFaceFeature(playerPed, 11, character.EyeOpening);
            API.SetPedFaceFeature(playerPed, 12, character.LipThickness);
            API.SetPedFaceFeature(playerPed, 13, character.JawBoneWidth);
            API.SetPedFaceFeature(playerPed, 14, character.JawBoneShape);
            API.SetPedFaceFeature(playerPed, 15, character.ChinBone);
            API.SetPedFaceFeature(playerPed, 16, character.ChinBoneLength);
            API.SetPedFaceFeature(playerPed, 17, character.ChinBoneShape);
            API.SetPedFaceFeature(playerPed, 18, character.ChinHole);
            API.SetPedFaceFeature(playerPed, 19, character.NeckThickness);


            API.SetPedHeadOverlay(playerPed, 1, character.FacialHair, character.FacialHairOpacity);
            API.SetPedHeadOverlayColor(playerPed, 1, 1, character.FacialHairColor, character.FacialHairColor);
            API.SetPedHeadOverlay(playerPed, 2, character.Eyebrows, character.EyebrowsOpacity);
            API.SetPedHeadOverlayColor(playerPed, 2, 1, character.EyebrowsColor, character.EyebrowsColor);
            API.SetPedHeadOverlay(playerPed, 3, character.Ageing, character.AgeingOpacity);
            API.SetPedHeadOverlayColor(playerPed, 3, 1, character.AgeingColor, character.AgeingColor);
            API.SetPedHeadOverlay(playerPed, 4, character.Makeup, character.MakeupOpacity);
            API.SetPedHeadOverlayColor(playerPed, 4, 1, character.MakeupColor, character.MakeupColor);
            API.SetPedHeadOverlay(playerPed, 6, character.Complexion, character.ComplexionOpacity);
            API.SetPedHeadOverlayColor(playerPed, 6, 1, character.ComplexionColor, character.ComplexionColor);
            API.SetPedHeadOverlay(playerPed, 7, character.SunDamage, character.SunDamageOpacity);
            API.SetPedHeadOverlayColor(playerPed, 7, 1, character.SunDamageColor, character.SunDamageColor);
            API.SetPedHeadOverlay(playerPed, 8, character.Lipstick, character.LipstickOpacity);
            API.SetPedHeadOverlayColor(playerPed, 8, 1, character.LipstickColor, character.LipstickColor);
            API.SetPedHeadOverlay(playerPed, 9, character.MolesFreckles, character.MolesFrecklesOpacity);
            API.SetPedHeadOverlayColor(playerPed, 9, 1, character.MolesFrecklesColor, character.MolesFrecklesColor);
            API.SetPedHeadOverlay(playerPed, 11, character.BodyBlemishes, character.BodyBlemishesOpacity);
            API.SetPedHeadOverlayColor(playerPed, 11, 1, character.BodyBlemishesColor, character.BodyBlemishesColor);

            API.SetPedComponentVariation(playerPed, 2, 2, 0, 2);
            API.SetPedHairColor(playerPed, character.HairColor, character.HairColor);
            API.SetPedComponentVariation(playerPed, 3, character.Torso, character.TorsoTexture, 0);
            API.SetPedComponentVariation(playerPed, 4, character.Legs, character.LegsTexture, 0);
            API.SetPedComponentVariation(playerPed, 6, character.Foot, character.FootTexture, 0);
            API.SetPedComponentVariation(playerPed, 7, character.Accesories, character.AccesoriesTexture, 0);
            API.SetPedComponentVariation(playerPed, 8, character.Scarfs, character.ScarfsTexture, 0);
            API.SetPedComponentVariation(playerPed, 11, character.Torso2, character.Torso2Texture, 0);
        }
    }
}