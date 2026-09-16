using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts._Common.ProceduralAnimation
{
    public class HitEffect : MonoBehaviour
    {
        [SerializeField] private Vector3 _minScale = Vector3.one * 0.9f;
        [SerializeField] private Vector3 _maxScale = Vector3.one * 1.1f;
        [SerializeField, Min(0.001f)] private float _duration = 0.12f;
        [SerializeField] private Ease _ease = Ease.OutQuad;
        [SerializeField] private bool _returnToInitialScale = true;

        private Sequence _hitEffectSequence;
        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = transform.localScale;
        }

        [ContextMenu(nameof(StartHit))]
        public void StartHit()
        {
            if (_hitEffectSequence == null)
                InitializeHitEffect();

            _hitEffectSequence.Restart();
        }

        private void OnDisable()
        {
            _hitEffectSequence?.Kill();
            _hitEffectSequence = null;

            if (_returnToInitialScale)
                transform.localScale = _initialScale;
        }

        private void InitializeHitEffect()
        {
            float halfDuration = _duration * 0.5f;
            Vector3 endScale = _returnToInitialScale ? _initialScale : _minScale;

            _hitEffectSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Pause()
                .Append(transform.DOScale(_maxScale, halfDuration).SetEase(_ease))
                .Append(transform.DOScale(endScale, halfDuration).SetEase(_ease));
        }
    }
}
