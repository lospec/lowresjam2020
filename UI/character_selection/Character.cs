using Godot;

namespace HeroesGuild.UI.character_selection
{
    public class Character : MarginContainer
    {
        public string CharacterName;

        public TextureButton characterButton;
        private TextureRect hoverTextureRect;

        public override void _Ready()
        {
            characterButton = GetNode<TextureButton>("Character");
            hoverTextureRect = GetNode<TextureRect>("Control/Hover");
        }

        public bool UpdateCharacter()
        {
            var tex = new AtlasTexture();
            tex.Atlas = GD.Load<Texture>
                ($"res://Entities/Player/spritesheets/{CharacterName.Replace(" ", "_")}_Overworld.png");
            if (tex.Atlas == null)
            {
                return false;
            }

            tex.Region = new Rect2(0, 0, 8, 12);
            characterButton.TextureNormal = tex;
            return true;
        }

        private void OnCharacter_MouseEntered()
        {
            hoverTextureRect.Visible = true;
        }

        private void OnCharacter_MouseExited()
        {
            hoverTextureRect.Visible = false;
        }
    }
}