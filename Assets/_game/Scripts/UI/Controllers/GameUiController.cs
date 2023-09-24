using System;
using _game.Scripts.Core.Ui;
using _game.Scripts.SlotComponent;
using UnityEngine;
using UnityEngine.UI;

namespace _game.Scripts.Ui.Controllers
{
    public class GameUiController : UiController
    {
        [SerializeField] private SlotMachineController m_slotMachine;
        [SerializeField] private Button m_spinButton;

        public void Show(Action<SlotMachineController> onSpin)
        {
            m_spinButton.onClick.RemoveAllListeners();
            m_spinButton.onClick.AddListener(() => onSpin?.Invoke(m_slotMachine));

            m_slotMachine.Initialize(OnSpinStart, OnSpinEnd);

            base.Show();
        }

        private void OnSpinStart()
        {
            m_spinButton.interactable = false;
        }

        private void OnSpinEnd()
        {
            m_spinButton.interactable = true;
        }
    }
}