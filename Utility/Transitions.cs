using System;
using System.Threading.Tasks;
using Godot;

namespace HeroesGuild.Utility
{
	public class Transitions : CanvasLayer
	{
		public enum TransitionType
		{
			ShrinkingCircle,
			MultipleSquares,
			MultipleCirclesFilled,
			Lines,
			Swirl,
			Blocks
		}

		public static Texture GetTransitionTextures(TransitionType transitionType)
		{
			var path = transitionType switch
			{
				TransitionType.ShrinkingCircle => "res://Transitions/shrinking_circle.png",
				TransitionType.MultipleSquares => "res://Transitions/multiple_squares.png",
				TransitionType.MultipleCirclesFilled => "res://Transitions/multiple_circles_filled.png",
				TransitionType.Lines => "res://Transitions/lines.png",
				TransitionType.Swirl => "res://Transitions/swirl.png",
				TransitionType.Blocks => "res://Transitions/blocks.png",
				_ => throw new ArgumentException()
			};
			return ResourceLoader.Load<Texture>(path);
		}

		public struct TransitionParams
		{
			public TransitionType TransitionType;
			public float Duration;
			public bool Invert;
			public Color Color;
			public float TransitionFeather;
			public Tween.TransitionType TweenTransitionType;
			public Tween.EaseType TweenEaseType;

			public TransitionParams(TransitionType transitionType, float duration, bool invert = false, Color color =
				default, float transitionFeather = 0.2f, Tween.TransitionType tweenTransitionType = Tween.TransitionType
				.Linear, Tween.EaseType tweenEaseType = Tween.EaseType.InOut)
			{
				TransitionType = transitionType;
				Duration = duration;
				Invert = invert;
				Color = color == default ? Colors.Black : color;
				TransitionFeather = transitionFeather;
				TweenTransitionType = tweenTransitionType;
				TweenEaseType = tweenEaseType;
			}
		}

		private TextureRect _transitionTextureRect;
		private ShaderMaterial _transitionShader;
		private Tween _tween;

		public override void _Ready()
		{
			_transitionTextureRect = GetNode<TextureRect>("Transition");
			_transitionShader = (ShaderMaterial) _transitionTextureRect.Material;
			_tween = GetNode<Tween>("Tween");
		}

		public void ChangeScene(string scenePath, TransitionParams @params)
		{
			var error = GetTree().ChangeScene(scenePath);
			if (error != Error.Ok)
			{
				GD.PrintErr($"Error {error} occured while attempting to change scene.");
			}

			StartTransition(@params);
		}

		public async Task ChangeSceneDoubleTransition(string scenePath, TransitionParams @params)
		{
			var invertParams = @params;
			invertParams.Invert = !@params.Invert;
			StartTransition(invertParams);
			await ToSignal(_tween, "tween_completed");
			ChangeScene(scenePath, @params);
		}

		public void StartTransition(TransitionParams @params)
		{
			_transitionShader.SetShaderParam("mask_tex", GetTransitionTextures(@params.TransitionType));
			_transitionShader.SetShaderParam("transition_invert_mask", @params.Invert);
			_transitionShader.SetShaderParam("transition_col", @params.Color);
			_transitionShader.SetShaderParam("transition_feather", @params.TransitionFeather);
			_transitionShader.SetShaderParam("transition_time", 1);
			_tween.InterpolateProperty(_transitionShader, "shader_param/transition_time", 1, 0, @params.Duration, @params
				.TweenTransitionType, @params.TweenEaseType);
			_tween.Start();
		}
	}
}
