using Godot;

namespace HeroesGuild.Utility
{
    public static class Singleton
    {
        public static T Get<T>(Node accessNode) where T : Object
        {
            var instance = Engine.HasSingleton(typeof(T).Name)
                ? (T) Engine.GetSingleton(typeof(T).Name)
                : accessNode.GetNode<T>($"/root/{typeof(T).Name}");
            return instance;
        }
    }
}