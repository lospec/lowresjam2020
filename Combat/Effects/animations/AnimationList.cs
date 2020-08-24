using System;
using System.Collections.Generic;
using Godot;
using HeroesGuild.Utility;

namespace HeroesGuild.Combat.Effects.animations
{
    public class AnimationList : Node
    {
        private Dictionary<string, AnimatedTexture> AttackAnimations = new Dictionary<string, AnimatedTexture>();

        public override void _Ready()
        {
            var file = new File();

            foreach (WeaponUtil.DamageType type in Enum.GetValues(typeof(WeaponUtil.DamageType)))
            {
                var name = WeaponUtil.GetDamageTypeName(type).ToLower();
                var path = $"res://Combat/Effects/animations/attack_animation_{name}.tres";
                if (file.FileExists(path))
                {
                    AttackAnimations.Add(name, GD.Load<AnimatedTexture>(path));
                }
            }

            AttackAnimations.Add("counter",
                GD.Load<AnimatedTexture>("res://Combat/Effects/animations/counter_animation.tres"));
        }

        public AnimatedTexture GetAnimation(WeaponUtil.DamageType type)
        {
            var key = WeaponUtil.GetDamageTypeName(type).ToLower();
            return GetAnimation(key);
        }

        public AnimatedTexture GetAnimation(string key)
        {
            if (AttackAnimations.ContainsKey(key))
            {
                return AttackAnimations[key];
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}