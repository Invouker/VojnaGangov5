using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
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
            Instance = this;


            AddEventHandler("player:post_join",
                            new Action<Player>(ServiceManager.CharacterCreatorService
                                                             .Loader)); // Load and switch between creator / spawn in world
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

            CommandsTest.RegisterCommands(Players);
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