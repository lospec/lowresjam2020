using Godot;

namespace HeroesGuild.world.static_objects
{
    public class GuildHall : StaticBody2D
    {
        public override void _Ready()
        {
            GetTree().CallGroup("guild_hall_flag", "play");
        }
    }
}
