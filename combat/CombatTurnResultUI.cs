using System;
using System.Threading.Tasks;
using Godot;
using HeroesGuild.combat.combat_actions;

namespace HeroesGuild.combat
{
    public class CombatTurnResultUI : MarginContainer
    {
        private Label _enemyActionLabel;
        private Label _lineLabel;
        private Label _playerActionLabel;

        private Tween _tween;
        private Label _winActionLabel;
        private Label _winActorLabel;
        [Export] public float actionTweenDelay = 1f;
        [Export] public float actionTweenDuration = 1f;
        public HBoxContainer compareContainer;
        public VBoxContainer winContainer;

        public override void _Ready()
        {
            _tween = GetNode<Tween>("Tween");
            compareContainer = GetNode<HBoxContainer>("CompareContainer");
            _lineLabel = GetNode<Label>("LineLabel");
            winContainer = GetNode<VBoxContainer>("WinResultContainer");
            _playerActionLabel =
                GetNode<Label>("CompareContainer/PlayerTurnContainer/ActionLabel");
            _enemyActionLabel =
                GetNode<Label>("CompareContainer/EnemyTurnContainer/ActionLabel");
            _winActorLabel = GetNode<Label>("WinResultContainer/ActorLabel");
            _winActionLabel = GetNode<Label>("WinResultContainer/ActionLabel");
        }

        public async Task ShowTurnCompare(BaseCombatAction playerAction,
            BaseCombatAction enemyAction,
            float duration = 1.5f)
        {
            _playerActionLabel.Text = playerAction.ActionName;
            _playerActionLabel.Modulate = playerAction.ActionColor;
            _enemyActionLabel.Text = enemyAction.ActionName;
            _enemyActionLabel.Modulate = enemyAction.ActionColor;
            _lineLabel.Visible = true;
            compareContainer.Visible = true;

            if (duration > 0)
            {
                var color = _playerActionLabel.Modulate;
                color.a = 0;
                _playerActionLabel.Modulate = color;
                color = _enemyActionLabel.Modulate;
                color.a = 0;
                _enemyActionLabel.Modulate = color;

                _tween.InterpolateProperty(_enemyActionLabel, "modulate:a", 0f, 1f,
                    actionTweenDuration, Tween.TransitionType.Quad, Tween.EaseType.Out,
                    actionTweenDelay);
                _tween.InterpolateProperty(_playerActionLabel, "modulate:a", 0f, 1f,
                    actionTweenDuration, Tween.TransitionType.Quad, Tween.EaseType.Out,
                    actionTweenDelay);
                _tween.Start();

                await ToSignal(_tween, "tween_all_completed");
                await ToSignal(GetTree().CreateTimer(duration), "timeout");

                _lineLabel.Visible = false;
                compareContainer.Visible = false;
            }
        }


        private void SetWinLabel(string actor, CombatAction action)
        {
            _winActorLabel.Text = actor;
            _winActionLabel.Text = action.ActionName;
            _winActionLabel.Modulate = action.ActionColor;
        }

        public async void ShowWinResult(BaseCombatAction playerAction,
            BaseCombatAction enemyAction, float duration = 0)
        {
            if (playerAction is FleeAction || enemyAction is FleeAction)
            {
                await ToSignal(GetTree(), "idle_frame");
            }
            else
            {
                ShowWinResult((CombatAction) playerAction, (CombatAction) enemyAction,
                    duration);
            }
        }

        private async void ShowWinResult(CombatAction playerAction,
            CombatAction enemyAction, float duration)
        {
            var turnOutcome = CombatAction.CompareActions(playerAction, enemyAction);
            switch (turnOutcome)
            {
                case TurnOutcome.Tie:
                    SetWinLabel("TIE", playerAction);
                    break;
                case TurnOutcome.PlayerWin:
                    SetWinLabel("Player Win", playerAction);
                    break;
                case TurnOutcome.EnemyWin:
                    SetWinLabel("Enemy Win", enemyAction);
                    break;
                default:
                    SetWinLabel("Error", null);
                    throw new ArgumentOutOfRangeException();
            }

            winContainer.Visible = true;
            if (duration > 0)
            {
                await ToSignal(GetTree().CreateTimer(duration), "timeout");
                winContainer.Visible = false;
            }
        }
    }
}