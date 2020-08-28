using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace HeroesGuild.Entities.BaseEntity
{
	public class BaseEntity : KinematicBody2D
	{
		private const float RUN_ANIM_MIN_SPEED = 0;


		private static readonly Dictionary<Vector2, string> DirectionToAnimation =
			new Dictionary<Vector2, string>
			{
				{Vector2.Down, RUN_DOWN_ANIM_NAME},
				{Vector2.Up, RUN_UP_ANIM_NAME},
				{Vector2.Right, RUN_SIDE_ANIM_NAME},
				{Vector2.Left, RUN_SIDE_ANIM_NAME}
			};

		private static readonly Dictionary<string, string> RunAnimationsToIdleAnimations =
			new Dictionary<string, string>
			{
				{RUN_DOWN_ANIM_NAME, IDLE_DOWN_ANIM_NAME},
				{RUN_UP_ANIM_NAME, IDLE_UP_ANIM_NAME},
				{RUN_SIDE_ANIM_NAME, IDLE_SIDE_ANIM_NAME},
			};


		private static readonly string[] IdleAnimations =
		{
			IDLE_DOWN_ANIM_NAME, IDLE_UP_ANIM_NAME, IDLE_SIDE_ANIM_NAME
		};


		[Export] public float moveSpeed = 10;

		public Vector2 Velocity { get; set; } = Vector2.Zero;

		protected AnimatedSprite animatedSprite;

		// Animation name constants
		private const string PREMADE_DEFAULT_ANIM_NAME = "default";
		private const string IDLE_DOWN_ANIM_NAME = "idle_down";
		private const string IDLE_SIDE_ANIM_NAME = "idle_side";
		private const string IDLE_UP_ANIM_NAME = "idle_up";
		private const string RUN_DOWN_ANIM_NAME = "run_down";
		private const string RUN_SIDE_ANIM_NAME = "run_side";
		private const string RUN_UP_ANIM_NAME = "run_up";
		private const string DEFAULT_ANIM_NAME = IDLE_DOWN_ANIM_NAME;

		// Frame constants
		private const int OVERWORLD_SPRITE_SIZE_X = 8;
		private const int OVERWORLD_SPRITE_SIZE_Y = 12;
		private const int RUN_DOWN_ANIM_LENGTH = 6;
		private const int RUN_SIDE_ANIM_LENGTH = 6;
		private const int RUN_UP_ANIM_LENGTH = 6;

		public override void _Ready()
		{
			animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

			// Assign new SpriteFrames object
			SpriteFrames frames = new SpriteFrames();
			animatedSprite.Frames = frames;

			// Create animations
			frames.RemoveAnimation(PREMADE_DEFAULT_ANIM_NAME);
			frames.AddAnimation(IDLE_DOWN_ANIM_NAME);
			frames.AddAnimation(IDLE_SIDE_ANIM_NAME);
			frames.AddAnimation(IDLE_UP_ANIM_NAME);
			frames.AddAnimation(RUN_DOWN_ANIM_NAME);
			frames.AddAnimation(RUN_SIDE_ANIM_NAME);
			frames.AddAnimation(RUN_UP_ANIM_NAME);

			frames.SetAnimationSpeed(RUN_DOWN_ANIM_NAME, 10);
			frames.SetAnimationSpeed(RUN_SIDE_ANIM_NAME, 10);
			frames.SetAnimationSpeed(RUN_UP_ANIM_NAME, 10);

			// Load players sprite sheet
			Texture playersTexture = ResourceLoader.Load("res://entities/player/players.png") as Texture;

			// Add idle frames
			frames.AddFrame(IDLE_DOWN_ANIM_NAME, new AtlasTexture
			{
				Atlas = playersTexture,
				Region = new Rect2(0, 0, OVERWORLD_SPRITE_SIZE_X, OVERWORLD_SPRITE_SIZE_Y)
			}, 0);

			frames.AddFrame(IDLE_UP_ANIM_NAME, new AtlasTexture
			{
				Atlas = playersTexture,
				Region = new Rect2(OVERWORLD_SPRITE_SIZE_X, 0, OVERWORLD_SPRITE_SIZE_X, OVERWORLD_SPRITE_SIZE_Y)
			}, 0);

			frames.AddFrame(IDLE_SIDE_ANIM_NAME, new AtlasTexture
			{
				Atlas = playersTexture,
				Region = new Rect2(OVERWORLD_SPRITE_SIZE_X * 2, 0, OVERWORLD_SPRITE_SIZE_X, OVERWORLD_SPRITE_SIZE_Y)
			}, 0);

			// Add run frames
			for (int i = 0; i < RUN_DOWN_ANIM_LENGTH; i++)
			{
				frames.AddFrame(RUN_DOWN_ANIM_NAME, new AtlasTexture
				{
					Atlas = playersTexture,
					Region = new Rect2(OVERWORLD_SPRITE_SIZE_X * i, OVERWORLD_SPRITE_SIZE_Y, OVERWORLD_SPRITE_SIZE_X, OVERWORLD_SPRITE_SIZE_Y)
				}, i);
			}

			for (int i = 0; i < RUN_SIDE_ANIM_LENGTH; i++)
			{
				frames.AddFrame(RUN_SIDE_ANIM_NAME, new AtlasTexture
				{
					Atlas = playersTexture,
					Region = new Rect2(OVERWORLD_SPRITE_SIZE_X * i, OVERWORLD_SPRITE_SIZE_Y * 2, OVERWORLD_SPRITE_SIZE_X, OVERWORLD_SPRITE_SIZE_Y)
				}, i);
			}

			for (int i = 0; i < RUN_UP_ANIM_LENGTH; i++)
			{
				frames.AddFrame(RUN_UP_ANIM_NAME, new AtlasTexture
				{
					Atlas = playersTexture,
					Region = new Rect2(OVERWORLD_SPRITE_SIZE_X * i, OVERWORLD_SPRITE_SIZE_Y * 3, OVERWORLD_SPRITE_SIZE_X, OVERWORLD_SPRITE_SIZE_Y)
				}, i);
			}

			// Play default animation
			animatedSprite.Play(DEFAULT_ANIM_NAME);
		}

		public override void _PhysicsProcess(float delta)
		{
			Movement();
			Animate();
		}


		private void Movement()
		{
			Velocity = MoveAndSlide(Velocity);
		}

		private void Animate()
		{
			if (Velocity.Length() > 0)
			{
				animatedSprite.FlipH = Velocity.x < 0;
			}

			var newAnim = animatedSprite.Animation;

			if (Velocity.Length() > RUN_ANIM_MIN_SPEED)
			{
				var animDir = Velocity.Normalized();
				if (Mathf.Abs(animDir.x) > RUN_ANIM_MIN_SPEED && Mathf.Abs(animDir.y) > RUN_ANIM_MIN_SPEED)
				{
					animDir = (Mathf.Abs(animDir.y) >= Mathf.Abs(animDir.x)
						? new Vector2(0, animDir.y)
						: new Vector2(animDir.x, 0)).Normalized();
				}

				newAnim = DirectionToAnimation[animDir];
			}
			else if (!IdleAnimations.Contains(newAnim))
			{
				newAnim = RunAnimationsToIdleAnimations[newAnim];
			}

			animatedSprite.Play(newAnim);
		}
	}
}
