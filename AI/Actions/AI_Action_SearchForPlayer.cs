using System.Linq;
using Godot;
using HeroesGuild.Entities.BaseEntity;

namespace HeroesGuild.AI.Actions
{
    public class AI_Action_SearchForPlayer : AI_State_Action
    {
        [Export] public float SearchRange = 1f;

        public override void Perform(StateMachine stateMachine, float delta, ref bool interrupt)
        {
            // TODO: Default might not be null here
            var target =
                (from body in stateMachine.FindBodiesInRange(SearchRange)
                    where body.IsInGroup("PlayerGroup")
                    select body as BaseEntity).FirstOrDefault();
            stateMachine.Target = target;
        }
    }
}