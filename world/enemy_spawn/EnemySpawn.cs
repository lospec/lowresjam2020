using Godot;

namespace HeroesGuild.world.enemy_spawn
{
    public class EnemySpawn : Area2D
    {
        private CollisionShape2D _collisionShape;
        [Export] public string[] enemies;
        [Export] public int enemyNumber;

        public override void _Ready()
        {
            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        }

        public Vector2 GetRandomGlobalPosition()
        {
            var center = _collisionShape.GlobalPosition;
            var size = ((RectangleShape2D) _collisionShape.Shape).Extents;
            return new Vector2
            {
                x = (float) GD.RandRange(center.x - size.x, center.x + size.x),
                y = (float) GD.RandRange(center.y - size.y, center.y + size.y)
            };
        }
    }
}