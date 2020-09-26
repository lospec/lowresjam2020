using Godot;
using Godot.Collections;
using HeroesGuild.Data;
using HeroesGuild.Utility;

namespace HeroesGuild.UI.CharacterSelection
{
	public class CharacterSelector : MarginContainer
	{
		private const string WORLD_SCENE_PATH = "res://world/world.tscn";
		private const string CHARACTER_RESOURCE_PATH = "res://ui/character_selection/character.tscn";
		private const int SCROLL_AMOUNT = 40;
		private static readonly PackedScene CharacterResource = ResourceLoader.Load<PackedScene>(CHARACTER_RESOURCE_PATH);

		public string selectedCharacterName;

		private bool _scrollLeftHeld = false;
		private bool _scrollRightHeld = false;
		private float _scrollValue = 0f;

		private ScrollContainer _characterScroll;
		private BoxContainer _characters;
		private BoxContainer _selectVBox;
		private Label _nameLabel;
		private BaseButton[] _buttons;

		public override void _Ready()
		{
			_characterScroll = GetNode<ScrollContainer>("MarginContainer/VBoxContainer/CenterContainer/CharactersScroll");
			_characters = _characterScroll.GetNode<BoxContainer>("Characters");
			_selectVBox = GetNode<BoxContainer>("MarginContainer/VBoxContainer/VBoxContainer2/SelectVBox");
			_nameLabel = _selectVBox.GetNode<Label>("VBoxContainer/Label");
			_buttons = new[]
			{
				GetNode<BaseButton>("MarginContainer/VBoxContainer/VBoxContainer2/HBoxContainer/ScrollLeft"),
				GetNode<BaseButton>("MarginContainer/VBoxContainer/VBoxContainer2/HBoxContainer/ScrollRight"),
				GetNode<BaseButton>("MarginContainer/VBoxContainer/VBoxContainer2/SelectVBox/Select"),
			};

			_selectVBox.Visible = false;
			UpdateCharacters();
			foreach (var button in _buttons)
			{
				button.Connect("mouse_entered", this, nameof(OnButton_MouseEntered), new Array {button});
				button.Connect("mouse_entered", this, nameof(OnButton_Pressed), new Array {button});
			}
		}

		private void UpdateCharacters()
		{
			var saveData = Autoload.Get<SaveData>();

			foreach (var keyValuePair in Autoload.Get<Data.Data>().characterData)
			{
				var characterName = keyValuePair.Key;
				var characterRecord = keyValuePair.Value;

				if (characterRecord.guildLevel > saveData.GuildLevel)
				{
					continue;
				}

				var character = (Character) CharacterResource.Instance();
				_characters.AddChild(character);
				character.characterName = characterName;
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
			_characterScroll.ScrollHorizontal = (int) _scrollValue;
			if ((int) _scrollValue > _characterScroll.ScrollHorizontal)
			{
				_scrollValue = _characterScroll.ScrollHorizontal;
			}
		}

		private void OnCharacter_Pressed(Character character)
		{
			_selectVBox.Visible = true;
			selectedCharacterName = character.characterName;
			_nameLabel.Text = character.characterName;
			AudioSystem.PlaySFX(AudioSystem.SFX.ButtonClick, character.RectGlobalPosition, -15);
		}

		private async void OnSelect_Pressed()
		{
			Autoload.Get<SaveData>().CharacterName = selectedCharacterName;
			var transitionParams = new Transitions.TransitionParams(Transitions.TransitionType.ShrinkingCircle, 0.2f);
			await Autoload.Get<Transitions>().ChangeSceneDoubleTransition(WORLD_SCENE_PATH, transitionParams);
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
}