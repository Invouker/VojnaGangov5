using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;
using Server.Services;
using Server.Testable;

namespace Server{
    public class Main : BaseScript{
        public static Main Instance{ get; set; }

        public PlayerList PlayerList(){
            return Players;
        }

        public Main(){
            Instance = this;
            new ServiceManager();
            //["player:join"] += new Action<Player>(ServiceManager.PlayerService.PlayerJoin);
            EventHandlers["player:post_join"] += new Action<Player>(StreamerTest.PlayerPostJoin);
            //EventHandlers["playerDropped"] += new Action<Player, string>(ServiceManager.PlayerService.OnPlayerDropped);
            //EventHandlers["onResourceStop"] += new Action<string>(ServiceManager.PlayerService.OnResourceStop); //Todo: Uncomment this if playerlist and autosave will be ok
            EventHandlers["player:interact:marker"] += new Action<int>(StreamerTest.OnMarkerInteract);

            EventHandlers["player:data:character"] +=
                new Action<Player, string>(ServiceManager.CharacterCreatorService.SaveCharacterData);
            EventHandlers["player:spawn:to:world:server"] +=
                new Action<Player, int>(ServiceManager.CharacterCreatorService.LoadCharacterData);
            EventHandlers["player:post_join"] +=
                new Action<Player>(ServiceManager.CharacterCreatorService
                                                 .Loader); // Load and switch between creator / spawn in world


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