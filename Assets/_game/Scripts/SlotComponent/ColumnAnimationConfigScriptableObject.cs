using UnityEngine;

namespace _game.Scripts.SlotComponent
{
    [CreateAssetMenu(fileName = "ColumnAnimationConfigScriptableObject", menuName = "ScriptableObjects/ColumnAnimationConfigScriptableObject", order = 1)]
    public class ColumnAnimationConfigScriptableObject : ScriptableObject
    {
        public ColumnAnimationConfig Config;
    }
}
