using Godot;

namespace HeroesGuild.Combat
{
    public class DamageLabel : Label
    {
        [Export] public float floatSpeed = 2;
        [Export] public float fadeDuration = 2;
        [Export] public float fadeDelay = 0;

        private float _alpha;

        public override void _Ready()
        {
            _alpha = 1 + (fadeDelay / fadeDuration);
        }

        public override void _Process(float delta)
        {
            var rectPosition = RectPosition;
            rectPosition.y -= delta * floatSpeed;
            RectPosition = rectPosition;
            _alpha = Mathf.Max(0, _alpha - delta / fadeDuration);
            var modulate = Modulate;
            modulate.a = _alpha;
            Modulate = modulate;
            if (Modulate.a <= 0)
            {
                QueueFree();
            }
        }
    }
}