using Godot;

namespace HeroesGuild.combat
{
    public static class ColorValues
    {
        public static readonly Color White = new Color(1, 1, 1);
        public static readonly Color Black = new Color(0, 0, 0);

        public static readonly Color Invalid = new Color(1, 0, 1);

        public static readonly Color AttackQuick = new Color(0.12f, 0.47f, 0.9f);
        public static readonly Color AttackCounter = new Color(0.21f, 0.69f, 0.21f);
        public static readonly Color AttackHeavy = new Color(0.74f, 0.21f, 0.21f);

        public static readonly Color Flee = new Color(0.74f, 0.74f, 0.74f);
    }
}