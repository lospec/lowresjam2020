using Godot;

namespace HeroesGuild.Utility
{
    public class Singleton : Node
    {
        private static Singleton instance;

        public override void _Ready()
        {
            base._Ready();
            instance = this;
        }

        public static T Get<T>(Node accessNode) where T : Object
        {
            return instance.GetTree().Root.GetNode<T>($"{typeof(T).Name}");
        }
    }
}