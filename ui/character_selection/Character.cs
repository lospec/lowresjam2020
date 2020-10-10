using Godot;

namespace HeroesGuild.ui.character_selection
{
    public class Character : MarginContainer
    {
        private const string OverworldSpriteSheetPath =
            "res://entities/player/spritesheets/{0}_overworld.png";
        private TextureRect _hoverTextureRect;

        public TextureButton characterButton;

        public string characterName;

        public override void _Ready()
        {
            characterButton = GetNode<TextureButton>("Character");
            _hoverTextureRect = GetNode<TextureRect>("Control/Hover");
        }

        public bool UpdateCharacter()
        {
            var tex = new AtlasTexture
            {
                Atlas = GD.Load<Texture>(string.Format(OverworldSpriteSheetPath,
                    characterName.Replace(" ", "_").ToLower()))
            };

            if (tex.Atlas == null) return false;

            tex.Region = new Rect2(0, 0, 8, 12);
            characterButton.TextureNormal = tex;
            return true;
        }

        private void OnCharacter_MouseEntered()
        {
            _hoverTextureRect.Visible = true;
        }

        private void OnCharacter_MouseExited()
        {
            _hoverTextureRect.Visible = false;
        }
    }
}