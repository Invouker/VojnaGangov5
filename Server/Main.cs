using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Server.Database.Entities.Player.PlayerInventory;
using Server.Services;
using Server.Testable;

namespace Server{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Main : BaseScript{
        public static Main Instance{ get; set; }

        public PlayerList PlayerList(){
            return Players;
        }

        public Main(){
            Trace.Log("Initializing server resource.");
            Instance = this;

            StreamerTest.Init();

            ServiceManager.PlayerService.Init();
            ServiceManager.CharacterCreatorService.Init();
            //AddEventHandler("playerLoaded", new Action<Player>(CharacterCreatorService.Loader)); // Load and switch between creator / spawn in world
            AddEventHandler("playerlist:list",
                            new Action<NetworkCallbackDelegate>(call => { // For Player List information
                                                                    call.Invoke(JsonConvert
                                                                                   .SerializeObject(ServiceManager
                                                                                       .PlayerService
                                                                                       .PlayerSlots));
                                                                }));

            AddEventHandler("playerlist:list:max",
                            new Action<NetworkCallbackDelegate>(call => { // For getting a max player count
                                                                    int maxPlayers =
                                                                        int.Parse(API.GetConvar("sv_maxclients",
                                                                            5.ToString()));
                                                                    string serverName =
                                                                        API.GetConvar("sv_hostname",
                                                                            "Error while loading");
                                                                    call.Invoke(maxPlayers, serverName);
                                                                }));

            AddEventHandler("player:get:inventory", new Action<string, NetworkCallbackDelegate>((player, call) => {
                Trace.Log("Calling a player:get:inventory");
                string json = Inventory.ConvertInventoryOfPlayerToJson(player);
                call.Invoke(json);
            }));
            
            new InventoryTest();
            CommandsTest.RegisterCommands(Players);
            
            Trace.Log("Resource is fully loaded!");
        }

        public void AddEventHandler(string handler, Delegate action){
            EventHandlers[handler] += action;
        }

        public static void sendMessage(string message){
            TriggerEvent("chat:addMessage", new{
                color = new[]{ 16, 43, 76 },
                args = new[]{ "[Server]", message }
            });
        }
    
    }
}