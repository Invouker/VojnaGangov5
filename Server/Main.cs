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

            #region CharacterCreator Data

            EventHandlers["player:data:character:blend"] +=
                new Action<Player, int, int, int, int, float, float>(ServiceManager.CharacterCreatorService
                                                                        .ClientDataBlend);
            EventHandlers["player:data:character:facefeature"] +=
                new Action<Player, float, float, float, float, float, float, float, float, float, float, float>
                    (ServiceManager.CharacterCreatorService.ClientDataFaceFeature);
            EventHandlers["player:data:character:facefeature2"] +=
                new Action<Player, float, float, float, float, float, float, float, float, float>
                    (ServiceManager.CharacterCreatorService.ClientDataFaceFeature2);
            EventHandlers["player:data:character:drawable"] +=
                new Action<Player, int, int, int, int, int, int, int, int, int, int, int, int, int, int>(ServiceManager
                   .CharacterCreatorService.ClientDataDrawable);

            EventHandlers["player:data:character:save"] +=
                new Action<Player>(ServiceManager.CharacterCreatorService.SaveCharacterData);

            #endregion

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