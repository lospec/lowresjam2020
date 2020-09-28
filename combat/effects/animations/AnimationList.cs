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

        private readonly Dictionary<DamageType, AnimatedTexture> _attackAnimations =
            new Dictionary<DamageType, AnimatedTexture>();

        private AnimatedTexture _counterAnimation;

        public override void _Ready()
        {
            var file = new File();

            foreach (DamageType type in Enum.GetValues(
                typeof(DamageType)))
            {
                var name = WeaponUtil.GetDamageTypeName(type).ToLower();
                var path = string.Format(AttackAnimationPath, name);
                if (file.FileExists(path))
                    _attackAnimations.Add(type, GD.Load<AnimatedTexture>(path));
            }

            _counterAnimation = GD.Load<AnimatedTexture>(CounterAnimationPath);
        }

        public AnimatedTexture GetAnimation(DamageType type)
        {
            if (_attackAnimations.ContainsKey(type))
            {
                return _attackAnimations[type];
            }

            throw new ArgumentOutOfRangeException($"{type}");
        }

        public AnimatedTexture GetCounterAnimation()
        {
            return _counterAnimation;
        }
    }
}