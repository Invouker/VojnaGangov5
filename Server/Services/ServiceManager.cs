namespace Server.Services{
    class ServiceManager{
        public static PlayerService PlayerService{ get; private set; }
        public static StreamerService StreamerService{ get; private set; }

        public ServiceManager(){
            PlayerService = new PlayerService();
            StreamerService = new StreamerService();
        }
    }
}