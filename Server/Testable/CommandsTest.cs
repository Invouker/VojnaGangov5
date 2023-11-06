using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Server.Database.Entities;
using Server.Services;

namespace Server.Testable{
    public class CommandsTest{
        public static void RegisterCommands(PlayerList Players){
            API.RegisterCommand("get", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                ServiceManager.PlayerService.Players.TryGetValue(Utils.GetLicense(player), out VGPlayer vgPlayer);

                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{ "[Server]", $"Player get: {vgPlayer.ToString()}" }
                });
            }), false);

            API.RegisterCommand("save", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                ServiceManager.PlayerService.UpdatePlayer(player, Utils.GetLicense(player));
                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{ "[Server]", $"Player saved" }
                });
            }), false);

            API.RegisterCommand("load", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                ServiceManager.PlayerService.LoadPlayer(player, Utils.GetLicense(player));
                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{ "[Server]", $"Player saved" }
                });
            }), false);
        }
    }
}