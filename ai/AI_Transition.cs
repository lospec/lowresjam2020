using Godot;
using HeroesGuild.AI.Conditions;

namespace HeroesGuild.AI
{
    public class AI_Transition : Resource
    {
        [Export] public AI_State_Condition condition;
        [Export] public int trueStateIndex;
        [Export] public int falseStateIndex;
    }
}