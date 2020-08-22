using System.Collections.Generic;
using System.Linq;
using Godot;
using Newtonsoft.Json;
using File = Godot.File;


namespace HeroesGuild.data
{
    public class Data : Node
    {
        private const string ITEM_DATA_PATH = "res://data/item_data.json";
        private const string ENEMY_DATA_PATH = "res://data/enemy_data.json";
        private const string CHARACTER_DATA_PATH = "res://data/character_data.json";

        public Dictionary<string, ItemRecord> ItemData = new Dictionary<string, ItemRecord>();
        public Dictionary<string, EnemyRecord> EnemyData = new Dictionary<string, EnemyRecord>();
        public Dictionary<string, CharacterRecord> CharacterData = new Dictionary<string, CharacterRecord>();

        private int _minSpeedStat = int.MinValue;
        private int _maxSpeedStat = int.MaxValue;
        private bool _speedStatRead = false;

        public override void _Ready()
        {
            ItemData = ParseData<ItemRecord>(ITEM_DATA_PATH);
            EnemyData = ParseData<EnemyRecord>(ENEMY_DATA_PATH);
            CharacterData = ParseData<CharacterRecord>(CHARACTER_DATA_PATH);
            SetMinMaxSpeedStat();
        }

        public float GetLerpedSpeedStat(int speedStat, float minSpeed, float maxSpeed)
        {
            if (!_speedStatRead)
            {
                SetMinMaxSpeedStat();
            }

            return Mathf.Lerp(minSpeed, maxSpeed,
                (float) (speedStat - _minSpeedStat) / (_maxSpeedStat - _minSpeedStat));
        }

        private void SetMinMaxSpeedStat()
        {
            foreach (var speed in EnemyData.Values.Select(enemyRecord => enemyRecord.MoveSpeed))
            {
                if (speed < _minSpeedStat)
                {
                    _minSpeedStat = speed;
                }

                if (speed > _maxSpeedStat)
                {
                    _maxSpeedStat = speed;
                }

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