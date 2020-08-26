using Godot;

namespace HeroesGuild.UI.Credits
{
    public class Credits : Node
    {
        private const string MainMenuPath = "res://UI/Main Menu/Main Menu.tscn";
        private const int SCROLL_SPEED = 5;
        private const int THANKS_GAP = 20;
        private const int MAX_THANKS_POS_Y = 0;
        private const int START_SCROLL_POS = 43;

        private VBoxContainer _mainCreditsVBox;
        private Label _thanksLabel;

        public override void _Ready()
        {
            _mainCreditsVBox = GetNode<VBoxContainer>("Main Credits");
            _thanksLabel = GetNode<Label>("Thanks");
        }

        private void OnCredits_SortChildren()
        {
            var rectPosition = _mainCreditsVBox.RectPosition;
            rectPosition.y = START_SCROLL_POS;
            _mainCreditsVBox.RectPosition = rectPosition;
            rectPosition = _thanksLabel.RectPosition;
            rectPosition.y =
                _mainCreditsVBox.RectSize.y + THANKS_GAP + START_SCROLL_POS;
            _thanksLabel.RectPosition = rectPosition;
        }

        public override void _Process(float delta)
        {
            var rectPosition = _mainCreditsVBox.RectPosition;
            rectPosition.y -= SCROLL_SPEED * delta;
            rectPosition = _thanksLabel.RectPosition;
            if (rectPosition.y > MAX_THANKS_POS_Y)
            {
                rectPosition.y -= SCROLL_SPEED * delta;
                _thanksLabel.RectPosition = rectPosition;
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (Input.IsActionJustPressed("ui_cancel"))
            {
                ExitToMainMenu();
            }
        }

        private void ExitToMainMenu()
        {
            if (GetTree().ChangeScene(MainMenuPath) != Error.Ok)
            {
                GD.PushError(
                    "An error occured while attempting to change to the main menu scene");
            }
        }
    }
}