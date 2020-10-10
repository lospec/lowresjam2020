using Godot;

namespace HeroesGuild.ai.conditions
{
    public class AI_Condition_FarFromOrigin : AI_State_Condition
    {
        [Export] public float distanceFromOrigin;

        public override bool Evaluate(StateMachine stateMachine)
        {
            return stateMachine.DistanceToOrigin > distanceFromOrigin;
        }
    }
}