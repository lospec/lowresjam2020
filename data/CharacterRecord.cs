using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public struct CharacterRecord : IDataRecord
    {
        [JsonProperty("race")]
        public string race;

        [JsonProperty("guild_level")]
        public int guildLevel;

        [JsonProperty("rarity")]
        public int rarity;
    }
}