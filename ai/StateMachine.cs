using System;
using Godot;
using Godot.Collections;
using HeroesGuild.Entities.BaseEntity;
using HeroesGuild.Entities.Enemies.BaseEnemy;

namespace HeroesGuild.AI
{
    public class StateMachine : Node
    {
        [Signal] public delegate void OnCollision(StateMachine stateMachine);

        [Export] public bool active;
        [Export] public AI_Behaviour behaviour;

        public BaseEntity Entity { get; private set; }
        public AI_State CurrentState { get; private set; }
        public BaseEntity Target { get; set; }
        public Vector2 OriginPosition { get; private set; }

        private Area2D _circleArea;
        private CollisionShape2D _collisionCircleShape;


        public float DistanceToTarget
        {
            get
            {
                if (Target == null)
                {
                    return -1f;
                }

                return Entity.Position.DistanceTo(Target.Position);
            }
        }

        public float DistanceToOrigin => Entity.Position.DistanceTo(OriginPosition);

        public override async void _Ready()
        {
            Entity = GetParent<BaseEntity>();
            _circleArea = GetNode<Area2D>("CircleArea2D");
            _collisionCircleShape = _circleArea.GetNode<CollisionShape2D>("CollisionShape2D");
            if (Entity.HasSignal(nameof(BaseEnemy.StatsLoaded)))
            {
                await ToSignal(Entity, nameof(BaseEnemy.StatsLoaded));
            }

            OriginPosition = Entity.Position;
            behaviour.SetStartingState(this);
        }

        public override void _Process(float delta)
        {
            UpdateCurrentState(delta);
            if (Entity.GetSlideCount() > 0)
            {
                EmitSignal(nameof(OnCollision), this);
            }
        }

        private void UpdateCurrentState(float delta)
        {
            if (!active)
            {
                return;
            }

            Entity.Velocity = Vector2.Zero;
            CurrentState.UpdateState(this, delta);
        }

        public void TransitionToState(int stateIndex, out bool result)
        {
            result = false;
            if (stateIndex == -1)
            {
                return;
            }

            var state = behaviour.states[stateIndex];
            if (state != null)
            {
                CurrentState = state.Instance();
                CurrentState.OnStart(this);
                result = true;
            }
        }

        public Array<KinematicBody2D> FindBodiesInRange(float range)
        {
            var shape = (CircleShape2D) _collisionCircleShape.Shape;
            if (Math.Abs(shape.Radius - range) > 1f)
            {
                shape.Radius = range;
            }

            _collisionCircleShape.Shape = shape;
            return new Array<KinematicBody2D>(_circleArea.GetOverlappingBodies());
        }
    }
}