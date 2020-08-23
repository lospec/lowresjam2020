using Godot;

namespace HeroesGuild.AI.Conditions
{
    public class AI_Condition_FarFromOrigin : AI_State_Condition
    {
        [Export] public float DistanceFromOrigin;

        public override bool Evaluate(StateMachine stateMachine)
        {
            return stateMachine.DistanceToOrigin > DistanceFromOrigin;
        }
    }
}