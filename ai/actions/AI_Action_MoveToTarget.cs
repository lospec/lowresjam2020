using Godot;

namespace HeroesGuild.ai.actions
{
    public class AI_Action_MoveToTarget : AI_State_Action
    {
        [Export] public float moveSpeedFactor = 1f;

        public override void Perform(StateMachine stateMachine, float delta,
            ref bool interrupt)
        {
            var target = stateMachine.Target;
            if (target == null) return;

            var move = (target.Position - stateMachine.Entity.Position).Normalized();
            SetMove(stateMachine, move, moveSpeedFactor, out _);
        }
    }
}