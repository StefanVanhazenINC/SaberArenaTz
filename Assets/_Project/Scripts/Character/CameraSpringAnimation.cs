using _Project.Scripts._Common.ProceduralAnimation;
using Alchemy.Inspector;
using UnityEngine;

namespace _Project.Scripts.Character
{
    [HideScriptField]
    public class CameraSpringAnimation : MonoBehaviour
    {
        [SerializeField] private SpringAnimation _springAnimation;
        [SerializeField] private bool _enableAngularDisplacement;
        [SerializeField,ShowIf("_enableAngularDisplacement")] private bool _enableOnlyYDisplacement = true;
        [SerializeField] private bool _enableLinearDisplacement;
        [Tooltip("Смешение относительно положения- наклон")] 
        [SerializeField,ShowIf("_enableAngularDisplacement")]
        private float _angularDisplacement = 2f;
        [Tooltip("Смешение относительно положения- позиция")] 
        [SerializeField,ShowIf("_enableLinearDisplacement")]
        private float _linearDisplacement = 0.05f;

        public void BeforeUpdate()
        {
            if (_enableLinearDisplacement)
            {
                transform.localPosition = Vector3.zero;
            }
        }

        public void UpdateSpring(float deltaTime,Vector3 up)
        {
            var localSpringPosition = _springAnimation.SpringPosition - transform.position;

            if ( _enableAngularDisplacement)
            {
                var springHeight = Vector3.Dot(localSpringPosition, up);
                transform.localEulerAngles = new Vector3(-springHeight * _angularDisplacement, 0, 0f);
            }

            if ( _enableLinearDisplacement)
            {
                if (_enableOnlyYDisplacement )
                {
                    var yDisplacement = new Vector3(0, localSpringPosition.y, 0);
                    transform.localPosition =yDisplacement * _linearDisplacement;

                }
                else
                {
                    transform.localPosition = localSpringPosition * _linearDisplacement;
                }
            }
        }

    }
}