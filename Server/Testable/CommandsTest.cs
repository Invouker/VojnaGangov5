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
                //ServiceManager.PlayerService.Players.TryGetValue(Utils.GetLicense(player), out VGPlayer vgPlayer);
                VGPlayer vgPlayer = PlayerService.GetVgPlayerByLicense(player.Name);
                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{ "[Server]", $"Player get: {vgPlayer.ToString()}" }
                });
            }), false);

            API.RegisterCommand("save", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                PlayerService.UpdatePlayer(player, Utils.GetLicense(player));
                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{ "[Server]", $"Player saved" }
                });
            }), false);

            API.RegisterCommand("mypos", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];


                player.TriggerEvent("chat:addMessage", new{
                    color = new[]{ 16, 43, 76 },
                    args = new[]{
                        "[Server]",
                        $"Position X: {player.Character.Position.X}, Y: {player.Character.Position.Y}, Z: {player.Character.Position.Z}, Head: {player.Character.Heading}"
                    }
                });
            }), false);

            API.RegisterCommand("test", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                player.TriggerEvent("test:rankup");
                PlayerService.SetMoney(player, PlayerService.MoneyType.Wallet, 32503536);
                PlayerService.SetMoney(player, PlayerService.MoneyType.Bank, 1503536);
                Debug.WriteLine("test:rankup");
            }), false);
            API.RegisterCommand("test2", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                player.TriggerEvent("test:rankup");
                PlayerService.TakeMoney(player, PlayerService.MoneyType.Wallet, 25300);
                PlayerService.AddMoney(player, PlayerService.MoneyType.Bank, 5700);
                Debug.WriteLine("test:rankup");
            }), false);

            API.RegisterCommand("test3", new Action<int, List<object>, string>((source, args, rawCommand) => {
                Player player = Players[source];
                PlayerService.GiveXP(player, 60);
                Debug.WriteLine($"XP: {PlayerService.GetXP(player)}, Level: {PlayerService.GetLevel(player)}");
                Debug.WriteLine($"XP: {PlayerService.GetXP(player)},XpToNextLevel: {PlayerService.GetReputationToLevel(PlayerService.GetLevel(player) + 1)} ,Level: {PlayerService.GetLevel(player)}");
            }), false);
        }
    }
}