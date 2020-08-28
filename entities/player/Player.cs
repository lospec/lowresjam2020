using Godot;

namespace HeroesGuild.Entities.Player
{
	public class Player : BaseEntity.BaseEntity
	{
		public Camera2D camera;

		public override void _Ready()
		{
			camera = GetNode<Camera2D>("Camera2D");

			base._Ready();
		}

		public override void _PhysicsProcess(float delta)
		{
			Velocity = GetInputVelocity().Normalized() * moveSpeed;

			base._PhysicsProcess(delta);
		}

		public Vector2 GetInputVelocity()
		{
			return new Vector2
			{
				x = Input.GetActionStrength("player_move_right") - Input.GetActionStrength("player_move_left"),
				y = Input.GetActionStrength("player_move_down") - Input.GetActionStrength("player_move_up")
			};
		}
	}
}
