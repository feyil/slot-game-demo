using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _game.Scripts.SlotComponent
{
    public class SpinColumnController : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rectTransform;

        [Title("Parameters")] [SerializeField] private int m_startSpinLoopCount = 5;
        [SerializeField] private float m_startSpinDuration = 1f;

        [SerializeField] private int m_stopSpinLoopCount = 1;
        [SerializeField] private float m_stopSpinDuration = 0.5f;

        private float _spinFillAmount;

        [Button]
        public void Spin(SpinColumnId spinColumnId, Action onComplete)
        {
            var targetValue1 = _spinFillAmount + m_startSpinLoopCount;
            var startTween = DOTween.To(() => _spinFillAmount, (value) =>
            {
                _spinFillAmount = value;
                SetNormalized(_spinFillAmount);
            }, targetValue1, m_startSpinDuration).SetEase(Ease.Linear);

            // 5.2
            // 5.2 - 5 = 0.2
            // 1 - 0.2 = 0.8
            // 5.2 + 0.8 = 6 -> you can go from here to any spin item +0.2 * n
            var fractionalPart = 1f - (targetValue1 - Mathf.Floor(targetValue1));

            var targetValue2 = targetValue1 + fractionalPart + m_stopSpinLoopCount + GetSpinFillAmount(spinColumnId);
            var stopTween = DOTween.To(() => _spinFillAmount, (value) =>
            {
                _spinFillAmount = value;
                SetNormalized(_spinFillAmount);
            }, targetValue2, m_stopSpinDuration);

            var seq = DOTween.Sequence();
            seq.Append(startTween);
            seq.Append(stopTween);
            seq.OnComplete(() => { onComplete?.Invoke(); });
        }

        private float GetSpinFillAmount(SpinColumnId spinColumnId)
        {
            // TODO can be handled by getting references and finding the exact number, current solution is error-prone
            switch (spinColumnId)
            {
                case SpinColumnId.Jackpot:
                    return 1f;
                case SpinColumnId.Seven:
                    return 0.8f;
                case SpinColumnId.Wild:
                    return 0.6f;
                case SpinColumnId.A:
                    return 0.4f;
                case SpinColumnId.Bonus:
                    return 0.2f;
            }

            Debug.LogException(new Exception("SpinColumnId doesnt match anything"));
            return 0;
        }

        [Button]
        private void SetNormalized(float t)
        {
            t -= Mathf.Floor(t);
            var posY = Mathf.Lerp(110, 1160, t);
            SetAnchorPosY(posY);
        }

        private void SetAnchorPosY(float y)
        {
            var pos = m_rectTransform.anchoredPosition;
            pos.y = y;
            m_rectTransform.anchoredPosition = pos;
        }
    }
}