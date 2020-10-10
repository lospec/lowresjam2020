using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public struct ItemRecord : IDataRecord
    {
        [JsonProperty("type")]
        public string type;

        [JsonProperty("buy_value")]
        public int buyValue;

        [JsonProperty("sell_value")]
        public int sellValue;

        [JsonProperty("quick_damage")]
        public int quickDamage;

        [JsonProperty("quick_damage_type")]
        public string quickDamageType;

        [JsonProperty("quick_status_effect")]
        public string quickStatusEffect;

        [JsonProperty("quick_effect_chance")]
        public float quickEffectChance;

        [JsonProperty("heavy_damage")]
        public int heavyDamage;

        [JsonProperty("heavy_damage_type")]
        public string heavyDamageType;

        [JsonProperty("heavy_status_effect")]
        public string heavyStatusEffect;

        [JsonProperty("heavy_effect_chance")]
        public float heavyEffectChance;

        [JsonProperty("counter_damage")]
        public int counterDamage;

        [JsonProperty("counter_damage_type")]
        public string counterDamageType;

        [JsonProperty("counter_status_effect")]
        public string counterStatusEffect;

        [JsonProperty("counter_effect_chance")]
        public float counterEffectChance;

        [JsonProperty("health_added")]
        public int healthAdded;

        [JsonProperty("usable")]
        public bool usable;

        [JsonProperty("health_gained")]
        public int healthGained;
    }
}