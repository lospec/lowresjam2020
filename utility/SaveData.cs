using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Object = System.Object;

namespace HeroesGuild.utility
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SaveData : Node
    {
        public const string SAVE_DATA_PATH = "user://heroes_guild.save";

        public const int DEFAULT_COINS = 100;
        public const string DEFAULT_WEAPON = "Stick";
        public const string DEFAULT_ARMOR = "";
        public const int DEFAULT_HEALTH = 20;
        public static readonly Vector2 DefaultWorldPosition = new Vector2(2578, 1517);
        public static readonly List<string> DefaultInventory = new List<string>
            {"Stick", "Hotdog"};

        public static readonly List<List<string>> DefaultChestContent =
            new List<List<string>>();

        public const int DEFAULT_GUILD_LEVEL = 1;
        public const int DEFAULT_COINS_DEPOSITED = 0;


        public Vector2 WorldPosition { get; set; } = DefaultWorldPosition;
        [JsonProperty]
        private float WorldPositionX
        {
            get => WorldPosition.x;
            set => WorldPosition = new Vector2(value, WorldPosition.y);
        }

        [JsonProperty]
        private float WorldPositionY
        {
            get => WorldPosition.y;
            set => WorldPosition = new Vector2(WorldPosition.x, value);
        }

        [JsonProperty]
        public string CharacterName { get; set; } = "Jason";
        [JsonProperty]
        public int Coins { get; set; } = DEFAULT_COINS;

        public List<string> Inventory { get; set; } = DefaultInventory;
        [JsonProperty]
        private object InventoryItems
        {
            get => Inventory;
            set
            {
                var inventoryJArray = (JArray) value;
                Inventory = inventoryJArray.ToObject<List<string>>();
            }
        }

        [JsonProperty]
        public string EquippedWeapon { get; set; } = DEFAULT_WEAPON;
        [JsonProperty]
        public string EquippedArmor { get; set; } = DEFAULT_ARMOR;
        [JsonProperty]
        public int MaxHealth { get; set; } = DEFAULT_HEALTH;
        [JsonProperty]
        public int Health { get; set; } = DEFAULT_HEALTH;

        public List<List<string>> ChestContent { get; set; } =
            DefaultChestContent;
        [JsonProperty]
        private object ChestItems
        {
            get => ChestContent;
            set
            {
                var chestContentJArray = (JArray) value;
                ChestContent = chestContentJArray.ToObject<List<List<string>>>();
            }
        }

        [JsonProperty]
        public int GuildLevel { get; set; } = DEFAULT_GUILD_LEVEL;
        [JsonProperty]
        public int CoinsDeposited { get; set; } = DEFAULT_COINS_DEPOSITED;

        public override void _Ready()
        {
            var file = new File();
            if (file.FileExists(SAVE_DATA_PATH))
            {
                LoadGame();
            }
        }

        public void SaveGame()
        {
            var file = new File();
            file.Open(SAVE_DATA_PATH, File.ModeFlags.Write);
            file.StoreLine(JsonConvert.SerializeObject(this, Formatting.Indented));
            file.Close();
        }

        public void LoadGame()
        {
            var file = new File();
            if (!file.FileExists(SAVE_DATA_PATH))
            {
                GD.PrintErr("Attempted to load game data but no save data file found");
                return;
            }

            file.Open(SAVE_DATA_PATH, File.ModeFlags.Read);

            var saveData = file.GetAsText();
            var saveDataParsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(saveData);
            foreach (KeyValuePair<string, object> data in saveDataParsed)
            {
                // Reflection is being used instead of Godot's set method.
                // This is as a result of the set method being unable to be used with unmarshallable
                // managed types (i.e. types that cannot be converted to Godot's variant type)
                // such as JArray

                PropertyInfo prop = GetType().GetProperty(data.Key,
                    BindingFlags.NonPublic | BindingFlags.Public |
                    BindingFlags.Instance);
                prop.SetValue(this,
                    prop.PropertyType == typeof(Object)
                        ? data.Value
                        : Convert.ChangeType(data.Value, prop.PropertyType));
            }

            file.Close();
        }

        public void ResetSave()
        {
            WorldPosition = DefaultWorldPosition;
            Coins = DEFAULT_COINS;
            Inventory = DefaultInventory;
            EquippedWeapon = DEFAULT_WEAPON;
            EquippedArmor = DEFAULT_ARMOR;
            MaxHealth = DEFAULT_HEALTH;
            Health = DEFAULT_HEALTH;
            ChestContent = DefaultChestContent;
            GuildLevel = DEFAULT_GUILD_LEVEL;
            CoinsDeposited = DEFAULT_COINS_DEPOSITED;

            SaveGame();
        }
    }
}