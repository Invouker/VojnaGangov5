using Client.Utils;

namespace Client.Events;

public class SoundEvent {

    public static void PlayFrontendSound(string soundSet, string soundName) {
        API.PlaySoundFrontend(-1, soundName, soundSet, true);
        Trace.Log($"SoundSet {soundSet}, soundName: {soundName}");
    }
    
}