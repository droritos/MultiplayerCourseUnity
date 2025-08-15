using UnityEngine;

namespace Game.Data
{
    public static class AnimatorParams
    {
        public const string Speed = "Speed";
        public const string State = "State"; // 1 - Place Bomb | 2 - Hit | 3 - Die
        public const string EmotePose = "EmotePose"; // 1 - First Emote | 2 - Second Emote
    }
}
public enum PlayerState
{
    Movement = 0,
    PlacingBomb = 1,
    Hitting = 2,
    Dying = 3
}
