using Godot;

namespace HeroesGuild.combat.Effects
{
    public class ShakeEffect : IShakable
    {
        private readonly Control _control;
        private float _amplitude = 0f;
        private float _duration = 0f;
        private Vector2 _lastOffset = Vector2.Zero;
        private float _lastShookTimer = 0f;
        private float _periodInMs = 0f;
        private Vector2 _previous = Vector2.Zero;

        private float _timer = 0f;

        public ShakeEffect(Control control)
        {
            _control = control;
            _control.SetProcess(true);
        }

        private Vector2 RectPosition
        {
            get => _control.RectPosition;
            set => _control.RectPosition = value;
        }

        public void Shake(float duration, float frequency, float amplitude)
        {
            _duration = duration;
            _timer = duration;
            _periodInMs = 1f / frequency;
            _amplitude = amplitude;
            _previous = new Vector2
            {
                x = (float) GD.RandRange(-1f, 1f),
                y = (float) GD.RandRange(-1f, 1f)
            };
            RectPosition -= _lastOffset;
            _lastOffset = Vector2.Zero;
        }

        public void Process(float delta)
        {
            if (_timer == 0)
            {
                return;
            }

            _lastShookTimer = _lastShookTimer + delta;
            while (_lastShookTimer >= _periodInMs)
            {
                _lastShookTimer = _lastShookTimer - _periodInMs;
                var intensity = _amplitude * (1 - (_duration - _timer) / _duration);
                var newX = (float) GD.RandRange(-1f, 1f);
                var newY = (float) GD.RandRange(-1f, 1f);
                var next = new Vector2
                {
                    x = intensity * (_previous.x + delta * (newX - _previous.x)),
                    y = intensity * (_previous.y + delta * (newY - _previous.y))
                };
                _previous = next;
                var newOffset = next;
                RectPosition = RectPosition - _lastOffset + newOffset;
                _lastOffset = newOffset;
            }

            _timer -= delta;
            if (_timer <= 0)
            {
                _timer = 0;
                RectPosition -= _lastOffset;
            }
        }
    }
}