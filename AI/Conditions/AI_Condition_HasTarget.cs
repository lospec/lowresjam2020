using Godot;

namespace HeroesGuild.AI.Conditions
{
    public class AI_Condition_HasTarget : AI_State_Condition
    {
        [Export] public bool HasTarget;

        public override bool Evaluate(StateMachine stateMachine)
        {
            return HasTarget ? stateMachine.Target != null : stateMachine.Target == null;
        }
    }
}