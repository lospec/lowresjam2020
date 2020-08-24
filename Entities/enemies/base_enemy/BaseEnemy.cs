using Godot;
using HeroesGuild.AI;
using HeroesGuild.data;
using HeroesGuild.Utility;

namespace HeroesGuild.Entities.enemies.base_enemy
{
    public class BaseEnemy : BaseEntity.BaseEntity
    {
        private const float MAX_SPEED = 40;
        private const float MIN_SPEED = 5;

        [Signal] public delegate void StatsLoaded();

        [Signal] public delegate void Died(BaseEnemy enemy);

        [Export] public string EnemyName;

        public EnemyRecord Stat { get; set; }
        public StateMachine stateMachine;
        public AtlasTexture battleTexture;

        public override void _Ready()
        {
            stateMachine = GetNode<StateMachine>("StateMachine");
            if (string.IsNullOrWhiteSpace(Stat.Race) && !string.IsNullOrWhiteSpace(EnemyName))
            {
                LoadEnemy(EnemyName);
                base._Ready();
            }
        }

        public async void LoadEnemy(string enemyDataName)
        {
            var data = Singleton.Get<Data>(this);
            if (!data.EnemyData.ContainsKey(enemyDataName))
            {
                GD.PushError($"Enemy data for {enemyDataName} not found");
                return;
            }

            EnemyName = enemyDataName;
            Stat = data.EnemyData[enemyDataName];
            MoveSpeed = data.GetLerpedSpeedStat(Stat.MoveSpeed, MIN_SPEED, MAX_SPEED);
            battleTexture = new AtlasTexture
            {
                Atlas = GD.Load<Texture>($"res://Entities/enemies/sprites/{enemyDataName}_Battle.png")
            };

            if (Sprite == null || stateMachine == null)
            {
                await ToSignal(this, "ready");
            }

            Sprite.Texture = GD.Load<Texture>($"res://Entities/enemies/sprites/{enemyDataName}_Overworld.png");
            stateMachine.Behaviour =
                GD.Load<AI_Behaviour>($"res://AI/Resources/{Stat.AiType.ToLower()}_behaviour.tres");
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