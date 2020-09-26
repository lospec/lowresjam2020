using Godot;

namespace HeroesGuild.AI
{
    public class AI_Behaviour : Resource
    {
        [Export] public AI_State[] states;
        [Export] public int startStateIndex;

        public void SetStartingState(StateMachine stateMachine)
        {
            stateMachine.TransitionToState(startStateIndex, out _);
        }
    }
}