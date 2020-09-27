using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public struct ItemRecord : IDataRecord, ICombatStat
    {
        [JsonProperty("type")]
        public string Type {get; set;}

        [JsonProperty("buy_value")]
        public int BuyValue {get; set;}

        [JsonProperty("sell_value")]
        public int SellValue {get; set;}

        [JsonProperty("quick_damage")]
        public int QuickDamage {get; set;}

        [JsonProperty("quick_damage_type")]
        public string QuickDamageType {get; set;}

        [JsonProperty("quick_status_effect")]
        public string QuickStatusEffect {get; set;}

        [JsonProperty("quick_effect_chance")]
        public float QuickEffectChance {get; set;}

        [JsonProperty("heavy_damage")]
        public int HeavyDamage {get; set;}

        [JsonProperty("heavy_damage_type")]
        public string HeavyDamageType {get; set;}

        [JsonProperty("heavy_status_effect")]
        public string HeavyStatusEffect {get; set;}

        [JsonProperty("heavy_effect_chance")]
        public float HeavyEffectChance {get; set;}

        [JsonProperty("counter_damage")]
        public int CounterDamage {get; set;}

        [JsonProperty("counter_damage_type")]
        public string CounterDamageType {get; set;}

        [JsonProperty("counter_status_effect")]
        public string CounterStatusEffect {get; set;}

        [JsonProperty("counter_effect_chance")]
        public float CounterEffectChance {get; set;}

        [JsonProperty("health_added")]
        public int HealthAdded {get; set;}

        [JsonProperty("usable")]
        public bool Usable {get; set;}

        [JsonProperty("health_gained")]
        public int HealthGained {get; set;}
    }
}