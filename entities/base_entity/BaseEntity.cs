using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HeroesGuild.data;
using HeroesGuild.status_effects;
using HeroesGuild.status_effects.effects;
using HeroesGuild.utility;

namespace HeroesGuild.entities.base_entity
{
    public class BaseEntity : KinematicBody2D
    {
        [Signal] public delegate void HealthChanged(int oldHealth, int newHealth);

        public enum Animations
        {
            IdleDown,
            IdleUp,
            IdleSide,
            RunDown,
            RunSide,
            RunUp
        }

        private const float RUN_ANIM_MIN_SPEED = 0;

        private static readonly Dictionary<Vector2, Animations> DirectionToAnimation =
            new Dictionary<Vector2, Animations>
            {
                {Vector2.Down, Animations.RunDown},
                {Vector2.Up, Animations.RunUp},
                {Vector2.Right, Animations.RunSide},
                {Vector2.Left, Animations.RunSide}
            };

        private static readonly Dictionary<Animations, Animations>
            RunAnimationsToIdleAnimations =
                new Dictionary<Animations, Animations>
                {
                    {Animations.RunDown, Animations.IdleDown},
                    {Animations.RunUp, Animations.IdleUp},
                    {Animations.RunSide, Animations.IdleSide}
                };

        private static readonly Animations[] IdleAnimations =
        {
            Animations.IdleDown, Animations.IdleUp, Animations.IdleSide
        };
        private AnimationPlayer _animationPlayer;

        private Vector2 _currentPosition;

        private int _health;
        private Vector2 _oldPosition;
        public int animationFrame = 0;
        public Animations currentAnimation = Animations.IdleDown;

        [Export] public int maxHealth = 10;
        [Export] public float moveSpeed = 10;
        public Vector2 realVelocity = Vector2.Zero;

        protected Sprite sprite;
        public Dictionary<string, StatusEffect> statusEffects =
            new Dictionary<string, StatusEffect>();

        public Vector2 Velocity { get; set; } = Vector2.Zero;


        public int Health
        {
            get => _health;
            set
            {
                var oldHealth = _health;
                _health = value;
                EmitSignal(nameof(HealthChanged), oldHealth, value);
            }
        }

        private static string GetAnimationName(Animations animation)
        {
            return animation switch
            {
                Animations.IdleDown => "idle_down",
                Animations.IdleUp => "idle_up",
                Animations.IdleSide => "idle_side",
                Animations.RunDown => "run_down",
                Animations.RunSide => "run_side",
                Animations.RunUp => "run_up",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void _Ready()
        {
            sprite = GetNode<Sprite>("Sprite");
            _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            if (maxHealth <= 0) return;

            Health = maxHealth;
        }

        public override void _PhysicsProcess(float delta)
        {
            Movement();
            Animate();
        }


        private void Movement()
        {
            _oldPosition = Position;
            realVelocity = MoveAndSlide(Velocity);
            var slideCount = GetSlideCount();
            if (slideCount >= 2)
            {
                var normal = Vector2.Zero;
                for (var i = 0; i < slideCount; i++)
                {
                    var collision = GetSlideCollision(i);
                    normal += collision.Normal;
                }

                if (Mathf.Abs(normal.x) > 0.3f && Mathf.Abs(normal.y) > 0.3)
                {
                    var pos = Position;
                    if (Mathf.Sign(Velocity.x) == Mathf.Sign(-normal.x))
                        pos.x = Mathf.Round(pos.x);

                    if (Mathf.Sign(Velocity.y) == Mathf.Sign(-normal.y))
                        pos.y = Mathf.Round(pos.y);

                    Position = pos;
                }
            }

            if (Mathf.Abs(realVelocity.x) > 0 && Mathf.Abs(realVelocity.y) > 0 &&
                !(Get("collision_detector") != null &&
                  ((Area2D) Get("collision_detector")).GetOverlappingBodies().Count >
                  0))
            {
                var pos = Position;
                if (Mathf.Abs(_oldPosition.x - pos.x) >
                    Mathf.Abs(_oldPosition.y - pos.y))
                {
                    _currentPosition.x = Mathf.Round(pos.x);
                    _currentPosition.y = Mathf.Round(pos.y +
                                                     (_currentPosition.x - pos.x) *
                                                     realVelocity
                                                         .y / realVelocity.x);
                    pos.y = _currentPosition.y;
                }
                else if (Mathf.Abs(_oldPosition.x - pos.x) <=
                         Mathf.Abs(_oldPosition.y - pos.y))
                {
                    _currentPosition.y = Mathf.Round(pos.y);
                    _currentPosition.x = Mathf.Round(pos.x +
                                                     (_currentPosition.y - pos.y) *
                                                     realVelocity.x /
                                                     realVelocity.y);
                    pos.x = _currentPosition.x;
                }

                Position = pos;
            }
        }

        private float CalculateWaitTime(float fps)
        {
            return 1f / fps;
        }

        private void Animate()
        {
            if (Velocity.Length() > 0) sprite.FlipH = Velocity.x < 0;

            var oldAnimation = currentAnimation;
            if (Velocity.Length() > RUN_ANIM_MIN_SPEED)
            {
                var animDir = Velocity.Normalized();
                if (Mathf.Abs(animDir.x) > RUN_ANIM_MIN_SPEED &&
                    Mathf.Abs(animDir.y) > RUN_ANIM_MIN_SPEED)
                    animDir = (Mathf.Abs(animDir.y) >= Mathf.Abs(animDir.x)
                        ? new Vector2(0, animDir.y)
                        : new Vector2(animDir.x, 0)).Normalized();

                currentAnimation = DirectionToAnimation[animDir];
            }
            else if (!IdleAnimations.Contains(currentAnimation))
            {
                currentAnimation = RunAnimationsToIdleAnimations[currentAnimation];
            }

            if (currentAnimation != oldAnimation)
                _animationPlayer.Play(GetAnimationName(currentAnimation));
        }

        private void AnimationPlayFootstep(int sfxOffset)
        {
            if (IsInGroup("player"))
			{
				if (sfxOffset == 0)
				{
					AudioSystem.PlaySFX("PlayerFootstep", Position);
				}
				else
				{
					AudioSystem.PlaySFX("PlayerFootstepAlt", Position);
				}
			}
        }


        public void AddStatusEffect(string statusEffectName)
        {
            StatusEffect statusEffect = statusEffectName switch
            {
                "Asleep" => new Asleep(),
                "Confused" => new Confused(),
                "Frozen" => new Frozen(),
                "OnFire" => new OnFire(),
                "Poison" => new Poison(),
                "Weak" => new Weak(),
                _ => throw new ArgumentOutOfRangeException()
            };

            if (statusEffectName == "OnFire" && statusEffects.ContainsKey("Frozen"))
            {
                statusEffects.Remove("Frozen");
                return;
            }

            if (statusEffectName == "Frozen" && statusEffects.ContainsKey("OnFire"))
            {
                statusEffects.Remove("OnFire");
                return;
            }

            if (statusEffects.ContainsKey(statusEffect.statusEffectName))
                statusEffects.Remove(statusEffect.statusEffectName);

            statusEffects.Add(statusEffect.statusEffectName, statusEffect);
        }
    }
}