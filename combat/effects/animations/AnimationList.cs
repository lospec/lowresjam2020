using System;
using System.Collections.Generic;
using Godot;
using HeroesGuild.Utility;

namespace HeroesGuild.Combat.Effects.Animations
{
    public class AnimationList : Node
    {
        private readonly Dictionary<string, AnimatedTexture> _attackAnimations =
            new Dictionary<string, AnimatedTexture>();

        public override void _Ready()
        {
            var file = new File();

            foreach (WeaponUtil.DamageType type in Enum.GetValues(
                typeof(WeaponUtil.DamageType)))
            {
                var name = WeaponUtil.GetDamageTypeName(type).ToLower();
                var path =
                    $"res://combat/effects/animations/attack_animation_{name}.tres";
                if (file.FileExists(path))
                {
                    _attackAnimations.Add(name, GD.Load<AnimatedTexture>(path));
                }
            }

            _attackAnimations.Add("counter",
                GD.Load<AnimatedTexture>(
                    "res://Combat/Effects/animations/counter_animation.tres"));
        }

        public AnimatedTexture GetAnimation(WeaponUtil.DamageType type)
        {
            var key = WeaponUtil.GetDamageTypeName(type).ToLower();
            return GetAnimation(key);
        }

        public AnimatedTexture GetAnimation(string key)
        {
            key = key.ToLower();
            if (_attackAnimations.ContainsKey(key))
            {
                return _attackAnimations[key];
            }

            throw new ArgumentOutOfRangeException($"{key} not found");
        }
    }
}