using System;
using System.Collections.Generic;
using Godot;
using HeroesGuild.utility;

namespace HeroesGuild.combat.Effects.animations
{
    public class AnimationList : Node
    {
        private const string AttackAnimationPath =
            "res://combat/effects/animations/attack_animation_{0}.tres";
        private const string CounterAnimationPath =
            "res://combat/effects/animations/counter_animation.tres";

        private readonly Dictionary<string, AnimatedTexture> _attackAnimations =
            new Dictionary<string, AnimatedTexture>();

        public override void _Ready()
        {
            var file = new File();

            foreach (WeaponUtil.DamageType type in Enum.GetValues(
                typeof(WeaponUtil.DamageType)))
            {
                var name = WeaponUtil.GetDamageTypeName(type).ToLower();
                var path = string.Format(AttackAnimationPath, name);
                if (file.FileExists(path))
                    _attackAnimations.Add(name, GD.Load<AnimatedTexture>(path));
            }

            _attackAnimations.Add("counter",
                GD.Load<AnimatedTexture>(CounterAnimationPath));
        }

        public AnimatedTexture GetAnimation(WeaponUtil.DamageType type)
        {
            var key = WeaponUtil.GetDamageTypeName(type).ToLower();
            return GetAnimation(key);
        }

        public AnimatedTexture GetAnimation(string key)
        {
            key = key.ToLower();
            if (_attackAnimations.ContainsKey(key)) return _attackAnimations[key];

            throw new ArgumentOutOfRangeException($"{key} not found");
        }
    }
}