using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HeroesGuild.utility
{
    public static class Utility
    {
        private const string InventorySpritePath =
            "res://items/inventory_sprites/{0}.png";

        private const string OverWorldSprite =
            "res://entities/player/spritesheets/{0}_overworld.png";

        public static readonly Random Random = new Random();

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
            var idx = (int) Mathf.PosMod(GD.Randi(), list.Count);
            return list[idx];
        }

        public static Texture GetInventoryItemResource(string itemName)
        {
            return GD.Load<Texture>(string.Format(InventorySpritePath,
                itemName.ToLower().Replace(" ", "_")));
        }

        public static Texture GetPlayerResource(string playerName)
        {
            return ResourceLoader.Load<Texture>(string.Format(OverWorldSprite,
                playerName.ToLower()));
        }
    }
}