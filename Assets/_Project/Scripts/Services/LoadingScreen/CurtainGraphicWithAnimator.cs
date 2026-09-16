using UnityEngine;

namespace _Project.Scripts.Services.LoadingScreen
{
    public sealed class CurtainGraphicWithAnimator : CurtainGraphic
    {
        private static readonly int StartAnimationTrigger = Animator.StringToHash("Start");
        private static readonly int EndAnimationTrigger = Animator.StringToHash("End");

        [SerializeField] private Animator _animator;

        public override void FadeShow()
        {
            _animator?.SetTrigger(StartAnimationTrigger);
        }

        public override void FadeHide()
        {
            _animator?.SetTrigger(EndAnimationTrigger);
        }
    }
}
