namespace HeroesGuild.utility
{
    public interface IDependency<T>
    {
        delegate void Dependency(ref T dependency);

        Dependency OnDependency { get; set; }
    }
}