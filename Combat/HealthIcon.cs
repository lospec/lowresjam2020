using Godot;

namespace HeroesGuild.Combat
{
    public class HealthIcon : TextureRect
    {
        [Signal] public delegate void Stopped();

        public bool isBlinking = false;
        private float _duration = 0;
        private float _timeLeft = 0;
        private int _frequency;

        private float T => _duration - _timeLeft;

        public override void _Process(float delta)
        {
            if (!isBlinking)
            {
                return;
            }

            if (_timeLeft <= 0)
            {
                StopBlink();
                return;
            }

            var color = Modulate;
            color.a = Mathf.FloorToInt(T * _frequency) % 2 == 0 ? 0 : 1;
            Modulate = color;
            _timeLeft -= delta;
        }

        private void StopBlink()
        {
            isBlinking = false;
            _timeLeft = 0;
            var color = Modulate;
            color.a = 1;
            Modulate = color;
            EmitSignal(nameof(Stopped));
        }

        public void Blink(float duration, int frequency)
        {
            if (_timeLeft > 0)
            {
                StopBlink();
            }

            isBlinking = true;
            _duration = duration;
            _timeLeft = duration;
            _frequency = frequency;
        }
    }
}