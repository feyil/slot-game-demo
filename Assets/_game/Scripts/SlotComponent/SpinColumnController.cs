using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _game.Scripts.SlotComponent
{
    [Serializable]
    public class ColumnAnimationConfig
    {
        public int StartSpinLoopCount = 5;
        public float StartSpinDuration = 1f;
        public int StopSpinLoopCount = 1;
        public float StopSpinDuration = 0.5f;
        public AnimationCurve StopCurve;
    }

    public enum ColumnAnimationConfigId
    {
        Fast = 0,
        Normal = 1,
        Slow
    }

    public class SpinColumnController : MonoBehaviour
    {
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private ItemView[] m_itemViewArray;

        [SerializeField] private ColumnAnimationConfigScriptableObject m_fastAnimation;
        [SerializeField] private ColumnAnimationConfigScriptableObject m_normalAnimation;
        [SerializeField] private ColumnAnimationConfigScriptableObject m_slowAnimation;

        private float _spinFillAmount;

        [Button]
        public void Spin(SpinColumnId spinColumnId, Action<SpinColumnController> onComplete, float delay = 0f,
            ColumnAnimationConfigId animationConfigId = ColumnAnimationConfigId.Fast)
        {
            var config = GetConfig(animationConfigId);

            var targetValue1 = _spinFillAmount + config.StartSpinLoopCount;
            var startTween = DOTween.To(() => _spinFillAmount, (value) =>
            {
                _spinFillAmount = value;
                SetNormalized(_spinFillAmount);
            }, targetValue1, config.StartSpinDuration).SetEase(Ease.Linear);

            // 5.2
            // 5.2 - 5 = 0.2
            // 1 - 0.2 = 0.8
            // 5.2 + 0.8 = 6 -> you can go from here to any spin item +0.2 * n
            var fractionalPart = 1f - (targetValue1 - Mathf.Floor(targetValue1));

            var targetValue2 = targetValue1 + fractionalPart + config.StopSpinLoopCount +
                               GetSpinFillAmount(spinColumnId);
            var stopTween = DOTween.To(() => _spinFillAmount, (value) =>
            {
                _spinFillAmount = value;
                SetNormalized(_spinFillAmount);
            }, targetValue2, config.StopSpinDuration);

            stopTween.SetEase(config.StopCurve);

            var seq = DOTween.Sequence();
            seq.AppendInterval(delay);
            seq.AppendCallback(() => SetBlur(true));
            seq.Append(startTween);
            seq.AppendCallback(() => SetBlur(false));
            seq.Append(stopTween);
            seq.OnComplete(() => { onComplete?.Invoke(this); });
        }

        private float GetSpinFillAmount(SpinColumnId spinColumnId)
        {
            // TODO can be handled by getting references and finding the exact number, current solution is error-prone
            switch (spinColumnId)
            {
                case SpinColumnId.Jackpot:
                    return 0f;
                case SpinColumnId.Seven:
                    return 0.2f;
                case SpinColumnId.Wild:
                    return 0.4f;
                case SpinColumnId.A:
                    return 0.6f;
                case SpinColumnId.Bonus:
                    return 0.8f;
            }

            Debug.LogException(new Exception("SpinColumnId doesnt match anything"));
            return 0;
        }

        [Button]
        private void SetNormalized(float t)
        {
            t -= Mathf.Floor(t);
            var posY = Mathf.Lerp(110, 1160, 1 - t);
            SetAnchorPosY(posY);
        }

        private void SetAnchorPosY(float y)
        {
            var pos = m_rectTransform.anchoredPosition;
            pos.y = y;
            m_rectTransform.anchoredPosition = pos;
        }

        private void SetBlur(bool state)
        {
            foreach (var itemView in m_itemViewArray)
            {
                itemView.SetBlur(state);
            }
        }

        private ColumnAnimationConfig GetConfig(ColumnAnimationConfigId animationConfigId)
        {
            switch (animationConfigId)
            {
                case ColumnAnimationConfigId.Fast:
                    return m_fastAnimation.Config;
                case ColumnAnimationConfigId.Normal:
                    return m_normalAnimation.Config;
                case ColumnAnimationConfigId.Slow:
                    return m_slowAnimation.Config;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animationConfigId), animationConfigId, null);
            }
        }
    }
}