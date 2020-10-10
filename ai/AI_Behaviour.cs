using Godot;

namespace HeroesGuild.ai
{
    public class AI_Behaviour : Resource
    {
        [Export] public int startStateIndex;
        [Export] public AI_State[] states;

        public void SetStartingState(StateMachine stateMachine)
        {
            stateMachine.TransitionToState(startStateIndex, out _);
        }
    }
}