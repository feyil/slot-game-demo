using System;
using UnityEngine;

namespace _game.Scripts.SlotComponent
{
    [CreateAssetMenu(fileName = "ColumnAnimationConfigScriptableObject",
        menuName = "ScriptableObjects/ColumnAnimationConfigScriptableObject", order = 1)]
    public class ColumnAnimationConfigScriptableObject : ScriptableObject
    {
        public ColumnAnimationConfig FastConfig;
        public ColumnAnimationConfig NormalConfig;
        public ColumnAnimationConfig SlowConfig;

        public ColumnAnimationConfig GetConfig(ColumnAnimationConfigId animationConfigId)
        {
            switch (animationConfigId)
            {
                case ColumnAnimationConfigId.Fast:
                    return FastConfig;
                case ColumnAnimationConfigId.Normal:
                    return NormalConfig;
                case ColumnAnimationConfigId.Slow:
                    return SlowConfig;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animationConfigId), animationConfigId, null);
            }
        }
    }
}