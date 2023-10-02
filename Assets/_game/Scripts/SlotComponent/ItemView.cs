using _game.Scripts.Utility;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _game.Scripts.SlotComponent
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Vector2 m_bounds;
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private Image m_image;

        [Button]
        public void SetNormalized(float t)
        {
            t -= Mathf.Floor(t);
            var value = Mathf.Lerp(m_bounds.x, m_bounds.y, t);
            m_rectTransform.SetAnchorPosY(value);
        }

        [Button]
        public float GetNormalized()
        {
            var pos = m_rectTransform.anchoredPosition;
            return Mathf.InverseLerp(m_bounds.x, m_bounds.y, pos.y);
        }

        public void SetSprite(Sprite getSprite)
        {
            m_image.sprite = getSprite;
        }

        public void SetSprite(ItemSpriteData[] itemSpriteDataArray, bool isBlurred)
        {
            foreach (var itemSpriteData in itemSpriteDataArray)
            {
                var currentSprite = m_image.sprite;
                if (itemSpriteData.Default == currentSprite || itemSpriteData.Blurred == currentSprite)
                {
                    m_image.sprite = isBlurred ? itemSpriteData.Blurred : itemSpriteData.Default;
                }
            }
        }
    }
}