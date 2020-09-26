using Godot;

namespace HeroesGuild.ai.actions
{
    public class AI_Action_ReturnToOrigin : AI_State_Action
    {
        [Export] public float moveSpeedFactor = 1f;

        public override void Perform(StateMachine stateMachine, float delta,
            ref bool interrupt)
        {
            var origin = stateMachine.OriginPosition;
            var move = (origin - stateMachine.Entity.Position).Normalized();
            SetMove(stateMachine, move, moveSpeedFactor, out _);
        }
    }
}