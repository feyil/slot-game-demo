using _game.Scripts.Core.Ui;
using UnityEngine.EventSystems;

namespace _game.Scripts.Ui.Controllers
{
    public class GameUiController : UiController, IPointerClickHandler
    {
        public override void Show()
        {
            base.Show();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Hide();
        }
    }
}