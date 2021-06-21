using System.Threading.Tasks;
using Godot;
using HeroesGuild.ai;
using HeroesGuild.data;
using HeroesGuild.entities.base_entity;
using HeroesGuild.entities.player;
using HeroesGuild.utility;

namespace HeroesGuild.entities.enemies.base_enemy
{
    public class BaseEnemy : BaseEntity, IDependency<BaseEnemy.EnemyDependencies>
    {
        public struct EnemyDependencies
        {
            public BaseEntity PlayerInstance { get; set; }
            public string EnemyName { get; set; }
        }

        [Signal] public delegate void Died(BaseEnemy enemy);

        [Signal] public delegate void StatsLoaded();

        private const string BattleSpritePath =
            "res://entities/enemies/sprites/{0}_battle.png";
        private const string OverworldSpritePath =
            "res://entities/enemies/sprites/{0}_overworld.png";
        private const string AIResourcePath = "res://ai/resources/{0}_behaviour.tres";

        private const float MAX_SPEED = 40;
        private const float MIN_SPEED = 5;
        public AtlasTexture battleTexture;

        [Export] public string enemyName;
        public StateMachine stateMachine;
        private readonly DependencyRequester<EnemyDependencies>
            _dependencyImplementation =
                new DependencyRequester<EnemyDependencies>();

        public EnemyRecord Stat { get; set; }

        private Area2D _isSpawnSafeArea2D;

        public override void _Ready()
        {
            base._Ready();
            stateMachine = GetNode<StateMachine>("StateMachine");

            var dependency = _dependencyImplementation.Dependency;
            LoadEnemy(dependency.EnemyName);
            ((Player) dependency.PlayerInstance).ToggleEnemyActive += (entity, b) =>
            {
                if (entity == this)
                {
                    stateMachine.active = b;
                }
            };

            _isSpawnSafeArea2D = GetNode<Area2D>("IsSpawnSafeArea2D");

            if (Stat.MaxHealth <= 0) return;
            Health = Stat.MaxHealth;
        }

        private async void LoadEnemy(string enemyDataName)
        {
            var data = Autoload.Get<Data>();
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
                Atlas = GD.Load<Texture>(string.Format(BattleSpritePath,
                    enemyDataName.ToLower()))
            };

            if (sprite == null || stateMachine == null) await ToSignal(this, "ready");


            sprite.Texture =
                GD.Load<Texture>(string.Format(OverworldSpritePath,
                    enemyDataName.ToLower()));

            stateMachine.behaviour =
                GD.Load<AI_Behaviour>(string.Format(AIResourcePath,
                    Stat.AiType.ToLower()));
            EmitSignal(nameof(StatsLoaded));
        }

        public async Task<bool> IsInAllowedTile()
        {
            await ToSignal(GetTree(), "physics_frame");
            return _isSpawnSafeArea2D.GetOverlappingBodies().Count == 0;
        }

        public void Die()
        {
            EmitSignal(nameof(Died), this);
            QueueFree();
        }

        public IDependency<EnemyDependencies>.Dependency OnDependency
        {
            get => _dependencyImplementation.OnDependency;
            set => _dependencyImplementation.OnDependency = value;
        }
    }
}