using System.Collections.Generic;
using Godot;
using Newtonsoft.Json;

namespace HeroesGuild.utility
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SaveData
    {
        public const int DefaultCoins = 100;
        public const string DefaultWeapon = "Stick";
        public const string DefaultArmor = "";
        public const int DefaultHealth = 20;

        public static readonly Vector2 DefaultWorldPosition = new Vector2(2578, 1517);

        public static readonly List<string> DefaultInventory = new List<string>
            {"Stick", "Hotdog"};

        public static readonly List<List<string>> DefaultChestContent =
            new List<List<string>>();

        public const int DefaultGuildLevel = 1;
        public const int DefaultCoinsDeposited = 0;
        [JsonProperty] public Vector2 WorldPosition { get; set; }
        [JsonProperty] public string CharacterName { get; set; }
        [JsonProperty] public int Coins { get; set; }
        [JsonProperty] public List<string> Inventory { get; set; }
        [JsonProperty] public string EquippedWeapon { get; set; }
        [JsonProperty] public string EquippedArmor { get; set; }
        [JsonProperty] public int MaxHealth { get; set; }
        [JsonProperty] public int Health { get; set; }
        [JsonProperty] public List<List<string>> ChestContent { get; set; }
        [JsonProperty] public int GuildLevel { get; set; }
        [JsonProperty] public int CoinsDeposited { get; set; }

        public SaveData()
        {
            CoinsDeposited = DefaultCoinsDeposited;
            WorldPosition = DefaultWorldPosition;
            CharacterName = "Jason";
            Coins = DefaultCoins;
            Inventory = DefaultInventory;
            EquippedWeapon = DefaultWeapon;
            EquippedArmor = DefaultArmor;
            MaxHealth = DefaultHealth;
            Health = DefaultHealth;
            ChestContent = DefaultChestContent;
            GuildLevel = DefaultGuildLevel;
        }

        public void ResetSave()
        {
            WorldPosition = DefaultWorldPosition;
            Coins = DefaultCoins;
            Inventory = DefaultInventory;
            EquippedWeapon = DefaultWeapon;
            EquippedArmor = DefaultArmor;
            MaxHealth = DefaultHealth;
            Health = DefaultHealth;
            ChestContent = DefaultChestContent;
            GuildLevel = DefaultGuildLevel;
            CoinsDeposited = DefaultCoinsDeposited;
        }
    }
}