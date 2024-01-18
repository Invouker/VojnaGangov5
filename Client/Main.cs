using System.Threading.Tasks;
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
            
            EventDispatcher.Initalize(Shared.FxInBound, Shared.FxOutBound, Shared.FxSignature, Shared.FxEncryption);
            
            EventDispatcher.Mount("toClient", new Action<string, int>((from, num) => {
                Trace.Log($"sideType: {from}, num: {num}");
            }));
            
            EventDispatcher.Send("toServer", "fromClient", 2);
            Tick += InteractStreamable.OnInteractTick;
            Tick += HudRenderEvent.OnRender;
            Tick += InteractiveMenu.Tick;
            Tick += KeyHandler.Tick;
            Tick += VehicleEvents.Tick;
            Tick += PlayerDeadEvent.Tick;

            Trace.Log("playerConnected");
            EventDispatcher.Send("playerConnected", LocalPlayer.Handle);

            EventDispatcher.Mount("playerlist:list:max", new Action<int, string>((max, serverName) => {
                Var.MaxPlayers = max;
                Var.ServerName = serverName;
            }));

            TestClassEvents.Handle(); // For handle test class
            
            Trace.Log("Inventory was received from server.");
            EventDispatcher.Mount("player:inventory:send", new Action<string>(InteractiveMenu.PlayerInventory.LoadPlayerInventory));
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

        /*public void AddEventHandler(string handler, Delegate action){
            EventHandlers[handler] += action;
        }*/
    }
}