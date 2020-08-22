using Godot;

namespace HeroesGuild.Utility
{
    [Tool]
    public class PaletteSwap : CanvasLayer
    {
        private bool _enabled;
        private Texture _palette;

        private TextureRect _paletteSwapTextureRect;

        [Export] public bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                if (!HasNode("TextureRect"))
                {
                    return;
                }

                ((ShaderMaterial) _paletteSwapTextureRect.Material).SetShaderParam("enabled", _enabled);
            }
        }
        [Export] public Texture Palette
        {
            get => _palette;
            set
            {
                _palette = value;
                if (!HasNode("TextureRect") || value == null)
                {
                    return;
                }

                ((ShaderMaterial) _paletteSwapTextureRect.Material).SetShaderParam("palette_tex", _palette);
            }
        }

        public override void _Ready()
        {
            _paletteSwapTextureRect = GetNode<TextureRect>("TextureRect");
        }
    }
}