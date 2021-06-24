using System.Collections.Generic;
using System.ComponentModel;
using Godot;
using Newtonsoft.Json;

namespace HeroesGuild.utility
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SaveData
    {
        public const string MostRecentSaveDataVersion = "0.1";
        public const int DefaultCoins = 100;
        public const string DefaultCharacterName = "Jason";
        public const string DefaultWeapon = "Stick";
        public const string DefaultArmor = "";
        public const int DefaultHealth = 20;
        public const int DefaultGuildLevel = 1;
        public const int DefaultCoinsDeposited = 0;

        public static readonly Vector2 DefaultWorldPosition = new Vector2(2578, 1517);

        public static readonly List<string> DefaultInventory = new List<string>
            {"Stick", "Hotdog"};

        public static readonly List<List<string>> DefaultChestContent =
            new List<List<string>>();
        
        public static readonly List<string> DefaultDeadCharacterNames =
            new List<string>();

        [JsonProperty(Required = Required.Always, Order = -999)]
        public string SaveDataVersion { get; set; } = MostRecentSaveDataVersion;

        [JsonProperty]
        public Vector2 WorldPosition { get; set; } = DefaultWorldPosition;

        [JsonProperty, DefaultValue(DefaultCharacterName)]
        public string CharacterName { get; set; } = DefaultCharacterName;

        [JsonProperty, DefaultValue(DefaultCoins)]
        public int Coins { get; set; } = DefaultCoins;

        [JsonProperty, DefaultValue(false)]
        public bool isDead { get; set; } = false;


        [JsonProperty, DefaultValue(DefaultWeapon)]
        public string EquippedWeapon { get; set; } = DefaultWeapon;

        [JsonProperty, DefaultValue(DefaultArmor)]
        public string EquippedArmor { get; set; } = DefaultArmor;

        [JsonProperty, DefaultValue(DefaultHealth)]
        public int MaxHealth { get; set; } = DefaultHealth;

        [JsonProperty, DefaultValue(DefaultHealth)]
        public int Health { get; set; } = DefaultHealth;

        [JsonProperty, DefaultValue(DefaultGuildLevel)]
        public int GuildLevel { get; set; } = DefaultGuildLevel;

        [JsonProperty, DefaultValue(DefaultCoinsDeposited)]
        public int CoinsDeposited { get; set; } = DefaultCoinsDeposited;

        [JsonProperty("Inventory", Required = Required.Always)]
        private List<string> _inventory = new List<string>(DefaultInventory);

        [JsonProperty("ChestContent", Required = Required.Always)]
        private List<List<string>> _chestContent = DefaultChestContent;

        [JsonProperty("DeadCharacterNames")]
        private List<string> _deadCharacterNames = DefaultDeadCharacterNames;


        public ref List<List<string>> ChestContent => ref _chestContent;
        public ref List<string> Inventory => ref _inventory;

        public ref List<string> DeadCharacterNames => ref _deadCharacterNames;
    }
}