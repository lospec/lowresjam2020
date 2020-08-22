using Godot;
using HeroesGuild.Utility;

namespace HeroesGuild.UI.MainMenu
{
    public class MainMenu : MarginContainer
    {
        private const string CharacterSelectionScenePath = "res://UI/character_selection/character_selector.tscn";
        private TextureRect _startSignifierLabel;
        private AnimationPlayer _startSignifierAnimationPlayer;

        private bool _changing_scene = false;

        // TODO: AUDIOSYSTEM
        //var _sfx_player

        public override void _Ready()
        {
            _startSignifierLabel = GetNode<TextureRect>("StartSigniferMargin/StartSignifier");
            _startSignifierAnimationPlayer = GetNode<AnimationPlayer>("StartSigniferMargin/AnimationPlayer");

            //TODO: AudioSystem
            /*var heroes_guild_sfx = {
                AudioSystem.SFX.HEROES_GUILD_NOX_1: -20,
                AudioSystem.SFX.HEROES_GUILD_NOX_3: -20,
                AudioSystem.SFX.HEROES_GUILD_NOX_4: -20,
                AudioSystem.SFX.HEROES_GUILD_PUREASBESTOS_1: -20,
                AudioSystem.SFX.HEROES_GUILD_UNSETTLED_1: -22,
                AudioSystem.SFX.HEROES_GUILD_WILDLEOKNIGHT_2: -18,
            }
	
            var sfx = Utility.rand_element(heroes_guild_sfx.keys())
            var volume = heroes_guild_sfx[sfx]
            _sfx_player = AudioSystem.play_sfx(sfx,
                null, volume)
	
            _sfx_player.connect("finished", start_signifier_animation_player, "play",
                ["flash"])
            _sfx_player.connect("finished", AudioSystem, "play_music",
                [AudioSystem.Music.TITLE_SCREEN, -25])*/
        }

        public override void _Input(InputEvent @event)
        {
            switch (@event)
            {
                case InputEventKey eventKey when eventKey.Pressed && !_changing_scene:
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
                    .ButtonIndex == (int) ButtonList.Left && !_changing_scene:
                    GoToCharacterSelector();
                    break;
            }
        }

        private async void GoToCharacterSelector()
        {
            //TODO: AudioSystem
            /*if _sfx_player:
            _sfx_player.stop()
	
            var _s = AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK,
                Vector2.ZERO, -15)*/

            _changing_scene = true;
            var transitionParams = new Transitions.TransitionParams(Transitions.TransitionType.ShrinkingCircle, 0.2f);
            var transitions = Singleton.Get<Transitions>(this);
            await transitions.ChangeSceneDoubleTransition(CharacterSelectionScenePath, transitionParams);
        }
    }
}