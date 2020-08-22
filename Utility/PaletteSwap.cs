using Godot;

namespace HeroesGuild.Utility
{
	public class PaletteSwap : CanvasLayer
	{
		[Export] private bool _enabled;
		[Export] private Texture _palette;

		private TextureRect _paletteSwapTextureRect;

		public bool Enabled
		{
			set
			{
				_enabled = value;
				if (!HasNode(_paletteSwapTextureRect.Name))
				{
					return;
				}

				((ShaderMaterial) _paletteSwapTextureRect.Material).SetShaderParam("enabled", _enabled);
			}
		}
		public Texture Palette
		{
			set
			{
				_palette = value;
				if (!HasNode(_paletteSwapTextureRect.Name))
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
