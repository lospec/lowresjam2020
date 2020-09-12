using Godot;
using System;

namespace HeroesGuild.InteractableObjects
{
	public class InteractableObject : Area2D
	{
		private void _on_InteractableObject_body_entered(Node body)
		{
			GD.Print(body.Name);
		}
	}
}
