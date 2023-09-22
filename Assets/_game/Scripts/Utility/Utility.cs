using UnityEngine;

namespace _game.Scripts.Utility
{
    public static class Utility
    {
        public static void SetAnchorPosY(this RectTransform rectTransform, float y)
        {
            var pos = rectTransform.anchoredPosition;
            pos.y = y;
            rectTransform.anchoredPosition = pos;
        }
    }
}