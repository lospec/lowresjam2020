using Godot;

namespace HeroesGuild.AI.Actions
{
    public class AI_Action_ReturnToOrigin : AI_State_Action
    {
        [Export] public float MoveSpeedFactor = 1f;

        public override void Perform(StateMachine stateMachine, float delta, ref bool interrupt)
        {
            var origin = stateMachine.OriginPosition;
            var move = (origin - stateMachine.Entity.Position).Normalized();
            SetMove(stateMachine, move, MoveSpeedFactor, out _);
        }
    }
}