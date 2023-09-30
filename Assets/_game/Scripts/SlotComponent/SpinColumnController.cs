using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _game.Scripts.SlotComponent
{
    public class SpinColumnController : MonoBehaviour
    {
        [SerializeField] private ItemView[] m_itemViewArray;
        [SerializeField] private ItemSpriteData[] m_itemSpriteDataArray;

        private bool _isBlurred;
        private int _spriteIndex = -1;
        private SpinColumnId _lastSpinColumnId;

        public void Spin(SpinColumnId spinColumnId, Action<SpinColumnController> onComplete,
            ColumnAnimationConfig animationConfig, float delay)
        {
            var startSequence = DOTween.Sequence();
            startSequence.AppendInterval(delay);
            startSequence.AppendCallback(() => { _isBlurred = true; });

            var zeroPointOffset = -GetTargetItemOffset(_lastSpinColumnId);
            foreach (var itemView in m_itemViewArray)
            {
                var tween = AnimateItemView(itemView, animationConfig.StartSpinDuration,
                        animationConfig.StartSpinLoopCount, zeroPointOffset)
                    .SetEase(Ease.Linear).OnStart(() => { itemView.SetSprite(m_itemSpriteDataArray, _isBlurred); });
                startSequence.Join(tween);
            }

            startSequence.OnComplete(() =>
            {
                var stopSequence = DOTween.Sequence();
                stopSequence.AppendCallback(() => { _isBlurred = false; });

                var targetItemOffset = GetTargetItemOffset(spinColumnId);
                foreach (var itemView in m_itemViewArray)
                {
                    var tween = AnimateItemView(itemView, animationConfig.StopSpinDuration,
                            animationConfig.StopSpinLoopCount, targetItemOffset)
                        .SetEase(animationConfig.StopCurve)
                        .OnStart(() => { itemView.SetSprite(m_itemSpriteDataArray, _isBlurred); });
                    stopSequence.Join(tween);
                }

                stopSequence.OnComplete(() => { onComplete?.Invoke(this); });
            });

            _lastSpinColumnId = spinColumnId;
        }

        private Tween AnimateItemView(ItemView itemView, float duration, int loopCount, float spinTargetItemOffset = 0f)
        {
            var currentNormalizedValue = itemView.GetNormalized();
            var targetLoopValue = GetSingleLoop() * loopCount +
                                  spinTargetItemOffset;

            var targetValue = currentNormalizedValue + targetLoopValue;

            var loopCounter = 1;
            return DOTween.To(itemView.GetNormalized, (value) =>
                {
                    itemView.SetNormalized(value);

                    // Forward handling
                    while (value > loopCounter)
                    {
                        loopCounter++;
                        IncreaseSpriteIndex();

                        var sprite = GetSprite();
                        itemView.SetSprite(sprite);
                    }

                    // Backward handling
                    while (value < loopCounter - 1)
                    {
                        loopCounter--;
                        DecreaseSpriteIndex();

                        var sprite = GetSprite(-2);
                        itemView.SetSprite(sprite);
                    }
                }, targetValue,
                duration);
        }

        private float GetStep()
        {
            return 1f / 3;
        }

        private float GetSingleLoop()
        {
            return 1f + GetStep() * 2;
        }

        private void IncreaseSpriteIndex()
        {
            _spriteIndex++;
        }

        private void DecreaseSpriteIndex()
        {
            _spriteIndex--;
        }

        private Sprite GetSprite(int offset = 0)
        {
            var index = _spriteIndex + offset;
            if (index < 0)
            {
                index = m_itemSpriteDataArray.Length + index;
            }

            var data = m_itemSpriteDataArray[index % m_itemSpriteDataArray.Length];
            return _isBlurred ? data.Blurred : data.Default;
        }

        [Button]
        private float GetTargetItemOffset(SpinColumnId spinColumnId)
        {
            switch (spinColumnId)
            {
                case SpinColumnId.Jackpot:
                    return 0f;
                case SpinColumnId.Seven:
                    return GetStep();
                case SpinColumnId.Wild:
                    return GetStep() * 2;
                case SpinColumnId.A:
                    return GetStep() * 3;
                case SpinColumnId.Bonus:
                    return GetStep() * 4;
            }

            Debug.LogException(new Exception("SpinColumnId doesnt match anything"));
            return 0;
        }
    }
}