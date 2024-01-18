using Newtonsoft.Json;
using Server.Database.Entities.Player.PlayerInventory;
using Server.Services;
using Server.Testable;
using Server.Utils;

namespace Server{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Main : BaseScript{
        public static Main Instance{ get; set; }

        public PlayerList PlayerList(){
            return Players;
        }

        public Main(){
            //SnowflakeGenerator.Create(127);
            EventDispatcher.Initalize(Shared.FxInBound, Shared.FxOutBound, Shared.FxSignature, Shared.FxEncryption);
            
            Trace.Log("Initializing server resource.");
            Instance = this;
            
            ServiceManager.PlayerService.Init();
            ServiceManager.CharacterCreatorService.Init();
            StreamerTest.Init();
            
            //EventDispatcher.Mount("playerLoaded", new Action<Player>(CharacterCreatorService.Loader)); // Load and switch between creator / spawn in world
            EventDispatcher.Mount("playerlist:list",
                            new Action<NetworkCallbackDelegate>(call => { // For Player List information
                                                                    call.Invoke(JsonConvert
                                                                                   .SerializeObject(ServiceManager
                                                                                       .PlayerService
                                                                                       .PlayerSlots));
                                                                }));

            EventDispatcher.Mount("playerlist:list:max",
                            new Action<NetworkCallbackDelegate>(call => { // For getting a max player count
                                                                    int maxPlayers =
                                                                        int.Parse(API.GetConvar("sv_maxclients",
                                                                        5.ToString()));
                                                                    string serverName =
                                                                        API.GetConvar("sv_hostname",
                                                                            "Error while loading");
                                                                    call.Invoke(maxPlayers, serverName);
                                                                }));

            EventDispatcher.Mount("player:get:inventory", new Action<string, NetworkCallbackDelegate>((player, call) => {
                string json = Inventory.ConvertInventoryOfPlayerToJson(player);
                call.Invoke(json);
            }));
            
            //EventDispatcher.Mount("player:sound:playfrontend", new Action<string, string>(SoundEvent.PlayFrontendSound));
            
            new InventoryTest();
            CommandsTest.RegisterCommands(Players);
            
            Trace.Log("Resource is fully loaded!");
        }
        

       /* public void AddEventHandler(string handler, Delegate action){
            EventHandlers[handler] += action;
        }*/

        public static void sendMessage(string message){
            TriggerEvent("chat:addMessage", new{
                color = new[]{ 16, 43, 76 },
                args = new[]{ "[Server]", message }
            });
        }
    
    }
}