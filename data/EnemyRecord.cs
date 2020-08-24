using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public struct EnemyRecord : IDataRecord
    {
        [JsonProperty("race")]
        public string Race { get; set; }

        [JsonProperty("max_health")]
        public int MaxHealth { get; set; }

        [JsonProperty("weakness")]
        public string Weakness { get; set; }

        [JsonProperty("resistance")]
        public string Resistance { get; set; }

        [JsonProperty("coin_drop_amount")]
        public int CoinDropAmount { get; set; }

        [JsonProperty("item_drop_1")]
        public string ItemDrop1 { get; set; }

        [JsonProperty("item_drop_1_chance")]
        public float ItemDrop1Chance { get; set; }

        [JsonProperty("item_drop_2")]
        public string ItemDrop2 { get; set; }

        [JsonProperty("item_drop_2_chance")]
        public float ItemDrop2Chance { get; set; }

        [JsonProperty("max_items_dropped")]
        public int MaxItemDropped { get; set; }

        [JsonProperty("attack_pool")]
        public string AttackPool { get; set; }

        [JsonProperty("quick_damage")]
        public int QuickDamage { get; set; }

        [JsonProperty("quick_damage_type")]
        public string QuickDamageType { get; set; }

        [JsonProperty("quick_status_effect")]
        public string QuickStatusEffect { get; set; }

        [JsonProperty("quick_effect_chance")]
        public float QuickEffectChance { get; set; }

        [JsonProperty("heavy_damage")]
        public int HeavyDamage { get; set; }

        [JsonProperty("heavy_damage_type")]
        public string HeavyDamageType { get; set; }

        [JsonProperty("heavy_status_effect")]
        public string HeavyStatusEffect { get; set; }

        [JsonProperty("heavy_effect_chance")]
        public float HeavyEffectChance { get; set; }

        [JsonProperty("counter_damage")]
        public int CounterDamage { get; set; }

        [JsonProperty("counter_damage_type")]
        public string CounterDamageType { get; set; }

        [JsonProperty("counter_status_effect")]
        public string CounterStatusEffect { get; set; }

        [JsonProperty("counter_effect_chance")]
        public float CounterEffectChance { get; set; }

        [JsonProperty("ai_type")]
        public string AiType { get; set; }

        [JsonProperty("move_speed")]
        public int MoveSpeed { get; set; }
    }
}