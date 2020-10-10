using Godot;
using HeroesGuild.utility;

namespace HeroesGuild.ui.game_over
{
    public class GameOver : PanelContainer
    {
        private const string CharacterSelectionScenePath =
            "res://ui/character_selection/character_selector.tscn";

        private bool _changingScene;

        public override void _Ready()
        {
            AudioSystem.PlayMusic(AudioSystem.MusicCollection.BattleGameOver);
        }

        public override void _Input(InputEvent @event)
        {
            switch (@event)
            {
                case InputEventKey eventKey when eventKey.Pressed && !_changingScene:
                {
                    GoToCharacterSelector();
                    break;
                }
                case InputEventMouseButton eventMouseButton
                    when eventMouseButton.Pressed && eventMouseButton
                        .ButtonIndex == (int)ButtonList.Left && !_changingScene:
                    GoToCharacterSelector();
                    break;
            }
        }

        private async void GoToCharacterSelector()
        {
            AudioSystem.StopAllMusic();

            _changingScene = true;
            var transitionParams =
                new Transitions.TransitionParams(
                    Transitions.TransitionType.ShrinkingCircle, 0.2f);
            var transitions = Autoload.Get<Transitions>();
            await transitions.ChangeSceneDoubleTransition(CharacterSelectionScenePath,
                transitionParams);
        }
    }
}
