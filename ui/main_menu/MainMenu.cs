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
        private const string WorldScenePath = "res://world/world.tscn";

        private List<SFXRecord> _introSFX;

        private bool _changingScene = false;
        private bool _introSFXPlayed = false;

        private AudioStreamPlayer _introSFXPlayer;
        private AnimationPlayer _startSignifierAnimationPlayer;

        public override void _Ready()
        {
            _introSFX = new List<SFXRecord>
            {
                AudioSystem.SFXCollection.TitleScreenIntroNox1,
                AudioSystem.SFXCollection.TitleScreenIntroNox2,
                AudioSystem.SFXCollection.TitleScreenIntroNox3,
                AudioSystem.SFXCollection.TitleScreenIntroPureAsbestos,
                AudioSystem.SFXCollection.TitleScreenIntroWildleoknight
            };

            _startSignifierAnimationPlayer =
                GetNode<AnimationPlayer>("StartSignifierMargin/AnimationPlayer");

            var sfx = _introSFX.RandomElement();
            _introSFXPlayer = AudioSystem.PlaySFX(sfx);

            _introSFXPlayer.Connect("finished", _startSignifierAnimationPlayer, "play",
                new Array {"flash"});
            _introSFXPlayer.Connect("finished", this, nameof(PlayTitleScreenMusic));
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
                        GoToNextMenu();
                    }

                    break;
                }
                case InputEventMouseButton eventMouseButton
                    when eventMouseButton.Pressed && eventMouseButton
                        .ButtonIndex == (int) ButtonList.Left && !_changingScene:
                    GoToNextMenu();
                    break;
            }
        }

        private void GoToNextMenu()
        {
            if (!_introSFXPlayed)
            {
                _introSFXPlayer.Stop();
                _introSFXPlayer.QueueFree();

                PlayTitleScreenMusic();
            }

            AudioSystem.PlaySFX(AudioSystem.SFXCollection.TitleScreenKeyPressed);

            _changingScene = true;
            SaveManager.LoadGame(out bool wasSuccessful);
            if (wasSuccessful && !SaveManager.SaveData.isDead)
            {
                GoToWorld();
            }
            else
            {
                GoToCharacterSelector();
            }
        }

        private async void GoToCharacterSelector()
        {
            var transitionParams =
                new Transitions.TransitionParams(
                    Transitions.TransitionType.ShrinkingCircle, 0.2f);
            var transitions = Autoload.Get<Transitions>();
            await transitions.ChangeSceneDoubleTransition(CharacterSelectionScenePath,
                transitionParams);
        }

        private async void GoToWorld()
        {
            var transitionParams =
                new Transitions.TransitionParams(
                    Transitions.TransitionType.ShrinkingCircle, 0.2f);
            await Autoload.Get<Transitions>()
                .ChangeSceneDoubleTransition(WorldScenePath, transitionParams);
        }

        private void SetIntroSFXPlayed()
        {
            _introSFXPlayed = true;
        }

        private void PlayTitleScreenMusic()
        {
            AudioSystem.PlayMusic(AudioSystem.MusicCollection.TitleScreen);
        }
    }
}