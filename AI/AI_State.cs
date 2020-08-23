using Godot;
using HeroesGuild.AI.Actions;

namespace HeroesGuild.AI
{
    public class AI_State : Node
    {
        [Export] public AI_State_Action[] Actions;
        [Export] public AI_Transition[] Transitions;

        public AI_State Instance()
        {
            var state = new AI_State
            {
                Actions = new AI_State_Action[Actions.Length],
                Transitions = new AI_Transition[Transitions.Length]
            };
            for (var i = 0; i < state.Actions.Length; i++)
            {
                state.Actions[i] = (AI_State_Action) Actions[i].Duplicate();
            }

            for (var i = 0; i < Transitions.Length; i++)
            {
                state.Transitions[i] = (AI_Transition) Transitions[i].Duplicate();
            }

            return state;
        }

        public void UpdateState(StateMachine stateMachine, float delta)
        {
        }

        public void OnStart(StateMachine stateMachine)
        {
            foreach (var action in Actions)
            {
                if (action == null || action.IsInitialized)
                {
                    continue;
                }

                action.ResourceLocalToScene = true;
                action.OnStart(stateMachine);
            }
        }

        private void PerformActions(StateMachine stateMachine, float delta)
        {
            var interrupt = false;
            foreach (var action in Actions)
            {
                action.Perform(stateMachine, delta, ref interrupt);
                if (interrupt)
                {
                    return;
                }
            }
        }

        private void CheckAllTransitions(StateMachine stateMachine)
        {
            foreach (var transition in Transitions)
            {
                var stateIndex = transition.Condition.Evaluate(stateMachine)
                    ? transition.TrueStateIndex
                    : transition.FalseStateIndex;

                stateMachine.TransitionToState(stateIndex, out var result);
                if (result)
                {
                    return;
                }
            }
        }
    }
}