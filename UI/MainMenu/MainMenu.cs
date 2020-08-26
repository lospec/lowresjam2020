using Godot;
using Godot.Collections;
using HeroesGuild.Utility;

namespace HeroesGuild.UI.MainMenu
{
    public class MainMenu : MarginContainer
    {
        private const string CharacterSelectionScenePath = "res://UI/character_selection/character_selector.tscn";
        private TextureRect _startSignifierLabel;
        private AnimationPlayer _startSignifierAnimationPlayer;

        private bool _changingScene = false;

        private AudioStreamPlayer2D _sfxPlayer;

        public override void _Ready()
        {
            _startSignifierLabel = GetNode<TextureRect>("StartSigniferMargin/StartSignifier");
            _startSignifierAnimationPlayer = GetNode<AnimationPlayer>("StartSigniferMargin/AnimationPlayer");

            var heroesGuildSFX = new Dictionary<AudioSystem.SFX, float>()
            {
                {AudioSystem.SFX.HeroesGuildNox1, -20f},
                {AudioSystem.SFX.HeroesGuildNox3, -20f},
                {AudioSystem.SFX.HeroesGuildNox4, -20f},
                {AudioSystem.SFX.HeroesGuildPureasbestos1, -20f},
                {AudioSystem.SFX.HeroesGuildUnsettled1, -22f},
                {AudioSystem.SFX.HeroesGuildWildleoknight2, -18f},
            };
            var sfx = heroesGuildSFX.Keys.RandomElement();
            var volume = heroesGuildSFX[sfx];
            _sfxPlayer = AudioSystem.PlaySFX(sfx, null, volume);
            _sfxPlayer.Connect("finished", _startSignifierAnimationPlayer, "play", new Array{"flash"});
            _sfxPlayer.Connect("finished", AudioSystem.instance, nameof(AudioSystem.instance.PlayMusic), 
                new Array {AudioSystem.Music.TitleScreen, -25f,});
        }

        public override void _Input(InputEvent @event)
        {
            switch (@event)
            {
                case InputEventKey eventKey when eventKey.Pressed && !_changingScene:
                {
                    if (OS.IsDebugBuild() && eventKey.Scancode == (int) KeyList.F9 && eventKey.Shift)
                    {
                        //TODO: AI-Editor
                    }
                    else
                    {
                        GoToCharacterSelector();
                    }

                    break;
                }
                case InputEventMouseButton eventMouseButton when eventMouseButton.Pressed && eventMouseButton
                    .ButtonIndex == (int) ButtonList.Left && !_changingScene:
                    GoToCharacterSelector();
                    break;
            }
        }

        private async void GoToCharacterSelector()
        {
            _sfxPlayer?.Stop();
            AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, Vector2.Zero, -15);
            _changingScene = true;
            var transitionParams = new Transitions.TransitionParams(Transitions.TransitionType.ShrinkingCircle, 0.2f);
            var transitions = Autoload.Get<Transitions>();
            await transitions.ChangeSceneDoubleTransition(CharacterSelectionScenePath, transitionParams);
        }
    }
}