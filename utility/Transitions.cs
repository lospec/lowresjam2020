using System;
using System.Threading.Tasks;
using Godot;

namespace HeroesGuild.utility
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

        private ShaderMaterial _transitionShader;

        private TextureRect _transitionTextureRect;
        private Tween _tween;

        private static Texture GetTransitionTextures(TransitionType transitionType)
        {
            var path = transitionType switch
            {
                TransitionType.ShrinkingCircle =>
                    "res://transitions/shrinking_circle.png",
                TransitionType.MultipleSquares =>
                    "res://transitions/multiple_squares.png",
                TransitionType.MultipleCirclesFilled =>
                    "res://transitions/multiple_circles_filled.png",
                TransitionType.Lines => "res://transitions/lines.png",
                TransitionType.Swirl => "res://transitions/swirl.png",
                TransitionType.Blocks => "res://transitions/blocks.png",
                _ => throw new ArgumentException()
            };
            return ResourceLoader.Load<Texture>(path);
        }

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
                GD.PrintErr(
                    $"Error {error} occured while attempting to change to the {scenePath} scene.");
            }

            StartTransition(@params);
        }

        public async Task ChangeSceneDoubleTransition(string scenePath,
            TransitionParams @params)
        {
            var invertParams = @params;
            invertParams.invert = !@params.invert;
            StartTransition(invertParams);
            await ToSignal(_tween, "tween_completed");
            ChangeScene(scenePath, @params);
        }

        public void StartTransition(TransitionParams @params)
        {
            _transitionShader.SetShaderParam("mask_tex",
                GetTransitionTextures(@params.transitionType));
            _transitionShader.SetShaderParam("transition_invert_mask", @params.invert);
            _transitionShader.SetShaderParam("transition_col", @params.color);
            _transitionShader.SetShaderParam("transition_feather",
                @params.transitionFeather);
            _transitionShader.SetShaderParam("transition_time", 1);
            _tween.InterpolateProperty(_transitionShader,
                "shader_param/transition_time", 1, 0, @params.duration, @params
                    .tweenTransitionType, @params.tweenEaseType);
            _tween.Start();
        }

        public struct TransitionParams
        {
            public TransitionType transitionType;
            public float duration;
            public bool invert;
            public Color color;
            public float transitionFeather;
            public Tween.TransitionType tweenTransitionType;
            public Tween.EaseType tweenEaseType;

            public TransitionParams(TransitionType transitionType, float duration,
                bool invert = false, Color color =
                    default, float transitionFeather = 0.2f,
                Tween.TransitionType tweenTransitionType = Tween.TransitionType
                    .Linear, Tween.EaseType tweenEaseType = Tween.EaseType.InOut)
            {
                this.transitionType = transitionType;
                this.duration = duration;
                this.invert = invert;
                this.color = color == default ? Colors.Black : color;
                this.transitionFeather = transitionFeather;
                this.tweenTransitionType = tweenTransitionType;
                this.tweenEaseType = tweenEaseType;
            }
        }
    }
}