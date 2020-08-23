using Godot;

namespace HeroesGuild.AI.Conditions
{
    public abstract class AI_State_Condition : Node
    {
        public abstract bool Evaluate(StateMachine stateMachine);
    }
}