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
        [SerializeField] private float m_itemHeight;
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
            var delta = 0f;
            var isFirst = true;
            var counter = m_itemViewArray.Length;
            var isResetTime = false;
            var lastValue = 0f;
            var startTween = DOTween.To(() => _spinFillAmount, (value) =>
            {
                delta += Mathf.Abs(value - _spinFillAmount);

                _spinFillAmount = value;
                SetNormalized(_spinFillAmount);

                var offset = isFirst ? 0.165f : 0.33f;
                if (delta >= offset && counter >= 1)
                {
                    isFirst = false;
                    delta = 0f;
                    counter -= 1;
                    // Debug.Log(GetNormalizedPosition());
                    
                    var lastItem = m_itemViewArray[counter];

                    var lastY = lastItem.GetComponent<RectTransform>().anchoredPosition.y;
                    lastItem.GetComponent<RectTransform>().DOAnchorPosY(lastY + m_itemHeight * 3, 0f);
                }

                isResetTime = GetNormalizedPosition() - lastValue <= 0f;
                lastValue = GetNormalizedPosition();
                
                // Debug.Log(GetNormalizedPosition());
                if (counter <= 0 && GetNormalizedPosition() >= 0 && isResetTime)
                {
                    for (var index = 0; index < m_itemViewArray.Length; index++)
                    {
                        var itemView = m_itemViewArray[index];
                        SetAnchorPosY(itemView.GetComponent<RectTransform>(), m_itemHeight * (1 - index));
                    }

                    counter = m_itemViewArray.Length;
                    Debug.Log("Reset");
                    isResetTime = false;
                }
                
            }, targetValue1, config.StartSpinDuration).SetEase(Ease.Linear);

            // 5.2
            // 5.2 - 5 = 0.2
            // 1 - 0.2 = 0.8
            // 5.2 + 0.8 = 6 -> you can go from here to any spin item +0.2 * n
            var fractionalPart = 1f - (targetValue1 - Mathf.Floor(targetValue1));

            var targetValue2 = targetValue1 + fractionalPart + config.StopSpinLoopCount +
                               GetSpinFillAmount(spinColumnId);
            // var stopTween = DOTween.To(() => _spinFillAmount, (value) =>
            // {
            //     _spinFillAmount = value;
            //     SetNormalized(_spinFillAmount);
            // }, targetValue2, config.StopSpinDuration);

            // stopTween.SetEase(config.StopCurve);

            var seq = DOTween.Sequence();
            seq.AppendInterval(delay);
            seq.AppendCallback(() => SetBlur(true));
            seq.Append(startTween);
            seq.AppendCallback(() => SetBlur(false));
            // seq.Append(stopTween);
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
                    return 0.33f;
                case SpinColumnId.Wild:
                    return 0.66f;
                // case SpinColumnId.Bonus:
                // return 0.8f;
            }

            Debug.LogException(new Exception("SpinColumnId doesnt match anything"));
            return 0;
        }

        [Button]
        private void SetNormalized(float t)
        {
            t -= Mathf.Floor(t);
            var posY = Mathf.Lerp(0, -m_itemHeight * 3, t);
            SetAnchorPosY(m_rectTransform, posY);
        }

        [Button]
        private float GetNormalizedPosition()
        {
            return Mathf.InverseLerp(0, -m_itemHeight * 3, m_rectTransform.anchoredPosition.y);
        }

        private void SetAnchorPosY(RectTransform rectTransform, float y)
        {
            var pos = rectTransform.anchoredPosition;
            pos.y = y;
            rectTransform.anchoredPosition = pos;
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