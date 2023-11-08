using System;
using CitizenFX.Core;
using Server.Services;
using Server.Testable;

namespace Server{
    public class Main : BaseScript{
        public Main(){
            new ServiceManager();
            EventHandlers["player:join"] += new Action<Player>(ServiceManager.PlayerService.PlayerJoin);
            EventHandlers["player:post_join"] += new Action<Player>(StreamerTest.PlayerPostJoin);
            EventHandlers["playerDropped"] += new Action<Player, string>(ServiceManager.PlayerService.OnPlayerDropped);
            EventHandlers["onResourceStop"] += new Action<string>(ServiceManager.PlayerService.OnResourceStop);
            EventHandlers["player:interact:marker"] += new Action<int>(StreamerTest.OnMarkerInteract);
            CommandsTest.RegisterCommands(Players);
        }


        public static void sendMessage(string message){
            TriggerEvent("chat:addMessage", new{
                color = new[]{ 16, 43, 76 },
                args = new[]{ "[Server]", message }
            });
        }
    }
}