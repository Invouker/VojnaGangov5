using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Numerics;
using static CitizenFX.Core.Native.API;
using static System.Net.Mime.MediaTypeNames;

namespace Server
{
    public class Main : BaseScript
    {
        public Main()
        {
            EventHandlers["playerSpawned"] += new Action<int>(OnPlayerSpawned);
            RegisterCommand("pos", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var player = Players[source];
                if (player == null)
                {
                    return;
                }

                var position = player.Character.Position;
                string message = $"X: {position.X}, Y: {position.Y}, Z: {position.Z}";

                TriggerClientEvent(player, "chat:addMessage", new
                {
                    color = new[] { 255, 0, 0 },
                    args = new[] { "Server", message }
                });
            }), false);
            RegisterCommand("gw", new Action<int, List<object>, string>((source, args, raw) =>
            {
                var player = Players[source];
                TriggerClientEvent(player, "chat:addMessage", new
                {
                    color = new[] { 255, 0, 0 },
                    args = new[] { "Server", "gw command" }
                });
                TriggerClientEvent(player, "sendGiveWeapon");
            }), false);
        }

        private void OnPlayerSpawned(int source)
        {
            SendMessage("OnPlayerSpawned");
            TriggerClientEvent("csharp:setPlayerPosition", source, 1786.84, 3316.642, 41.52966, 180);
        }
        private void OnPlayerDied(int source, List<object> deathReason)
        {
            SendMessage("OnPlayerDied");
            TriggerClientEvent("csharp:setPlayerPosition", source, 1786.84, 3316.642, 41.52966, 180);
        }
        public void SendMessage(string text)
        {
            TriggerEvent("chat:addMessage", new
            {
                color = new[] { 250, 250, 250 },
                multiline = false,
                args = new[] { text }
            }); ;
        }
    }
}