using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public struct CharacterRecord : IDataRecord
    {
        [JsonProperty("race")]
        public string Race;

        [JsonProperty("guild_level")]
        public int GuildLevel;
        
        [JsonProperty("rarity")]
        public int Rarity;
    }
}