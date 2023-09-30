using System;
using UnityEngine;

namespace _game.Scripts.SlotComponent
{
    [Serializable]
    public class ColumnAnimationConfig
    {
        public int StartSpinLoopCount = 5;
        public float StartSpinDuration = 1f;
        public int StopSpinLoopCount = 1;
        public float StopSpinDuration = 0.5f;
        public AnimationCurve StopCurve;
    }
}