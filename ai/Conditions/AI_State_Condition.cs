using Godot;

namespace HeroesGuild.ai.conditions
{
    public abstract class AI_State_Condition : Resource
    {
        public abstract bool Evaluate(StateMachine stateMachine);
    }
}