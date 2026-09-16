using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts.Character.HitMark
{
    public interface IHitMarkView
    {
        void Show(bool isKill);
        void Hide();
    }

    public sealed class HitMarkView : MonoBehaviour, IHitMarkView
    {
        [SerializeField] private GameObject _markRoot;
        [SerializeField] private Graphic[] _markGraphic;
        [SerializeField, Min(0.01f)] private float _showDuration = 0.08f;
        [SerializeField, Min(0f)] private float _maxRandomAngle = 12f;
        [SerializeField] private Vector3 _punchScale = new Vector3(0.12f, 0.12f, 0f);
        [SerializeField, Min(0.01f)] private float _punchDuration = 0.08f;
        [SerializeField] private Color _defaultColor = Color.white;
        [SerializeField] private Color _killColor = Color.red;

        private Coroutine _hideCoroutine;
        private Tween _punchTween;
        private Transform _markTransform;
        private Vector3 _defaultScale;
        private Quaternion _defaultRotation;
        private float _hideAtTime;

        private void Awake()
        {
            CacheReferences();
            Hide();
        }

        public void Show(bool isKill)
        {
            CacheReferences();
            _punchTween?.Kill();
            _punchTween = null;

            _markTransform.localScale = _defaultScale;
            _markTransform.localRotation = _defaultRotation * Quaternion.Euler(0f, 0f, Random.Range(-_maxRandomAngle, _maxRandomAngle));
            
            if (_markGraphic != null)
                for (int i = 0; i < _markGraphic.Length; i++)
                {
                    if (_markGraphic[i] == null)
                        continue;

                    _markGraphic[i].color = isKill ? _killColor : _defaultColor;
                }
            
            _markRoot.SetActive(true);
            _hideAtTime = Time.time + _showDuration;

            _punchTween = _markTransform
                .DOPunchScale(_punchScale, _punchDuration)
                .SetEase(Ease.OutQuad);

            if (_hideCoroutine == null)
                _hideCoroutine = StartCoroutine(HideAfterDelay());
        }

        public void Hide()
        {
            CacheReferences();
            _punchTween?.Kill();
            _punchTween = null;

            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }

            _markTransform.localScale = _defaultScale;
            _markTransform.localRotation = _defaultRotation;
            _markRoot.SetActive(false);
        }

        private void OnDisable()
        {
            _punchTween?.Kill();
            _punchTween = null;

            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
        }

        private IEnumerator HideAfterDelay()
        {
            while (true)
            {
                float remainingTime = _hideAtTime - Time.time;
                if (remainingTime <= 0f)
                    break;

                yield return new WaitForSeconds(remainingTime);
            }

            _hideCoroutine = null;
            Hide();
        }

        private void CacheReferences()
        {
            if (_markRoot == null)
                _markRoot = gameObject;

            if (_markTransform != null)
                return;

            _markTransform = _markRoot.transform;
            _defaultScale = _markTransform.localScale;
            _defaultRotation = _markTransform.localRotation;
        }
    }
}
