using System;
using CitizenFX.Core;

namespace Server.Services{
    internal static class ServiceManager{
        public static PlayerService PlayerService{ get; private set; }
        public static StreamerService StreamerService{ get; private set; }
        public static CharacterCreatorService CharacterCreatorService{ get; private set; }

        static ServiceManager(){
            PlayerService = new PlayerService();
            StreamerService = new StreamerService();
            CharacterCreatorService = new CharacterCreatorService();


            Main.Instance.AddEventHandler("playerDropped", new Action<Player, string>(PlayerService.OnPlayerDropped));
            Main.Instance.AddEventHandler("onResourceStop", new Action<string>(PlayerService.OnResourceStop));
            Main.Instance.AddEventHandler("player:data:character",
                                          new Action<Player, string>(CharacterCreatorService.SaveCharacterData));
            Main.Instance.AddEventHandler("player:spawn:to:world:server",
                                          new Action<Player, int>(CharacterCreatorService.LoadCharacterData));
        }
    }
}