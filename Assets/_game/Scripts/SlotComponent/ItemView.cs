using _game.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace _game.Scripts.SlotComponent
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private Image m_image;
        
        [SerializeField] private Sprite m_defaultSprite;
        [SerializeField] private Sprite m_blurredSprite;

        public void SetBlur(bool state)
        {
            // m_image.sprite = state ? m_blurredSprite : m_defaultSprite;
        }

        public Vector3 GetPosition()
        {
            return m_rectTransform.anchoredPosition;
        }

        public void SetPositionY(float y)
        {
            m_rectTransform.SetAnchorPosY(y);
        }

        public void SetSprite(Sprite itemSprite)
        {
            m_image.sprite = itemSprite;
        }
    }
}
