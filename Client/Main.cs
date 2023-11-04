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
            EventHandlers["streamer:createMarker"] += new Action<float, float, float, int>(Streamer.CreateMarker);
            EventHandlers["streamer:create3dText"] +=
                new Action<string, float, float, float, int, int, int, int>(Streamer.Create3dText);

            SpawnManager.SpawnPlayer();
            TriggerServerEvent("player:post_join");
        }

        [Tick]
        public async Task onTickRender(){
            Streamer.stream();
            await Task.FromResult(true);
        }

        private void LoadPlayerData(long money, long bankMoney, float x, float y, float z, int dimension, int hp,
            int maxHp, int armour, int maxArmour){
            API.StartPlayerTeleport(API.GetPlayerIndex(), x, y, z, 0, false, false, true);
            int playerPed = API.GetPlayerPed(-1);
            API.SetEntityHealth(playerPed, hp);
            API.SetEntityMaxHealth(playerPed, maxHp);
            API.SetPedArmour(playerPed, armour);
            API.SetPlayerMaxArmour(Game.Player.Handle, maxArmour);

            API.SetMaxHealthHudDisplay(maxHp);
            API.SetMaxArmourHudDisplay(maxArmour);
            Debug.WriteLine("Load Player Data!");
        }
    }
}