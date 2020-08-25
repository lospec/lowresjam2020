using System.Collections.Generic;
using Godot;


namespace HeroesGuild.guild_hall.chest
{
    public class Chest : StaticBody2D
    {
        public int chestID;
        public Dictionary<int, string> contents;

        public AnimatedSprite animatedSprite;

        public override void _Ready()
        {
            animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        }
    }
}