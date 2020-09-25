using Godot;

namespace HeroesGuild.AI.Actions
{
    public class AI_Action_RunFromTarget : AI_State_Action
    {
        [Export] public float moveSpeedFactor = 1f;

        public override void Perform(StateMachine stateMachine, float delta, ref bool interrupt)
        {
            var target = stateMachine.Target;
            if (target == null)
            {
                return;
            }

            var move = (stateMachine.Entity.Position - target.Position).Normalized();
            SetMove(stateMachine, move, moveSpeedFactor, out _);
        }
    }
}