using _game.Scripts.Core.Ui;
using _game.Scripts.SlotComponent;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _game.Scripts.Ui.Controllers
{
    public class GameUiController : UiController
    {
        [SerializeField] private SlotMachineController m_slotMachine;

        public SlotMachineController GetSlotMachine()
        {
            return m_slotMachine;
        }
    }
}