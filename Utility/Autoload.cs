using Godot;

namespace HeroesGuild.Utility
{
    public class Autoload : Node
    {
        private static Autoload _instance;

        public override void _Ready()
        {
            base._Ready();
            _instance = this;
        }

        public static T Get<T>() where T : Object
        {
            return _instance.GetTree().Root.GetNode<T>($"{typeof(T).Name}");
        }
    }
}