using Godot;

namespace HeroesGuild.Combat
{
    public class CombatAttackAnim : TextureRect
    {
        [Signal] public delegate void EffectDone();

        public bool playing = false;

        public async void Play(AnimatedTexture texture, Color color, float minDuration = 0f)
        {
            SceneTreeTimer timer = null;
            if (minDuration > 0)
            {
                timer = GetTree().CreateTimer(minDuration);
            }

            Texture = texture;
            ((AnimatedTexture) Texture).CurrentFrame = 0;
            Modulate = color;
            Visible = true;
            playing = true;
            await ToSignal(GetTree().CreateTimer(texture.Frames / texture.Fps), "timeout");
            playing = false;
            Visible = false;

            if (minDuration > 0 && timer?.TimeLeft > 0)
            {
                GD.Print(timer.TimeLeft);
                await ToSignal(timer, "timeout");
                EmitSignal(nameof(EffectDone));
            }
            else
            {
                EmitSignal(nameof(EffectDone));
            }
        }
    }
}