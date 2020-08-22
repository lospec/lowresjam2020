using System.Collections.Generic;
using Godot;

namespace HeroesGuild.Utility
{
    public class SaveData : Node
    {
        private const int DEFAULT_COINS = 100;
        private const string DEFAULT_WEAPON = "Stick";
        private const string DEFAULT_ARMOR = "";
        private const int DEFAULT_HEALTH = 20;
        private static readonly Vector2 DefaultWorldPosition = new Vector2(2578, 1517);
        private static readonly string[] DefaultInventory = {"Stick", "Hotdog"};

        public Vector2 WorldPosition { get; set; } = DefaultWorldPosition;
        public string CharacterName { get; set; } = "Jason";
        public int Coins { get; set; } = DEFAULT_COINS;
        public string[] Inventory { get; set; } = DefaultInventory;
        public string EquippedWeapon { get; set; } = DEFAULT_WEAPON;
        public string EquippedArmor { get; set; } = DEFAULT_ARMOR;
        public int MaxHealth { get; set; } = DEFAULT_HEALTH;
        public int Health { get; set; } = DEFAULT_HEALTH;
        public List<string> ChestContent { get; set; } = new List<string>();
        public int GuildLevel { get; set; } = 1;
        public int CoinsDeposited { get; set; } = 0;
    }
}