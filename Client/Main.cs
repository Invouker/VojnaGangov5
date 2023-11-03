using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Events;

namespace Client{
    public class Main : BaseScript{
        public Main(){
            TriggerServerEvent("player:join");
            EventHandlers["player:load:data"] +=
                new Action<long, long, float, float, float, int, int, int, int, int>(LoadPlayerData);
            SpawnManager.SpawnPlayer();
        }


        private void LoadPlayerData(long money, long bankMoney, float x, float y, float z, int dimension, int hp,
            int max_hp, int armour, int max_armour){
            API.StartPlayerTeleport(API.GetPlayerIndex(), x, y, z, 0, false, false, true);
            int playerPed = API.GetPlayerPed(-1);
            API.SetEntityHealth(playerPed, hp);
            API.SetEntityMaxHealth(playerPed, max_hp);
            API.SetPedArmour(playerPed, armour);
            API.SetPlayerMaxArmour(Game.Player.Handle, max_armour);

            API.SetMaxHealthHudDisplay(max_hp);
            API.SetMaxArmourHudDisplay(max_armour);
            Debug.WriteLine("Load Player Data!");
        }
    }
}