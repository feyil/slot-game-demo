using _game.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace _game.Scripts.SlotComponent
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private Image m_image;
        
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
