namespace HeroesGuild.utility
{
    public interface IDependency<in T>
    {
        void OnDependency(T dependency);
    }
}