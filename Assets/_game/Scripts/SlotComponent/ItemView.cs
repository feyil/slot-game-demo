using UnityEngine;
using UnityEngine.UI;

namespace _game.Scripts.SlotComponent
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Image m_image;
        
        [SerializeField] private Sprite m_defaultSprite;
        [SerializeField] private Sprite m_blurredSprite;

        public void SetBlur(bool state)
        {
            m_image.sprite = state ? m_blurredSprite : m_defaultSprite;
        }
    }
}
