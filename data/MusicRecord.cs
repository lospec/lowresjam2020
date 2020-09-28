using Newtonsoft.Json;
using static HeroesGuild.utility.AudioSystem;

namespace HeroesGuild.data
{
	public struct MusicRecord : IDataRecord
	{
		[JsonProperty("music_clip")]
		public Music musicClip;

		[JsonProperty("volume_db")]
		public float volumeDb;

		[JsonProperty("fade_in")]
		public bool fadeIn;

		[JsonProperty("playback_position")]
		public float playbackPosition;
	}
}