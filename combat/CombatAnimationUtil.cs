using System.Collections.Generic;

namespace HeroesGuild.Combat
{
    public class CombatAnimationUtil
    {
        public enum AnimationState
        {
            Normal,
            EyesClosed,
            Hurt
        }

        public const float BattleTexturePosY = 0f;
        public const float BattleTextureWidth = 32f;
        public const float BattleTextureHeight = 32f;

        public static readonly Dictionary<AnimationState, float> AnimationStateRegionPositionX =
            new Dictionary<AnimationState, float>
            {
                {AnimationState.Normal, 0f},
                {AnimationState.EyesClosed, 32f},
                {AnimationState.Hurt, 64f}
            };
    }
}