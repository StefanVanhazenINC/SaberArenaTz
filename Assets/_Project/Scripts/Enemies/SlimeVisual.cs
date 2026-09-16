using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Enemies
{
    public class SlimeVisual : MonoBehaviour
    {
        [Header("PrepareDashSetting")]
        [SerializeField] private Vector3 _sizePrepareDash = new Vector3(1.15f, 0.75f, 1.15f);
        [SerializeField, Min(0.001f)] private float _prepareDuration = 0.12f;

        [Header("ProcessDashSetting")]
        [SerializeField] private Vector3 _sizeProcessDash = new Vector3(1f, 1.15f, 1f);
        [SerializeField, Min(0.001f)] private float _processDuration = 0.1f;

        [Header("EndDashSetting")]
        [SerializeField] private Vector3 _sizeEndDash = Vector3.one;
        [SerializeField, Min(0.001f)] private float _endDuration = 0.12f;

        [SerializeField] private Ease _ease = Ease.OutQuad;

        private Tweener _scaleTweener;

        public void PrepareDash()
        {
            ChangeScale(_sizePrepareDash, _prepareDuration);
        }

        public void DashProcess()
        {
            ChangeScale(_sizeProcessDash, _processDuration);
        }

        public void DashEnd()
        {
            ChangeScale(_sizeEndDash, _endDuration);
        }

        private void OnDisable()
        {
            _scaleTweener?.Kill();
            _scaleTweener = null;
        }

        private void ChangeScale(Vector3 targetScale, float duration)
        {
            _scaleTweener?.Kill();
            _scaleTweener = transform
                .DOScale(targetScale, duration)
                .SetEase(_ease);
        }
    }
}
