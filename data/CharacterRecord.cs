using Newtonsoft.Json;

namespace HeroesGuild.Data
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