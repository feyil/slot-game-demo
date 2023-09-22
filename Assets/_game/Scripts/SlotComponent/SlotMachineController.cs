using System;
using _game.Scripts.SpinSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _game.Scripts.SlotComponent
{
    public class SlotMachineController : MonoBehaviour
    {
        [SerializeField] private CoinParticleController m_coinParticleController;

        [SerializeField] private SpinColumnController m_leftSpinController;
        [SerializeField] private SpinColumnController m_middleSpinController;
        [SerializeField] private SpinColumnController m_rightSpinController;

        [Title("Parameters")] [SerializeField] private float DelayMin = 0.1f;
        [SerializeField] private float DelayMax = 0.3f;

        [SerializeField] private int _completeCount;

        private bool _isReward;
        private SpinColumnId _rewardId;
        
        private Action _onSpinStart;
        private Action _onSpinEnd;

        public void Initialize(Action onSpinStart, Action onSpinEnd)
        {
            _onSpinStart = onSpinStart;
            _onSpinEnd = onSpinEnd;
        }

        [Button]
        public void Spin(SpinId spinId)
        {
            _completeCount = 0;
            
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
            _onSpinStart?.Invoke();
            
            m_leftSpinController.Spin(left, OnComplete);

            var delayMiddle = Random.Range(DelayMin, DelayMax);
            var delayRight = delayMiddle + Random.Range(DelayMin, DelayMax);

            m_middleSpinController.Spin(middle, OnComplete, delayMiddle);

            var isReward = left == middle && middle == right;
            _isReward = isReward;
            _rewardId = left;

            var animIndex = Random.Range(0, 2);

            var animId = ColumnAnimationConfigId.Fast;
            if (left == middle)
            {
                animId = animIndex == 1 ? ColumnAnimationConfigId.Normal : ColumnAnimationConfigId.Slow;
            }

            m_rightSpinController.Spin(right, OnComplete, delayRight, animId);
        }

        private void OnComplete(SpinColumnController spinColumnController)
        {
            _completeCount++;
            if (_completeCount == 3)
            {
                OnAllComplete();
            }
        }

        private void OnAllComplete()
        {
            Debug.Log("[SLOT_MACHINE_CONTROLLER] All columns completed.");
           
            if (_isReward)
            {
                m_coinParticleController.Play(_rewardId);
            }
            
            _onSpinEnd?.Invoke();
        }
    }
}