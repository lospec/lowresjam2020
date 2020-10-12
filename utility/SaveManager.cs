using Godot;
using Newtonsoft.Json;

namespace HeroesGuild.utility
{
    public static class SaveManager
    {
        private const string SaveDataPath = "user://heroes_guild.save";
        public static SaveData SaveData { get; private set; } = SaveData.Default();

        private static readonly JsonSerializerSettings _jsonSerializerSettings =
            new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                MissingMemberHandling = MissingMemberHandling.Error
            };

        static SaveManager()
        {
            LoadGame();
        }

        public static void LoadGame()
        {
            var file = new File();
            if (!file.FileExists(SaveDataPath))
            {
                GD.PrintErr("Attempted to load game data but no save data file found");
                return;
            }


            file.Open(SaveDataPath, File.ModeFlags.Read);
            var saveDataText = file.GetAsText();
            file.Close();

            SaveData data;
            try
            {
                data = JsonConvert.DeserializeObject<SaveData>(saveDataText,
                    _jsonSerializerSettings);
            }
            catch (JsonReaderException ex)
            {
                GD.PrintErr("Failed to read Save File - Save file is most likely not in the correct JSON format");
                GD.PrintErr($"Caught the following exception:\n{ex}");
                GD.Print("Will now load default save data");
                SaveData = SaveData.Default();
                return;
            }
            catch (JsonSerializationException ex)
            {
                GD.PrintErr("Failed to Deserialize Save File - Save file may be for an unsupported version of the game");
                GD.PrintErr($"Caught the following exception:\n{ex}");
                GD.Print("Will now load default save data");
                SaveData = SaveData.Default();
                return;
            }

            if (data == null)
            {
                GD.Print("Save file is corrupted");
                GD.PrintErr("Failed to Deserialize Save File");
                return;
            }

            SaveData = data;
        }


        public static void SaveGame()
        {
            var file = new File();
            file.Open(SaveDataPath, File.ModeFlags.Write);
            GD.Print($"Save to {file.GetPathAbsolute()}");
            file.StoreString(JsonConvert.SerializeObject(SaveData, Formatting.Indented, _jsonSerializerSettings));
            file.Close();
        }

        public static void ResetSave()
        {
            SaveData = SaveData.Default();
            SaveGame();
        }
    }
}