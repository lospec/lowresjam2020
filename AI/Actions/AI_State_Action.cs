using Godot;

namespace HeroesGuild.AI.Actions
{
    public abstract class AI_State_Action : Resource
    {
        public bool IsInitialized { get; private set; } = false;

        protected void SetMove(StateMachine stateMachine, Vector2 direction, float speedFactor, out Vector2 velocity)
        {
            velocity = direction * (stateMachine.Entity.MoveSpeed * speedFactor);
            stateMachine.Entity.Velocity = velocity;
        }

        protected void SetVelocity(StateMachine stateMachine, Vector2 velocity)
        {
            stateMachine.Entity.Velocity = velocity;
        }

        public virtual void OnStart(StateMachine stateMachine)
        {
            IsInitialized = true;
        }

        public abstract void Perform(StateMachine stateMachine, float delta, ref bool interrupt);
    }
}