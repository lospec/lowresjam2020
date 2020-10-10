using System.Collections.Generic;
using System.Linq;
using Godot;
using HeroesGuild.guild_hall.chest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public static readonly List<Dictionary<int, string>> DefaultChestContent =
            new List<Dictionary<int, string>>();
        

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
                if (value is null)
                {
                    GD.PrintErr($"Value is not of type JArray and is instead null");
                    return;
                }
                else if (!(value is JArray))
                {
                    GD.PrintErr($"Value is not of type JArray and is instead {value.GetType().Name}");
                    return;
                }

                var jArray = (JArray) value;
                Inventory = jArray.Select(i => (string) i).ToList();
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
        [JsonProperty]
        public List<Dictionary<int, string>> ChestContent { get; set; } =
            DefaultChestContent;
        /*[JsonProperty]
        private object ChestItems
        {
            get => ChestContent;
            set
            {
                if (value is null)
                {
                    GD.PrintErr($"Value is not of type JArray and is instead null");
                    return;
                }
                else if (!(value is JArray))
                {
                    GD.PrintErr($"Value is not of type JArray and is instead {value.GetType().Name}");
                    return;
                }

                var jArray = (JArray) value;
                foreach (var e in jArray.Select(i => (string) i))
                {
                    GD.Print(e);
                }
            }
        }*/

        [JsonProperty]
        public int GuildLevel { get; set; } = 1;
        [JsonProperty]
        public int CoinsDeposited { get; set; } = 0;

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
                // I don't know why but when Set is used with a JArray it sets it to null
                switch (data.Key)
                {
                    case nameof(InventoryItems):
                        InventoryItems = data.Value;
                        continue;
                    case nameof(ChestContent):
                        var jArray = (JArray) data.Value;
                        ChestContent = jArray.ToObject<List<Dictionary<int, string>>>();
                        continue;
                }

                Set(data.Key, data.Value);
            }

            file.Close();
        }
    }
}