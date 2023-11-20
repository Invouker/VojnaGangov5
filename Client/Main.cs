using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Events;
using Client.Streamable;

namespace Client{
    public class Main : BaseScript{
        public static Main Instance{ get; set; }

        public Main(){
            Instance = this;

            TriggerServerEvent("player:join");
            EventHandlers["player:load:data"] +=
                new Action<long, long, int, int, int, int, int, int, int>(LoadPlayerData);

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
            //EventHandlers["test:rankup"] += new Action(Hud.Rank);

            Tick += InteractStreamable.OnInteractTick;
            Tick += Hud.OnRender;
            TriggerServerEvent("player:post_join");

            AddEventHandler("player:hud:update:money", new Action<int, int>(Hud.ChangeMoney));
            AddEventHandler("player:hud:update:show:rank", new Action<int>(Hud.ShowRankBar));
            AddEventHandler("player:hud:update:xp", new Action<int, int>(Hud.ChangeXp));

            TriggerServerEvent("playerlist:list:max", new Action<int, string>((max, serverName) => {
                Var.MaxPlayers = max;
                Var.ServerName = serverName;
            }));
        }

        public PlayerList GetPlayers(){
            return Players;
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

        public void AddEventHandler(string handler, Delegate action){
            EventHandlers[handler] += action;
        }

        private void LoadPlayerData(long money, long bankMoney, int dimension, int hp, int maxHp, int armour,
            int maxArmour, int level, int xp){
            int playerPed = API.PlayerPedId();
            API.SetEntityHealth(playerPed, hp);
            API.SetEntityMaxHealth(playerPed, maxHp);
            API.SetPedArmour(playerPed, armour);
            API.SetPlayerMaxArmour(Player.Local.Handle, maxArmour);

            API.SetMaxHealthHudDisplay(maxHp);
            API.SetMaxArmourHudDisplay(maxArmour);
            Var.XP = xp;
            Var.Level = level;
        }
    }
}