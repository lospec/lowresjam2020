using System.Collections.Generic;
using Godot;
using Godot.Collections;
using HeroesGuild.data;
using HeroesGuild.utility;

namespace HeroesGuild.ui.main_menu
{
    public class MainMenu : MarginContainer
    {
        private const string CharacterSelectionScenePath =
            "res://ui/character_selection/character_selector.tscn";

        private readonly List<SFXRecord> _introSFX = new List<SFXRecord>
        {
            AudioSystem.SFXCollection.TitleScreenIntroNox1,
            AudioSystem.SFXCollection.TitleScreenIntroNox2,
            AudioSystem.SFXCollection.TitleScreenIntroNox3,
            AudioSystem.SFXCollection.TitleScreenIntroPureAsbestos,
            AudioSystem.SFXCollection.TitleScreenIntroWildleoknight
        };

        private bool _changingScene = false;
        private bool _introSFXPlayed = false;

        private AudioStreamPlayer _introSFXPlayer;
        private AnimationPlayer _startSignifierAnimationPlayer;
        private TextureRect _startSignifierLabel;

        public override void _Ready()
        {
            _startSignifierLabel =
                GetNode<TextureRect>("StartSignifierMargin/StartSignifier");
            _startSignifierAnimationPlayer =
                GetNode<AnimationPlayer>("StartSignifierMargin/AnimationPlayer");

            var sfx = _introSFX.RandomElement();
            _introSFXPlayer = AudioSystem.PlaySFX(sfx);

            _introSFXPlayer.Connect("finished", _startSignifierAnimationPlayer, "play",
                new Array {"flash"});
            _introSFXPlayer.Connect("finished", AudioSystem.instance,
                nameof(AudioSystem.PlayMusic),
                new Array {"TitleScreen"});
            _introSFXPlayer.Connect("tree_exited", this, nameof(SetIntroSFXPlayed));
        }

        public override void _Input(InputEvent @event)
        {
            switch (@event)
            {
                case InputEventKey eventKey when eventKey.Pressed && !_changingScene:
                {
                    if (OS.IsDebugBuild() && eventKey.Scancode == (int) KeyList.F9 &&
                        eventKey.Shift)
                    {
                        //TODO: AI-Editor
                    }
                    else
                    {
                        GoToCharacterSelector();
                    }

                    break;
                }
                case InputEventMouseButton eventMouseButton
                    when eventMouseButton.Pressed && eventMouseButton
                        .ButtonIndex == (int) ButtonList.Left && !_changingScene:
                    GoToCharacterSelector();
                    break;
            }
        }

        private async void GoToCharacterSelector()
        {
            if (!_introSFXPlayed)
            {
                _introSFXPlayer.Stop();
                _introSFXPlayer.QueueFree();

                AudioSystem.PlayMusic(AudioSystem.MusicCollection.TitleScreen);
            }

            AudioSystem.PlaySFX(AudioSystem.SFXCollection.TitleScreenKeyPressed);

            _changingScene = true;
            var transitionParams =
                new Transitions.TransitionParams(
                    Transitions.TransitionType.ShrinkingCircle, 0.2f);
            var transitions = Autoload.Get<Transitions>();
            await transitions.ChangeSceneDoubleTransition(CharacterSelectionScenePath,
                transitionParams);
        }

        private void SetIntroSFXPlayed()
        {
            _introSFXPlayed = true;
        }
    }
}