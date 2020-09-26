using Godot;

namespace HeroesGuild.AI.Actions
{
    public class AI_Action_WaitTime : AI_State_Action
    {
        [Export] public float waitPeriod = 1f;
        [Export] public float activePeriod = 1f;

        private float _timer = 0f;

        public override void Perform(StateMachine stateMachine, float delta, ref bool interrupt)
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