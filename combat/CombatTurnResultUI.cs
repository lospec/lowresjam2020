using System;
using System.Threading.Tasks;
using Godot;

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

        public async Task ShowTurnCompare(CombatUtil.CombatAction playerAction,
            CombatUtil.CombatAction enemyAction,
            float duration = 1.5f)
        {
            _playerActionLabel.Text = CombatUtil.GetActionName(playerAction);
            _playerActionLabel.Modulate = CombatUtil.GetActionColor(playerAction);
            _enemyActionLabel.Text = CombatUtil.GetActionName(enemyAction);
            _enemyActionLabel.Modulate = CombatUtil.GetActionColor(enemyAction);
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


        private void SetWinLabel(string actor, CombatUtil.CombatAction action)
        {
            _winActorLabel.Text = actor;
            _winActionLabel.Text = CombatUtil.GetActionName(action);
            _winActionLabel.Modulate = CombatUtil.GetActionColor(action);
        }

        public async void ShowWinResult(CombatUtil.CombatAction playerAction,
            CombatUtil.CombatAction enemyAction,
            float duration = 0)
        {
            if (playerAction == CombatUtil.CombatAction.Flee ||
                enemyAction == CombatUtil.CombatAction.Flee)
            {
                await ToSignal(GetTree(), "idle_frame");
                return;
            }

            var win = CombatUtil.ActionCompare(playerAction, enemyAction);
            switch (win)
            {
                case CombatUtil.TurnOutcome.Tie:
                    SetWinLabel("TIE", playerAction);
                    break;
                case CombatUtil.TurnOutcome.PlayerWin:
                    SetWinLabel("Player Win", playerAction);
                    break;
                case CombatUtil.TurnOutcome.EnemyWin:
                    SetWinLabel("Enemy Win", enemyAction);
                    break;
                default:
                    SetWinLabel("Error", CombatUtil.CombatAction.Invalid);
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