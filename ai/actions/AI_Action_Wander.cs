using Godot;

namespace HeroesGuild.ai.actions
{
    public class AI_Action_Wander : AI_State_Action
    {
        private Vector2 _currentVelocity = Vector2.Zero;

        private float _timer = 0f;
        [Export] public float moveSpeedFactor = 1;
        [Export] public float updateTime = 1;

        private void SetNewVelocity(StateMachine stateMachine)
        {
            var move = new Vector2
            {
                x = (float) GD.RandRange(-1, 1),
                y = (float) GD.RandRange(-1, 1)
            };

            SetMove(stateMachine, move, moveSpeedFactor, out _currentVelocity);
        }

        public override void OnStart(StateMachine stateMachine)
        {
            base.OnStart(stateMachine);
            _timer = updateTime;
            stateMachine.Connect(nameof(StateMachine.OnCollision), this,
                nameof(SetNewVelocity));
        }

        public override void Perform(StateMachine stateMachine, float delta,
            ref bool interrupt)
        {
            _timer += delta;
            if (_timer >= updateTime)
            {
                _timer = 0;
                SetNewVelocity(stateMachine);
                return;
            }

            SetVelocity(stateMachine, _currentVelocity);
        }
    }
}