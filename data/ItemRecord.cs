using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public struct ItemRecord : IDataRecord
    {
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("buy_value")]
        public int BuyValue;

        [JsonProperty("sell_value")]
        public int SellValue;

        [JsonProperty("quick_damage")]
        public int QuickDamage;

        [JsonProperty("quick_damage_type")]
        public string QuickDamageType;

        [JsonProperty("quick_status_effect")]
        public string QuickStatusEffect;

        [JsonProperty("quick_effect_chance")]
        public float QuickEffectChance;

        [JsonProperty("heavy_damage")]
        public int HeavyDamage;

        [JsonProperty("heavy_damage_type")]
        public string HeavyDamageType;

        [JsonProperty("heavy_status_effect")]
        public string HeavyStatusEffect;

        [JsonProperty("heavy_effect_chance")]
        public float HeavyEffectChance;

        [JsonProperty("counter_damage")]
        public int CounterDamage;

        [JsonProperty("counter_damage_type")]
        public string CounterDamageType;

        [JsonProperty("counter_status_effect")]
        public string CounterStatusEffect;

        [JsonProperty("counter_effect_chance")]
        public float CounterEffectChance;
    }
}