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

        [JsonProperty(Required = Required.Always)]
        public string SaveDataVersion { get; set; }

        [JsonProperty] public Vector2 WorldPosition { get; set; }

        [JsonProperty, DefaultValue(DefaultCharacterName)]
        public string CharacterName { get; set; }

        [JsonProperty, DefaultValue(DefaultCoins)]
        public int Coins { get; set; }


        [JsonProperty, DefaultValue(DefaultWeapon)]
        public string EquippedWeapon { get; set; }

        [JsonProperty, DefaultValue(DefaultArmor)]
        public string EquippedArmor { get; set; }

        [JsonProperty, DefaultValue(DefaultHealth)]
        public int MaxHealth { get; set; }

        [JsonProperty, DefaultValue(DefaultHealth)]
        public int Health { get; set; }

        [JsonProperty, DefaultValue(DefaultGuildLevel)]
        public int GuildLevel { get; set; }

        [JsonProperty, DefaultValue(DefaultCoinsDeposited)]
        public int CoinsDeposited { get; set; }

        [JsonProperty("Inventory", Required = Required.Always)]
        private List<string> _inventory;

        [JsonProperty("ChestContent", Required = Required.Always)]
        private List<List<string>> _chestContent;


        public ref List<List<string>> ChestContent => ref _chestContent;
        public ref List<string> Inventory => ref _inventory;

        public static SaveData Default()
        {
            return new SaveData
            {
                SaveDataVersion = MostRecentSaveDataVersion,
                _chestContent = DefaultChestContent,
                WorldPosition = DefaultWorldPosition,
                CharacterName = DefaultCharacterName,
                Coins = DefaultCoins,
                EquippedWeapon = DefaultWeapon,
                EquippedArmor = DefaultArmor,
                MaxHealth = DefaultHealth,
                Health = DefaultHealth,
                GuildLevel = DefaultGuildLevel,
                CoinsDeposited = DefaultCoinsDeposited,
                _inventory = DefaultInventory
            };
        }
    }
}