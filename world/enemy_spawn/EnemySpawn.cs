using Godot;

namespace HeroesGuild.World.EnemySpawn
{
    public class EnemySpawn : Area2D
    {
        [Export] public int enemyNumber;
        [Export] public string[] enemies;

        private CollisionShape2D _collisionShape;

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