using Newtonsoft.Json;
using static HeroesGuild.utility.AudioSystem;

namespace HeroesGuild.data
{
    public struct SFXRecord : IDataRecord
    {
        [JsonProperty("sfx_clip")]
        public SFX sfxClip;

        [JsonProperty("min_volume_db")]
        public float minVolumeDb;

        [JsonProperty("max_volume_db")]
        public float maxVolumeDb;

        [JsonProperty("min_pitch")]
        public float minPitch;

        [JsonProperty("max_pitch")]
        public float maxPitch;
    }
}