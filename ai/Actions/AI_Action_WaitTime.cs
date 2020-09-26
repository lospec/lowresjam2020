using Godot;

namespace HeroesGuild.ai.actions
{
    public class AI_Action_WaitTime : AI_State_Action
    {
        private float _timer = 0f;
        [Export] public float activePeriod = 1f;
        [Export] public float waitPeriod = 1f;

        public override void Perform(StateMachine stateMachine, float delta,
            ref bool interrupt)
        {
            _timer += delta;
            if (_timer >= waitPeriod && _timer > 0)
            {
                _timer = -activePeriod;
                return;
            }

            interrupt = true;
        }
    }
}