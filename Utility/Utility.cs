using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HeroesGuild.Utility
{
    public static class Utility
    {
        static Utility()
        {
            GD.Randomize();
        }

        public static T RandomElement<T>(this IEnumerable<T> list)
        {
            return RandomElement(list.ToArray());
        }

        public static T RandomElement<T>(this IList<T> list)
        {
            var idx = (int) (GD.Randi() % list.Count);
            return list[idx];
        }

        public static Texture GetInventoryItemResource(string itemName)
        {
            return GD.Load<Texture>($"res://items/inventory_sprites/{itemName.ToLower().Replace(" ", "_")}.png");
        }
    }
}