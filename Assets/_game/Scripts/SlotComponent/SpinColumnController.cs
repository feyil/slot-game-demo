using System;
using _game.Scripts.Utility;
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
        [SerializeField] private float m_itemHeight;
        [SerializeField] private RectTransform m_rectTransform;
        [SerializeField] private ItemView[] m_itemViewArray;

        [SerializeField] private ColumnAnimationConfigScriptableObject m_fastAnimation;
        [SerializeField] private ColumnAnimationConfigScriptableObject m_normalAnimation;
        [SerializeField] private ColumnAnimationConfigScriptableObject m_slowAnimation;

        private float _spinFillAmount;
        private int _itemCount;

        [Button]
        public void Spin(SpinColumnId spinColumnId, Action<SpinColumnController> onComplete, float delay = 0f,
            ColumnAnimationConfigId animationConfigId = ColumnAnimationConfigId.Fast)
        {
            var config = GetConfig(animationConfigId);

            var step = GetStep();
            var spinDeltaAmount = 0f;
            var isFirst = true;
            var lastItemIndex = m_itemViewArray.Length - 1;
            var lastNormalizedPosition = 0f;
            var isResetTime = false;

            void ResetLoopVariables()
            {
                spinDeltaAmount = 0f;
                isResetTime = true;
                lastItemIndex = m_itemViewArray.Length - 1;
                lastNormalizedPosition = 0f;
                isResetTime = false;
            }

            var targetValue1 = _spinFillAmount + config.StartSpinLoopCount;
            // var startTween = DOTween.To(() => _spinFillAmount, (value) =>
            // {
            //
            //     _spinFillAmount = value;
            //     SetNormalized(_spinFillAmount);
            //
            //     HandleItemLoop(step, ref isFirst, ref lastItemIndex, ref spinDeltaAmount, ref isResetTime,
            //         ref lastNormalizedPosition);
            // }, targetValue1, config.StartSpinDuration).SetEase(Ease.Linear);

            // 5.2
            // 5.2 - 5 = 0.2
            // 1 - 0.2 = 0.8
            // 5.2 + 0.8 = 6 -> you can go from here to any spin item +0.2 * n
            var fractionalPart = 1f - (targetValue1 - Mathf.Floor(targetValue1));

            var targetValue2 = targetValue1 + fractionalPart + config.StopSpinLoopCount +
                               GetSpinFillAmount(spinColumnId);
            var stopTween = DOTween.To(() => _spinFillAmount, (value) =>
            {
                var lastSpinPosition = _spinFillAmount;

                _spinFillAmount = value;
                SetNormalized(_spinFillAmount);

                HandleItemLoop(step, lastSpinPosition, ref isFirst, ref lastItemIndex, ref spinDeltaAmount,
                    ref isResetTime,
                    ref lastNormalizedPosition);
            }, targetValue2, config.StopSpinDuration).SetEase(config.StopCurve);

            var seq = DOTween.Sequence();
            seq.AppendInterval(delay);
            seq.AppendCallback(() => SetBlur(true));
            // seq.Append(startTween);
            seq.AppendCallback(() =>
            {
                SetBlur(false);
                ResetLoopVariables();
            });
            seq.Append(stopTween);
            seq.OnComplete(() => { onComplete?.Invoke(this); });
        }

        private void HandleItemLoop(float step, float lastSpinPosition, ref bool isFirst, ref int lastItemIndex,
            ref float spinDeltaAmount,
            ref bool isResetTime, ref float lastNormalizedPosition)
        {
            var diff = lastSpinPosition - _spinFillAmount;
            Debug.Log(diff);
            spinDeltaAmount += diff <= 0 ? -1 * diff : 0f;
            
            var offsetFromCenter = isFirst ? step / 2 : step;
            if (spinDeltaAmount >= offsetFromCenter && lastItemIndex >= 0)
            {
                isFirst = false;
                spinDeltaAmount = 0f;

                // var isNegative = diff < 0;
                // var index = isNegative ? lastItemIndex : m_itemViewArray.Length - 1 - lastItemIndex;

                var lastItem = m_itemViewArray[lastItemIndex];
                var lastItemPosition = lastItem.GetPosition();

                lastItem.SetPositionY(lastItemPosition.y + m_itemHeight * 3);
                lastItemIndex -= 1;
            }

            var currentNormalizedPosition = GetNormalizedPosition();
            isResetTime = currentNormalizedPosition - lastNormalizedPosition <= 0f;
            lastNormalizedPosition = currentNormalizedPosition;

            // Loop complete Reset Time
            if (lastItemIndex < 0 && GetNormalizedPosition() >= 0 && isResetTime)
            {
                lastItemIndex = m_itemViewArray.Length - 1;
                isResetTime = false;

                for (var index = 0; index < m_itemViewArray.Length; index++)
                {
                    var itemView = m_itemViewArray[index];
                    itemView.SetPositionY(m_itemHeight * (1 - index));
                }
            }
        }

        private float GetSpinFillAmount(SpinColumnId spinColumnId)
        {
            // // TODO can be handled by getting references and finding the exact number, current solution is error-prone
            // switch (spinColumnId)
            // {
            //     case SpinColumnId.Jackpot:
            //         return 0f;
            //     case SpinColumnId.Seven:
            //         return 0.33f;
            //     case SpinColumnId.Wild:
            //         return 0.66f;
            //     // case SpinColumnId.Bonus:
            //     // return 0.8f;
            // }

            // Debug.LogException(new Exception("SpinColumnId doesnt match anything"));
            return 0;
        }

        [Button]
        private void SetNormalized(float t)
        {
            t -= Mathf.Floor(t);
            var posY = Mathf.Lerp(0, -m_itemHeight * 3, t);
            m_rectTransform.SetAnchorPosY(posY);
        }

        [Button]
        private float GetNormalizedPosition()
        {
            return Mathf.InverseLerp(0, -m_itemHeight * 3, m_rectTransform.anchoredPosition.y);
        }

        private void SetBlur(bool state)
        {
            foreach (var itemView in m_itemViewArray)
            {
                itemView.SetBlur(state);
            }
        }

        private float GetStep()
        {
            return 1f / m_itemViewArray.Length;
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