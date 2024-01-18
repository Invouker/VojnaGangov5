using System.Threading.Tasks;

namespace Client.Streamable{
    public class InteractStreamable{
        private const short DistanceToInteract = 3;

        public static async Task OnInteractTick(){
            if (!API.IsControlJustPressed(0, Control.Pickup.GetHashCode())){ // 38	INPUT_PICKUP	"E" 	LB 
                await BaseScript.Delay(5000);
                return;
            }

            foreach (IStreamer streamer in Streamer.Streamed){
                if (!(streamer is Marker marker))
                    continue;

                Vector3 playerPos = new Vector3(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y,
                                                Game.PlayerPed.Position.Z);
                if (!(marker.GetMarkerPosition().DistanceToSquared(playerPos) < DistanceToInteract))
                    continue;

                if (API.IsPedInAnyVehicle(Game.PlayerPed.Handle, true) && !marker.AllowVehicleInteract)
                    return;

                int id = marker._id;
                EventDispatcher.Send("player:interact:marker", id);
                //BaseScript.TriggerEvent("player:interact:marker", id);
                await BaseScript.Delay(3000);
            }

            await BaseScript.Delay(3000);
        }
    }
}