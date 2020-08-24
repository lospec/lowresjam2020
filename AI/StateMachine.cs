using System;
using Godot;
using Godot.Collections;
using HeroesGuild.Entities.BaseEntity;
using HeroesGuild.Entities.enemies.base_enemy;

namespace HeroesGuild.AI
{
    public class StateMachine : Node
    {
        [Signal] public delegate void OnCollision(StateMachine stateMachine);

        [Export] public bool Active;
        [Export] public AI_Behaviour Behaviour;

        public BaseEntity Entity { get; private set; }
        public AI_State CurrentState { get; private set; }
        public BaseEntity Target { get; set; }
        public Vector2 OriginPosition { get; private set; }

        private Area2D circleArea;
        private CollisionShape2D collisionCircleShape;


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
            circleArea = GetNode<Area2D>("CircleArea2D");
            collisionCircleShape = circleArea.GetNode<CollisionShape2D>("CollisionShape2D");
            if (Entity.HasSignal(nameof(BaseEnemy.StatsLoaded)))
            {
                await ToSignal(Entity, nameof(BaseEnemy.StatsLoaded));
            }

            OriginPosition = Entity.Position;
            Behaviour.SetStartingState(this);
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
            if (!Active)
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

            var state = Behaviour.States[stateIndex];
            if (state != null)
            {
                CurrentState = state.Instance();
                CurrentState.OnStart(this);
                result = true;
            }
        }

        public Array<KinematicBody2D> FindBodiesInRange(float range)
        {
            var shape = (CircleShape2D) collisionCircleShape.Shape;
            if (Math.Abs(shape.Radius - range) > 1f)
            {
                shape.Radius = range;
            }

            collisionCircleShape.Shape = shape;
            return new Array<KinematicBody2D>(circleArea.GetOverlappingBodies());
        }
    }
}