using System.Collections;
using DG.Tweening;
using Alchemy.Inspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Project.Scripts._Common.Weapon.Damageable.HealthSystem.UI
{
    [HideScriptField]
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private bool _showBar;
        [SerializeField] private bool _showNubmnerValue;
        
        [SerializeField,ShowIf("_showBar")] private Slider _mainSlider;
        [SerializeField,ShowIf("_showBar")] private Slider _additionSlider;
        [SerializeField,ShowIf("_showBar")] private float _timeAnimation = 0.1f;
        [SerializeField,ShowIf("_showBar")] private float _timeToStartAnimation = 0.2f;//������� ������ ������ ������� � ����� �� ������ �������� 
        
        [SerializeField,ShowIf("_showNubmnerValue")] private TMP_Text _currentHealth;
        [SerializeField,ShowIf("_showNubmnerValue")] private TMP_Text _maxHealth;


        private Vector2 _defaulAchorPosition;
         
        private Tweener _punchTweener;
        private Coroutine _delayCo;

        private void Awake()
        {
           // _punchTweener = _mainSlider.rectTransform.DOPunchAnchorPos(new Vector2(10f,10f),_timeAnimation).SetAutoKill(false);
            _punchTweener.Pause();
        }

        public void SetMaxVaueSlider()
        {
            if (!_showBar) return; 
            
            _mainSlider.value = 1;
            //_mainSlider.fillAmount = 1;
            _additionSlider.value = 1;
            //_additionSlider.fillAmount = 1;
        }
        public void SetTextHealth(string current,string max) 
        {
            if (!_showNubmnerValue) return; 
            
            _currentHealth.text = current;
            if ( _maxHealth!=null)
            {
                _maxHealth.text = max;
            }

        }
        public void ChangeMainSlider(float value)
        {
            if (!_showBar) return; 
            if (_delayCo == null && gameObject.activeInHierarchy)
            {
                _delayCo = StartCoroutine(DelayAddtionslider());
            }
            else
            {
               // StopCoroutine(DelayAddtionslider());
            }
            
            //не запускать если уже играет 
           // _mainSlider.rectTransform.DOPunchAnchorPos(new Vector2(10f,10f),_timeAnimation);
           _punchTweener.Restart();
            if (_delayCo == null && gameObject.activeInHierarchy)
            {
                _delayCo = StartCoroutine(DelayAddtionslider());
            }
            _mainSlider.value = value;

            
        }


        public void ResetSlider()
        {
            if (!_showBar) return; 
            _mainSlider.value = 1;
            _additionSlider.value  = 1;
        }
        private IEnumerator DelayAddtionslider()
        {
            yield return new WaitForSeconds(_timeToStartAnimation);
           // _additionSlider.DOFillAmount(_mainSlider.fillAmount, _timeAnimation);
        }

        private void OnDestroy()
        {
            StopCoroutine(_delayCo);
        }
    }
}
