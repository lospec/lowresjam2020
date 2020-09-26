using System.Collections.Generic;
using Godot;

namespace HeroesGuild.guild_hall.chest
{
    public class Chest : StaticBody2D
    {
        public AnimatedSprite animatedSprite;
        public int chestID;
        public Dictionary<int, string> contents;

        public override void _Ready()
        {
            animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        }
    }
}