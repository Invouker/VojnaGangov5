using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Server.Database.Entities;
using Server.Services;

namespace Server{
    public class Main : BaseScript{
        public Main(){
            new ServiceManager();
            EventHandlers["player:join"] += new Action<Player>(ServiceManager.PlayerService.PlayerJoin);
            EventHandlers["player:post_join"] += new Action<Player>(PlayerPostJoin);
            EventHandlers["playerDropped"] += new Action<Player, string>(ServiceManager.PlayerService.OnPlayerDropped);
            EventHandlers["onResourceStop"] += new Action<string>(ServiceManager.PlayerService.OnResourceStop);
            EventHandlers["player:interact:marker"] += new Action<int>(OnMarkerInteract);
            registerCommands();
        }

        private void OnMarkerInteract(int id){
            sendMessage($"OnMarker Interact with id: {id}");
            Debug.WriteLine($"OnMarker Interact with id: {id}");
        }

        /// <summary>
        /// Register a streamable objects after server startup only in PlayerPostJoin event.
        /// </summary>
        private void PlayerPostJoin(Player player){
            StreamerService.CreateBlip("Gang: Alt", -470.547f, -1719.703f, 18.67876f, 59, 255, 1, 2, 1f, false);
            StreamerService.Create3dText("test\n :*~bold~ huhu 1\n :)\n\nPress ~INPUT_PICKUP~ to interact.", -470.547f,
                                         -1719.703f, 18.67876f, 255, 30, 10, 0);
            StreamerService.CreateMarker(0, -470.547f, -1719.703f, 18.67876f, 1, 255, 255, 255, true);


            StreamerService.CreateBlip("Gang: Alt", -460.547f, -1719.703f, 18.67876f, 59, 255, 5, 2, 1f, false);
            StreamerService.Create3dText("test\n :*~bold~ huhu 1\n :)\n\nPress ~INPUT_PICKUP~ to interact.", -460.547f,
                                         -1719.703f, 18.67876f, 255, 30, 10, 0);
            StreamerService.CreateMarker(1, -460.547f, -1719.703f, 18.67876f, 1, 255, 0, 0, false);
        }

        private void registerCommands(){
            API.RegisterCommand("get", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                ServiceManager.PlayerService.Players.TryGetValue(player, out VGPlayer vgPlayer);

                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{ "[Server]", $"Player get: {vgPlayer.ToString()}" }
                });
            }), false);

            API.RegisterCommand("save", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                ServiceManager.PlayerService.UpdatePlayer(player);
                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{ "[Server]", $"Player saved" }
                });
            }), false);

            API.RegisterCommand("load", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                ServiceManager.PlayerService.LoadPlayer(player);
                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{ "[Server]", $"Player saved" }
                });
            }), false);
        }

        public static void sendMessage(string message){
            TriggerEvent("chat:addMessage", new{
                color = new[]{ 16, 43, 76 },
                args = new[]{ "[Server]", message }
            });
        }
    }
}