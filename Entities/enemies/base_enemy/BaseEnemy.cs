using System.Threading.Tasks;
using Godot;
using HeroesGuild.AI;
using HeroesGuild.Data;
using HeroesGuild.Utility;

namespace HeroesGuild.Entities.Enemies.BaseEnemy
{
    public class BaseEnemy : BaseEntity.BaseEntity
    {
        private const string BattleSpritePath =
            "res://entities/enemies/sprites/{0}_battle.png";
        private const string OverworldSpritePath =
            "res://entities/enemies/sprites/{0}_overworld.png";
        private const string AIResourcePath = "res://ai/resources/{0}_behaviour.tres";

        private const float MAX_SPEED = 40;
        private const float MIN_SPEED = 5;

        [Signal] public delegate void StatsLoaded();

        [Signal] public delegate void Died(BaseEnemy enemy);

        [Export] public string enemyName;

        public EnemyRecord Stat { get; set; }
        public StateMachine stateMachine;
        public AtlasTexture battleTexture;

        public override void _Ready()
        {
            base._Ready();
            stateMachine = GetNode<StateMachine>("StateMachine");
            if (string.IsNullOrWhiteSpace(Stat.Race) &&
                !string.IsNullOrWhiteSpace(enemyName))
            {
                LoadEnemy(enemyName);
            }
        }

        public async void LoadEnemy(string enemyDataName)
        {
            var data = Autoload.Get<Data.Data>();
            if (!data.enemyData.ContainsKey(enemyDataName))
            {
                GD.PushError($"Enemy data for {enemyDataName} not found");
                return;
            }

            enemyName = enemyDataName;
            Stat = data.enemyData[enemyDataName];
            moveSpeed = data.GetLerpedSpeedStat(Stat.MoveSpeed, MIN_SPEED, MAX_SPEED);

            battleTexture = new AtlasTexture
            {
                Atlas = GD.Load<Texture>(string.Format(BattleSpritePath, enemyDataName))
            };

            if (sprite == null || stateMachine == null)
            {
                await ToSignal(this, "ready");
            }


            sprite.Texture =
                GD.Load<Texture>(string.Format(OverworldSpritePath, enemyDataName));

            stateMachine.behaviour =
                GD.Load<AI_Behaviour>(string.Format(AIResourcePath,
                    Stat.AiType.ToLower()));
            EmitSignal(nameof(StatsLoaded));
        }

        public bool IsInAllowedTile()
        {
            var isSpawnSafeArea2D = GetNode<Area2D>("IsSpawnSafeArea2D");
            if (isSpawnSafeArea2D.GetOverlappingBodies().Count == 0)
            {
                return true;
            }

            isSpawnSafeArea2D.QueueFree();
            return false;
        }

        public void Die()
        {
            EmitSignal(nameof(Died), this);
            QueueFree();
        }
    }
}