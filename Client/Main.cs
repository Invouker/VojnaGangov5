using System;
using System.Threading.Tasks;
using CitizenFX.Core;
using Client.Events;
using Client.Handlers;
using Client.Menus;
using Client.Streamable;
using Client.Testable;
using Client.Utils;

namespace Client{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Main : BaseScript{
        public static Main Instance{ get; private set; }

        public Main(){
            Instance = this;

            Tick += InteractStreamable.OnInteractTick;
            Tick += HudRenderEvent.OnRender;
            Tick += InteractiveMenu.Tick;
            Tick += KeyHandler.Tick;
            Tick += VehicleEvents.Tick;
            Tick += PlayerDeadEvent.Tick;

            TriggerServerEvent("playerConnected");

            TriggerServerEvent("playerlist:list:max", new Action<int, string>((max, serverName) => {
                Var.MaxPlayers = max;
                Var.ServerName = serverName;
            }));

            TestClassEvents.Handle(); // For handle test class
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
    }
}