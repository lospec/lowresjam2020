using Godot;
using HeroesGuild.AI.Conditions;

namespace HeroesGuild.AI
{
    public class AI_Transition : Node
    {
        [Export] public AI_State_Condition Condition;
        [Export] public int TrueStateIndex;
        [Export] public int FalseStateIndex;
    }
}