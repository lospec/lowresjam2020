using Godot;

namespace HeroesGuild.world.static_objects
{
    public class GuildHall : StaticBody2D
    {
        private Position2D _playerExitPosition2D;

        public override void _Ready()
        {
            _playerExitPosition2D = GetNode<Position2D>("PlayerExitPosition");

            GetTree().CallGroup("guild_hall_flag", "play");
        }

        public Vector2 getPlayerExitPosition()
        {
            return _playerExitPosition2D.GlobalPosition;
        }
    }
}
