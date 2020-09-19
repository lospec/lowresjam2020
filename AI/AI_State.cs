using Godot;
using HeroesGuild.AI.Actions;

namespace HeroesGuild.AI
{
    public class AI_State : Resource
    {
        [Export] public AI_State_Action[] actions;
        [Export] public AI_Transition[] transitions;

        public AI_State Instance()
        {
            var state = new AI_State
            {
                actions = new AI_State_Action[actions.Length],
                transitions = new AI_Transition[transitions.Length]
            };
            for (var i = 0; i < state.actions.Length; i++)
            {
                state.actions[i] = (AI_State_Action) actions[i].Duplicate();
            }

            for (var i = 0; i < transitions.Length; i++)
            {
                state.transitions[i] = (AI_Transition) transitions[i].Duplicate();
            }

            return state;
        }

        public void UpdateState(StateMachine stateMachine, float delta)
        {
            PerformActions(stateMachine, delta);
            CheckAllTransitions(stateMachine);
        }

        public void OnStart(StateMachine stateMachine)
        {
            foreach (var action in actions)
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
            foreach (var action in actions)
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
            foreach (var transition in transitions)
            {
                var stateIndex = transition.condition.Evaluate(stateMachine)
                    ? transition.trueStateIndex
                    : transition.falseStateIndex;

                stateMachine.TransitionToState(stateIndex, out var result);
                if (result)
                {
                    return;
                }
            }
        }
    }
}