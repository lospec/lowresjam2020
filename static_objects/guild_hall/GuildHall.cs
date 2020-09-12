using Godot;

namespace HeroesGuild.StatisObjects.GuildHall
{
	public class GuildHall : StaticBody2D
	{
		public AnimatedSprite flag1, flag2, flag3, flag4;

		public override void _Ready()
		{
			flag1 = GetNode<AnimatedSprite>("Flag");
			flag2 = GetNode<AnimatedSprite>("Flag2");
			flag3 = GetNode<AnimatedSprite>("Flag3");
			flag4 = GetNode<AnimatedSprite>("Flag4");

			flag1.Frame = 0;
			flag2.Frame = 1;
			flag3.Frame = 2;
			flag4.Frame = 3;

			flag1.Play("idle");
			flag2.Play("idle");
			flag3.Play("idle");
			flag4.Play("idle");
		}

		private void _on_InteractableDoor_body_entered(Node body)
		{
			GD.Print(body.Name);
		}
	}
}
