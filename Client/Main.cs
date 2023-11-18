using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Events;
using Client.Streamable;

namespace Client{
    public class Main : BaseScript{
        public Main(){
            TriggerServerEvent("player:join");
            EventHandlers["player:load:data"] +=
                new Action<long, long, float, float, float, int, int, int, int, int>(LoadPlayerData);

            EventHandlers["streamer:createBlip"] +=
                new Action<string, float, float, float, int, int, int, int, float, bool>(Streamer.CreateBlip);
            EventHandlers["streamer:createMarker"] +=
                new Action<int, float, float, float, int, int, int, int, bool>(Streamer.CreateMarker);
            EventHandlers["streamer:create3dText"] +=
                new Action<string, float, float, float, int, int, int, int>(Streamer.Create3dText);

            EventHandlers["player:spawn:to:world"] +=
                new Action<short, float, float, float, float>(SpawnManager.TeleportToWorld);
            EventHandlers["player:character:data"] += new Action<string>(SpawnManager.AssignCharacterData);
            EventHandlers["player:spawn:to:creator"] += new Action(SpawnManager.TeleportToCreator);

            Tick += InteractStreamable.OnInteractTick;

            Tick += async () => {
                Vector3 camPos = API.GetGameplayCamCoord();
                API.SetTextFont(1);
                API.SetTextProportional(true);
                API.SetTextScale(0.0f, 0.3f);
                API.SetTextColour(128, 128, 128, 255);
                API.SetTextDropshadow(0, 0, 0, 0, 255);
                API.SetTextEdge(1, 0, 0, 0, 150);

                API.SetTextOutline();
                API.SetTextEntry("String");
                API.AddTextComponentString($"X: {camPos.X}, Y: {camPos.Y}, X: {camPos.Z}");
                API.DrawText(20, 20);
                await Delay(5000);
                //await Task.FromResult(true);
                Debug.WriteLine($"X: {camPos.X}, Y: {camPos.Y}, X: {camPos.Z}");
            };
            TriggerServerEvent("player:post_join");
        }

        [Tick]
        public async Task onSpawnManagerTick(){
            await SpawnManager.CreatorTick();
            await Task.FromResult(true);
        }

        [Tick]
        public async Task onTickRender(){
            Streamer.stream();
            await Task.FromResult(true);
        }

        [Tick]
        public async Task OnTickInteract(){
            await InteractStreamable.OnInteractTick();
            await Task.FromResult(true);
        }

        private void LoadPlayerData(long money, long bankMoney, float x, float y, float z, int dimension, int hp,
            int maxHp, int armour, int maxArmour){
            int playerPed = API.PlayerPedId();
            API.SetEntityHealth(playerPed, hp);
            API.SetEntityMaxHealth(playerPed, maxHp);
            API.SetPedArmour(playerPed, armour);
            API.SetPlayerMaxArmour(Player.Local.Handle, maxArmour);

            API.SetMaxHealthHudDisplay(maxHp);
            API.SetMaxArmourHudDisplay(maxArmour);
        }
    }
}