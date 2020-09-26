using Godot;
using HeroesGuild.ai.conditions;

namespace HeroesGuild.ai
{
    public class AI_Transition : Resource
    {
        [Export] public AI_State_Condition condition;
        [Export] public int falseStateIndex;
        [Export] public int trueStateIndex;
    }
}