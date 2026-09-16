using System;
using UnityEngine;

namespace _Project.Scripts.Services.LoadingScreen
{
    public abstract class CurtainGraphic : MonoBehaviour
    {
        public event Action OnFadeShow;
        public event Action OnFadeHide;

        public virtual void FadeShow()
        {
        }

        public virtual void FadeHide()
        {
        }

        public virtual void FadeShowComplete()
        {
            OnFadeShow?.Invoke();
        }

        public virtual void FadeHideComplete()
        {
            OnFadeHide?.Invoke();
        }
    }
}
