using Sirenix.OdinInspector;
using UnityEngine;

namespace _game.Scripts.SlotComponent
{
    public class CoinParticleController : MonoBehaviour
    {
        [SerializeField] private ParticleSystem m_particleSystem;
        [SerializeField] private float m_defaultRateOverTime = 20;
        
        [Button]
        public void Play()
        {
            m_particleSystem.Play();
        }

        [Button]
        public void Play(float rateOverTime)
        {
            SetRateOverTime(rateOverTime);
            Play();
        }

        public void Play(SpinColumnId spinColumnId)
        {
            var rateOverTime = m_defaultRateOverTime;
            Play(rateOverTime);
            
            // TODO can be moved to Scriptable Object config.
            switch (spinColumnId)
            {
                case SpinColumnId.Jackpot:
                    rateOverTime = 70;
                    break;
                case SpinColumnId.Wild:
                    rateOverTime = 60;
                    break;
                case SpinColumnId.Seven:
                    rateOverTime = 50;
                    break;
                case SpinColumnId.Bonus:
                    rateOverTime = 40;
                    break;
                case SpinColumnId.A:
                    rateOverTime = 30;
                    break;
            }
            
            Play(rateOverTime);
        }

        private void SetRateOverTime(float rateOverTime)
        {
            var emissionModule = m_particleSystem.emission;
            emissionModule.rateOverTime = rateOverTime;
        }
    }
}