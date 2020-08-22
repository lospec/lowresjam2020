using System.Linq;
using Godot;
using Godot.Collections;
using HeroesGuild.data;
using HeroesGuild.UI.character_selection;
using HeroesGuild.Utility;

public class CharacterSelector : MarginContainer
{
    private const string WORLD_SCENE_PATH = "res://World/World.tscn";
    private const string CHARACTER_RESOURCE_PATH = "res://UI/character_selection/character.tscn";
    private const int SCROLL_AMOUNT = 40;
    private static readonly PackedScene CharacterResource = ResourceLoader.Load<PackedScene>(CHARACTER_RESOURCE_PATH);

    public string SelectedCharacterName;

    private bool _scrollLeftHeld = false;
    private bool _scrollRightHeld = false;
    private float _scrollValue = 0f;

    private ScrollContainer characterScroll;
    private BoxContainer characters;
    private BoxContainer selectVBox;
    private Label nameLabel;
    private BaseButton[] buttons;

    public override void _Ready()
    {
        characterScroll = GetNode<ScrollContainer>("MarginContainer/VBoxContainer/CenterContainer/CharactersScroll");
        characters = characterScroll.GetNode<BoxContainer>("Characters");
        selectVBox = GetNode<BoxContainer>("MarginContainer/VBoxContainer/VBoxContainer2/SelectVBox");
        nameLabel = selectVBox.GetNode<Label>("VBoxContainer/Label");
        buttons = new[]
        {
            GetNode<BaseButton>("MarginContainer/VBoxContainer/VBoxContainer2/HBoxContainer/ScrollLeft"),
            GetNode<BaseButton>("MarginContainer/VBoxContainer/VBoxContainer2/HBoxContainer/ScrollRight"),
            GetNode<BaseButton>("MarginContainer/VBoxContainer/VBoxContainer2/SelectVBox/Select"),
        };

        selectVBox.Visible = false;
        UpdateCharacters();
        foreach (var button in buttons)
        {
            button.Connect("mouse_entered", this, nameof(OnButton_MouseEntered), new Array {button});
            button.Connect("mouse_entered", this, nameof(OnButton_Pressed), new Array {button});
        }
    }

    private void UpdateCharacters()
    {
        var saveData = Singleton.Get<SaveData>(this);

        foreach (var keyValuePair in Singleton.Get<Data>(this).CharacterData)
        {
            var characterName = keyValuePair.Key;
            var characterRecord = keyValuePair.Value;

            if (characterRecord.GuildLevel > saveData.GuildLevel)
            {
                continue;
            }

            var character = (Character) CharacterResource.Instance();
            characters.AddChild(character);
            character.CharacterName = characterName;
            if (!character.UpdateCharacter())
            {
                character.QueueFree();
                continue;
            }

            character.characterButton.Connect("pressed", this, nameof(OnCharacter_Pressed), new Array {character});
        }
    }

    public void OnScrollLeft_GUIInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton inputEvent && inputEvent.ButtonIndex == (int) ButtonList.Left)
        {
            _scrollLeftHeld = inputEvent.Pressed;
        }
    }

    public void OnScrollRight_GUIInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton inputEvent && inputEvent.ButtonIndex == (int) ButtonList.Left)
        {
            _scrollRightHeld = inputEvent.Pressed;
        }
    }

    public override void _Process(float delta)
    {
        if (_scrollLeftHeld)
        {
            _scrollValue -= SCROLL_AMOUNT * delta;
        }

        if (_scrollRightHeld)
        {
            _scrollValue += SCROLL_AMOUNT * delta;
        }

        _scrollValue = Mathf.Max(_scrollValue, 0);
        characterScroll.ScrollHorizontal = (int) _scrollValue;
        if ((int) _scrollValue > characterScroll.ScrollHorizontal)
        {
            _scrollValue = characterScroll.ScrollHorizontal;
        }
    }

    private void OnCharacter_Pressed(Character character)
    {
        selectVBox.Visible = true;
        SelectedCharacterName = character.CharacterName;
        nameLabel.Text = character.CharacterName;
        AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, character.RectGlobalPosition, -15);
    }

    private async void OnSelect_Pressed()
    {
        Singleton.Get<SaveData>(this).CharacterName = SelectedCharacterName;
        var transitionParams = new Transitions.TransitionParams(Transitions.TransitionType.ShrinkingCircle, 0.2f);
        await Singleton.Get<Transitions>(this).ChangeSceneDoubleTransition(WORLD_SCENE_PATH, transitionParams);
    }

    private void OnButton_Pressed(Control button)
    {
        AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, button.RectGlobalPosition, -15);
    }

    private void OnButton_MouseEntered(Control button)
    {
        AudioSystem.PlaySFX(AudioSystem.SFX.ButtonHover, button.RectGlobalPosition, -20);
    }
}