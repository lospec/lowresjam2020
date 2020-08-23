using Godot;

namespace HeroesGuild.AI
{
    public class AI_Behaviour : Node
    {
        [Export] public AI_State[] States;
        [Export] public int StartStateIndex;

        public void SetStartingState(StateMachine stateMachine)
        {
            stateMachine.TransitionToState(StartStateIndex, out _);
        }
    }
}