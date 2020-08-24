using System;
using System.Threading.Tasks;
using Godot;

namespace HeroesGuild.Combat
{
    public class CombatTurnResultUI : MarginContainer
    {
        [Export] public float ActionTweenDelay = 1f;
        [Export] public float ActionTweenDuration = 1f;

        private Tween tween;
        public HBoxContainer compareContainer;
        private Label lineLabel;
        public VBoxContainer winContainer;
        private Label playerActionLabel;
        private Label enemyActionLabel;
        private Label winActorLabel;
        private Label winActionLabel;

        public override void _Ready()
        {
            tween = GetNode<Tween>("Tween");
            compareContainer = GetNode<HBoxContainer>("CompareContainer");
            lineLabel = GetNode<Label>("LineLabel");
            winContainer = GetNode<VBoxContainer>("WinResultContainer");
            playerActionLabel = GetNode<Label>("CompareContainer/PlayerTurnContainer/ActionLabel");
            enemyActionLabel = GetNode<Label>("CompareContainer/EnemyTurnContainer/ActionLabel");
            winActorLabel = GetNode<Label>("WinResultContainer/ActorLabel");
            winActionLabel = GetNode<Label>("WinResultContainer/ActionLabel");
        }

        public async Task ShowTurnCompare(CombatUtil.CombatAction playerAction, CombatUtil.CombatAction enemyAction,
            float duration = 1.5f)
        {
            playerActionLabel.Text = CombatUtil.GetActionName(playerAction);
            playerActionLabel.Modulate = CombatUtil.GetActionColor(playerAction);
            enemyActionLabel.Text = CombatUtil.GetActionName(enemyAction);
            enemyActionLabel.Modulate = CombatUtil.GetActionColor(enemyAction);
            lineLabel.Visible = true;
            compareContainer.Visible = true;

            if (duration > 0)
            {
                var color = playerActionLabel.Modulate;
                color.a = 0;
                playerActionLabel.Modulate = color;
                color = enemyActionLabel.Modulate;
                color.a = 0;
                enemyActionLabel.Modulate = color;

                tween.InterpolateProperty(enemyActionLabel, "modulate:a", 0f, 1f, ActionTweenDuration, Tween
                    .TransitionType.Quad, Tween.EaseType.Out, ActionTweenDelay);
                tween.InterpolateProperty(playerActionLabel, "modulate:a", 0f, 1f, ActionTweenDuration, Tween
                    .TransitionType.Quad, Tween.EaseType.Out, ActionTweenDelay);
                tween.Start();

                await ToSignal(tween, "tween_all_completed");
                await ToSignal(GetTree().CreateTimer(duration), "timeout");

                lineLabel.Visible = false;
                compareContainer.Visible = false;
            }
        }


        private void SetWinLabel(string actor, CombatUtil.CombatAction action)
        {
            winActorLabel.Text = actor;
            winActionLabel.Text = CombatUtil.GetActionName(action);
            winActionLabel.Modulate = CombatUtil.GetActionColor(action);
        }

        public async void ShowWinResult(CombatUtil.CombatAction playerAction, CombatUtil.CombatAction enemyAction,
            float duration = 0)
        {
            if (playerAction == CombatUtil.CombatAction.Flee || enemyAction == CombatUtil.CombatAction.Flee)
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