namespace Server.Services {
    internal static class ServiceManager {
        private static PlayerService _playerService;
        private static StreamerService _streamerService;
        private static CharacterCreatorService _characterCreatorService;

        public static PlayerService PlayerService => _playerService ??= new PlayerService();
        public static StreamerService StreamerService => _streamerService ??= new StreamerService();
        public static CharacterCreatorService CharacterCreatorService => _characterCreatorService ??= new CharacterCreatorService();
    }
}
