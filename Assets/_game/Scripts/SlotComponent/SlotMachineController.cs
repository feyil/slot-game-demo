using System;
using System.Linq;
using _game.Scripts.SpinSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _game.Scripts.SlotComponent
{
    public class SlotMachineController : MonoBehaviour
    {
        [SerializeField] private CoinParticleController m_coinParticleController;

        [SerializeField] private SpinColumnController[] m_columArray;

        [Title("Parameters")] [SerializeField] private float m_delayMin = 0.1f;
        [SerializeField] private float m_delayMax = 0.3f;
        [SerializeField] private ColumnAnimationConfigScriptableObject m_animationConfig;

        private int _completeCount;
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
                case SpinId.AWildBonus:
                    Spin(new[] { SpinColumnId.A, SpinColumnId.Wild, SpinColumnId.Bonus });
                    break;
                case SpinId.WildWildSeven:
                    Spin(new[] { SpinColumnId.Wild, SpinColumnId.Wild, SpinColumnId.Seven });
                    break;
                case SpinId.JackpotJackpotA:
                    Spin(new[] { SpinColumnId.Jackpot, SpinColumnId.Jackpot, SpinColumnId.A });
                    break;
                case SpinId.WildBonusA:
                    Spin(new[] { SpinColumnId.Wild, SpinColumnId.Bonus, SpinColumnId.A });
                    break;
                case SpinId.BonusAJackpot:
                    Spin(new[] { SpinColumnId.Bonus, SpinColumnId.A, SpinColumnId.Jackpot });
                    break;
                case SpinId.AAA:
                    Spin(new[] { SpinColumnId.A, SpinColumnId.A, SpinColumnId.A });
                    break;
                case SpinId.BonusBonusBonus:
                    Spin(new[] { SpinColumnId.Bonus, SpinColumnId.Bonus, SpinColumnId.Bonus });
                    break;
                case SpinId.SevenSevenSeven:
                    Spin(new[] { SpinColumnId.Seven, SpinColumnId.Seven, SpinColumnId.Seven });
                    break;
                case SpinId.WildWildWild:
                    Spin(new[] { SpinColumnId.Wild, SpinColumnId.Wild, SpinColumnId.Wild });
                    break;
                case SpinId.JackpotJackpotJackpot:
                    Spin(new[] { SpinColumnId.Jackpot, SpinColumnId.Jackpot, SpinColumnId.Jackpot });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(spinId), spinId, null);
            }
        }

        private void Spin(SpinColumnId[] columnIdArray)
        {
            _onSpinStart?.Invoke();

            var totalDelay = 0f;
            for (var index = 0; index < m_columArray.Length; index++)
            {
                var spinColumnController = m_columArray[index];
                if (index >= columnIdArray.Length) break;

                var spinColumnId = columnIdArray[index];
                var delay = 0f;
                var animId = ColumnAnimationConfigId.Fast;
                if (index != 0)
                {
                    delay = totalDelay + Random.Range(m_delayMin, m_delayMax);
                    totalDelay += totalDelay;
                }

                if (index == 2 && columnIdArray[index - 1] == columnIdArray[index - 2])
                {
                    var animIndex = Random.Range(0, 2);
                    animId = animIndex == 1 ? ColumnAnimationConfigId.Normal : ColumnAnimationConfigId.Slow;
                }

                spinColumnController.Spin(spinColumnId, OnComplete, m_animationConfig.GetConfig(animId), delay);
            }

            var isReward = columnIdArray.ToHashSet().Count == 1;
            _isReward = isReward;
            _rewardId = columnIdArray[0];
        }

        private void OnComplete(SpinColumnController spinColumnController)
        {
            _completeCount++;
            if (_completeCount == m_columArray.Length)
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