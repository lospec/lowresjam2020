using Godot;

namespace HeroesGuild.utility
{
    public class DependencyRequester<T> : IDependency<T> where T : new()
    {
        public IDependency<T>.Dependency OnDependency { get; set; } =
            (ref T dependency) =>
            {
                GD.PrintErr($"{typeof(T).Name} :: OnDependency not initialized");
            };

        public T Dependency
        {
            get
            {
                var dependency = new T();
                OnDependency(ref dependency);
                return dependency;
            }
        }
    }
}