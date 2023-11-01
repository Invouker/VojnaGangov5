

namespace Server.Services {

	class ServiceManager {

		public static PlayerService playerService { get; private set; }

		public ServiceManager() {
			playerService = new PlayerService();
		}
	}
}
