using Godot;
using HeroesGuild.combat.Effects;

namespace HeroesGuild.combat
{
    public class CombatEnemyTexture : TextureRect, IShakable
    {
        private IShakable _shakableImplementation;

        public void Shake(float duration, float frequency, float amplitude)
        {
            _shakableImplementation.Shake(duration, frequency, amplitude);
        }

        public override void _Ready()
        {
            base._Ready();
            _shakableImplementation = new ShakeEffect(this);
        }

        public override void _Process(float delta)
        {
            base._Process(delta);
            ((ShakeEffect) _shakableImplementation).Process(delta);
        }
    }
}