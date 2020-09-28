using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;

namespace HeroesGuild.data
{
    public class Data : Node
    {
        private const string ITEM_DATA_PATH = "res://data/item_data.json";
        private const string ENEMY_DATA_PATH = "res://data/enemy_data.json";
        private const string CHARACTER_DATA_PATH = "res://data/character_data.json";
		private const string MUSIC_DATA_PATH = "res://data/music_data.json";
		private const string SFX_DATA_PATH = "res://data/sfx_data.json";
		private int _maxSpeedStat = int.MinValue;

        private int _minSpeedStat = int.MaxValue;
        private bool _speedStatRead = false;
        public Dictionary<string, CharacterRecord> characterData =
            new Dictionary<string, CharacterRecord>();
        public Dictionary<string, EnemyRecord> enemyData =
            new Dictionary<string, EnemyRecord>();

        public Dictionary<string, ItemRecord> itemData =
            new Dictionary<string, ItemRecord>();

		public Dictionary<string, MusicRecord> musicData =
			new Dictionary<string, MusicRecord>();

		public Dictionary<string, SFXRecord> sfxData =
			new Dictionary<string, SFXRecord>();

        public override void _Ready()
        {
            itemData = ParseData<ItemRecord>(ITEM_DATA_PATH);
            enemyData = ParseData<EnemyRecord>(ENEMY_DATA_PATH);
            characterData = ParseData<CharacterRecord>(CHARACTER_DATA_PATH);
			musicData = ParseData<MusicRecord>(MUSIC_DATA_PATH);
			sfxData = ParseData<SFXRecord>(SFX_DATA_PATH);
			SetMinMaxSpeedStat();
        }

        public float GetLerpedSpeedStat(int speedStat, float minSpeed, float maxSpeed)
        {
            if (!_speedStatRead) SetMinMaxSpeedStat();

            return Mathf.Lerp(minSpeed, maxSpeed,
                (float) (speedStat - _minSpeedStat) / (_maxSpeedStat - _minSpeedStat));
        }

        private void SetMinMaxSpeedStat()
        {
            foreach (var speed in enemyData.Values.Select(enemyRecord =>
                enemyRecord.MoveSpeed))
            {
                if (speed < _minSpeedStat) _minSpeedStat = speed;

                if (speed > _maxSpeedStat) _maxSpeedStat = speed;

                _speedStatRead = true;
            }
        }

        private Dictionary<string, T> ParseData<T>(string path) where T : IDataRecord
        {
            var file = new File();
            if (!file.FileExists(path))
            {
                GD.PushError($"{path} file not found");
                return null;
            }

            file.Open(path, File.ModeFlags.Read);
            var json = file.GetAsText();
            var data = JsonConvert.DeserializeObject<Dictionary<string, T>>(json);
            return data;
        }
    }
}