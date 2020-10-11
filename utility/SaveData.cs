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
        public const int DefaultGuildLevel = 1;
        public const int DefaultCoinsDeposited = 0;

        public static readonly Vector2 DefaultWorldPosition = new Vector2(2578, 1517);

        public static readonly List<string> DefaultInventory = new List<string>
            {"Stick", "Hotdog"};

        public static readonly List<List<string>> DefaultChestContent =
            new List<List<string>>();


        [JsonProperty] public Vector2 WorldPosition { get; set; }

        [JsonProperty] public string CharacterName { get; set; }
        [JsonProperty] public int Coins { get; set; }


        [JsonProperty] public string EquippedWeapon { get; set; }
        [JsonProperty] public string EquippedArmor { get; set; }
        [JsonProperty] public int MaxHealth { get; set; }
        [JsonProperty] public int Health { get; set; }

        [JsonProperty] public int GuildLevel { get; set; }
        [JsonProperty] public int CoinsDeposited { get; set; }
        [JsonProperty("Inventory")] private List<string> _inventory;

        [JsonProperty("ChestContent")] private List<List<string>> _chestContent;


        public ref List<List<string>> ChestContent => ref _chestContent;
        public ref List<string> Inventory => ref _inventory;

        public static SaveData Default()
        {
            return new SaveData
            {
                _chestContent = DefaultChestContent,
                WorldPosition = DefaultWorldPosition,
                CharacterName = "Jason",
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