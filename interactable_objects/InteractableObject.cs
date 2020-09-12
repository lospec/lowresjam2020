using Godot;
namespace HeroesGuild.InteractableObjects
{
	public abstract class InteractableObject : Area2D
	{
		protected abstract void _on_InteractableObject_body_entered(Node body);
	}
}
