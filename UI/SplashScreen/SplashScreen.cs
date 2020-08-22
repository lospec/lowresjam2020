using System.Threading.Tasks;
using Godot;
using HeroesGuild.Utility;

namespace HeroesGuild.UI.SplashScreen
{
    public class SplashScreen : Node
    {
        private const string MainMenuScenePath = "res://UI/MainMenu/Main Menu.tscn";
        public bool goingToMainMenu = false;

        public override void _Ready()
        {
            Singleton.Get<PaletteSwap>(this).Enabled = false;
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventMouseButton || @event is InputEventKey)
            {
                if (!goingToMainMenu)
                {
                    GoToMainMenu();
                }
            }
        }

        public void OnAnimationPlayer_AnimationFinished(string animationName)
        {
            if (!goingToMainMenu)
            {
                goingToMainMenu = true;
                GoToMainMenu();
            }
        }

        private async void GoToMainMenu()
        {
            goingToMainMenu = true;
            var transitionParams = new Transitions.TransitionParams(Transitions.TransitionType.ShrinkingCircle, 0.15f);
            await Singleton.Get<Transitions>(this).ChangeSceneDoubleTransition(MainMenuScenePath, transitionParams);
            Singleton.Get<PaletteSwap>(this).Enabled = true;
        }
    }
}