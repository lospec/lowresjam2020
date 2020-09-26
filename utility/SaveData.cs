using System.Collections.Generic;
using Godot;

namespace HeroesGuild.utility
{
    public class SaveData : Node
    {
        public const int DEFAULT_COINS = 100;
        public const string DEFAULT_WEAPON = "Stick";
        public const string DEFAULT_ARMOR = "";
        public const int DEFAULT_HEALTH = 20;
        public static readonly Vector2 DefaultWorldPosition = new Vector2(2578, 1517);
        public static readonly List<string> DefaultInventory = new List<string>
            {"Stick", "Hotdog"};

        public Vector2 WorldPosition { get; set; } = DefaultWorldPosition;
        public string CharacterName { get; set; } = "Jason";
        public int Coins { get; set; } = DEFAULT_COINS;
        public List<string> Inventory { get; set; } = DefaultInventory;
        public string EquippedWeapon { get; set; } = DEFAULT_WEAPON;
        public string EquippedArmor { get; set; } = DEFAULT_ARMOR;
        public int MaxHealth { get; set; } = DEFAULT_HEALTH;
        public int Health { get; set; } = DEFAULT_HEALTH;
        public List<Dictionary<int, string>> ChestContent { get; set; } =
            new List<Dictionary<int, string>>();
        public int GuildLevel { get; set; } = 1;
        public int CoinsDeposited { get; set; } = 0;
    }
}