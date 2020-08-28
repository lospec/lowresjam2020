using Godot;

namespace HeroesGuild.Utility
{
	public class PixelPerfectScaling : Node
	{
		private Viewport _root;
		private Vector2 _baseSize;

		public override void _Ready()
		{
			_root = GetTree().Root;
			_baseSize = _root.GetVisibleRect().Size;
			if (GetTree().Connect("screen_resized", this, nameof(OnScreen_Resized)) !=
				Error.Ok)
			{
				GD.PushError(
					"An error occurred while connecting screen_resized signal.");
			}

			_root.SetAttachToScreenRect(_root.GetVisibleRect());
			OnScreen_Resized();
		}

		private void OnScreen_Resized()
		{
			var newWindowSize = OS.WindowSize;
			var scaleH = Mathf.Max((int) (newWindowSize.x / _baseSize.x), 1);
			var scaleW = Mathf.Max((int) (newWindowSize.y / _baseSize.y), 1);
			var scale = Mathf.Min(scaleH, scaleW);

			var diff = newWindowSize - _baseSize * scale;
			var diffHalf = (diff * 0.5f).Floor();
			_root.SetAttachToScreenRect(new Rect2(diffHalf, _baseSize * scale));
			var oddOffset = new Vector2
			{
				x = (int) newWindowSize.x % 2,
				y = (int) newWindowSize.y % 2
			};

			VisualServer.BlackBarsSetMargins(
				(int) Mathf.Max(diffHalf.x, 0),
				(int) Mathf.Max(diffHalf.y, 0),
				(int) Mathf.Max(diffHalf.x, 0) + (int) oddOffset.x,
				(int) Mathf.Max(diffHalf.y, 0) + (int) oddOffset.y
			);
		}
	}
}
