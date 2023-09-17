using System;
using _game.Scripts.SpinSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace _game.Scripts.SlotComponent
{
    public class SlotMachineController : MonoBehaviour
    {
        [SerializeField] private Button m_spinButton;
        
        [SerializeField] private SpinColumnController m_leftSpinController;
        [SerializeField] private SpinColumnController m_middleSpinController;
        [SerializeField] private SpinColumnController m_rightSpinController;

        public void Initialize(Action<SlotMachineController> onSpin)
        {
            m_spinButton.onClick.RemoveAllListeners();
            m_spinButton.onClick.AddListener(() => onSpin?.Invoke(this));
        }
        
        [Button]
        public void Spin(SpinId spinId)
        {
            switch (spinId)
            {
                case SpinId.A_WILD_BONUS:
                    Spin(SpinColumnId.A, SpinColumnId.Wild, SpinColumnId.Bonus);
                    break;
                case SpinId.WILD_WILD_SEVEN:
                    Spin(SpinColumnId.Wild, SpinColumnId.Wild, SpinColumnId.Seven);
                    break;
                case SpinId.JACKPOT_JACKPOT_A:
                    Spin(SpinColumnId.Jackpot, SpinColumnId.Jackpot, SpinColumnId.A);
                    break;
                case SpinId.WILD_BONUS_A:
                    Spin(SpinColumnId.Wild, SpinColumnId.Bonus, SpinColumnId.A);
                    break;
                case SpinId.BONUS_A_JACKPOT:
                    Spin(SpinColumnId.Bonus, SpinColumnId.A, SpinColumnId.Jackpot);
                    break;
                case SpinId.A_A_A:
                    Spin(SpinColumnId.A, SpinColumnId.A, SpinColumnId.A);
                    break;
                case SpinId.BONUS_BONUS_BONUS:
                    Spin(SpinColumnId.Bonus, SpinColumnId.Bonus, SpinColumnId.Bonus);
                    break;
                case SpinId.SEVEN_SEVEN_SEVEN:
                    Spin(SpinColumnId.Seven, SpinColumnId.Seven, SpinColumnId.Seven);
                    break;
                case SpinId.WILD_WILD_WILD:
                    Spin(SpinColumnId.Wild, SpinColumnId.Wild, SpinColumnId.Wild);
                    break;
                case SpinId.JACKPOT_JACKPOT_JACKPOT:
                    Spin(SpinColumnId.Jackpot, SpinColumnId.Jackpot, SpinColumnId.Jackpot);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(spinId), spinId, null);
            }
        }

        private void Spin(SpinColumnId left, SpinColumnId middle, SpinColumnId right)
        {
            m_leftSpinController.Spin(left, OnComplete);
            m_middleSpinController.Spin(middle, OnComplete);
            m_rightSpinController.Spin(right, OnComplete);
        }

        private void OnComplete(SpinColumnController spinColumnController)
        {
        }
    }
}