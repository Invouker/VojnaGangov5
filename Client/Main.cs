using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Client.Events;
using Client.Handlers;
using Client.Streamable;
using Client.UIHandlers;

namespace Client{
    public class Main : BaseScript{
        public static Main Instance{ get; set; }

        public Main(){
            Instance = this;
            TriggerServerEvent("player:join");

            AddEventHandler("player:load:data", new Action<int, int, int, int, int, int, int, int>(LoadPlayerData));
            AddEventHandler("streamer:createBlip",
                            new Action<string, float, float, float, int, int, int, int, float, bool, bool>(Streamer
                                        .CreateBlip));
            AddEventHandler("streamer:createMarker",
                            new Action<int, float, float, float, int, int, int, int, bool>(Streamer.CreateMarker));
            AddEventHandler("streamer:create3dText",
                            new Action<string, float, float, float, int, int, int, int>(Streamer.Create3dText));

            AddEventHandler("player:spawn:to:world",
                            new Action<short, float, float, float, float>(SpawnManager.TeleportToWorld));
            AddEventHandler("player:character:data", new Action<string>(SpawnManager.AssignCharacterData));
            AddEventHandler("player:spawn:to:creator", new Action(SpawnManager.TeleportToCreator));

            Tick += InteractStreamable.OnInteractTick;
            Tick += Hud.OnRender;
            Tick += InteractiveUI.Tick;
            Tick += KeyHandler.Tick;
            Tick += VehicleEvents.Tick;
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

        private void LoadPlayerData(int dimension, int hp, int maxHp, int armour, int maxArmour, int level, int xp,
            int walkingStyleInt){
            int playerPed = API.PlayerPedId();
            API.SetEntityHealth(playerPed, hp);
            API.SetEntityMaxHealth(playerPed, maxHp);
            API.SetPedArmour(playerPed, armour);
            API.SetPlayerMaxArmour(Player.Local.Handle, maxArmour);

            API.SetMaxHealthHudDisplay(maxHp);
            API.SetMaxArmourHudDisplay(maxArmour);
            string walkingStyle = Utils.AnimWalkingListIndex.ToArray()[walkingStyleInt];
            Utils.SetWalkingAnimToPed(walkingStyle);
            Var.XP = xp;
            Var.Level = level;
            Var.WalkingStyle = walkingStyleInt;
            Debug.WriteLine($"Load Player Data, dimension: {dimension}, Hp: {hp}, maxHp: {maxHp}, armour: {armour}, maxArmour: {maxArmour}, level: {level}, xp: {xp}, walkingStyle: {walkingStyle}");
        }
    }
}