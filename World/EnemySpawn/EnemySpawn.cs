using Godot;

namespace HeroesGuild.World.EnemySpawn
{
    public class EnemySpawn : Area2D
    {
        [Export] public int enemyNumber;
        [Export] public string[] enemies;

        private CollisionShape2D collisionShape;

        public Vector2 GetRandomGlobalPosition()
        {
            var center = collisionShape.GlobalPosition;
            var size = ((RectangleShape2D) collisionShape.Shape).Extents;
            return new Vector2
            {
                x = (float) GD.RandRange(center.x - size.x, center.x + size.x),
                y = (float) GD.RandRange(center.y - size.y, center.y + size.y)
            };
        }
    }
}