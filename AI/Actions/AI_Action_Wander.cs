using Godot;

namespace HeroesGuild.AI.Actions
{
    public class AI_Action_Wander : AI_State_Action
    {
        [Export] public float MoveSpeedFactor = 1;
        [Export] public float UpdateTime = 1;

        private float _timer = 0f;
        private Vector2 _currentVelocity = Vector2.Zero;

        private void SetNewVelocity(StateMachine stateMachine)
        {
            var move = new Vector2
            {
                x = (float) GD.RandRange(-1, 1),
                y = (float) GD.RandRange(-1, 1)
            };

            SetMove(stateMachine, move, MoveSpeedFactor, out _currentVelocity);
        }

        public override void OnStart(StateMachine stateMachine)
        {
            base.OnStart(stateMachine);
            _timer = UpdateTime;
            stateMachine.Connect(nameof(StateMachine.OnCollision), this, nameof(SetNewVelocity));
        }

        public override void Perform(StateMachine stateMachine, float delta, ref bool interrupt)
        {
            _timer += delta;
            if (_timer >= UpdateTime)
            {
                _timer = 0;
                SetNewVelocity(stateMachine);
                return;
            }

            SetVelocity(stateMachine, _currentVelocity);
        }
    }
}