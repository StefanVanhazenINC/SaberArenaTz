using System;
using System.Threading;
using _Project.Scripts._Common.SceneLoader;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace _Project.Scripts.Services.LoadingScreen
{
    public sealed class LoadingCurtain : ILoadingCurtain, IDisposable
    {
        private readonly CurtainGraphic _injectedGraphic;
        private CurtainGraphic _graphic;

        public event Action OnFadeShowComplete = delegate { };
        public event Action OnFadeHideComplete = delegate { };

        public LoadingCurtain([InjectOptional] CurtainGraphic graphic = null)
        {
            _injectedGraphic = graphic;
            SetGraphic(_injectedGraphic);
        }

        public void Show()
        {
            GetGraphic()?.FadeShow();
        }

        public void Hide()
        {
            GetGraphic()?.FadeHide();
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            CurtainGraphic graphic = GetGraphic();
            if (graphic == null)
                return;

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();

            void Complete()
            {
                graphic.OnFadeShow -= Complete;
                completionSource.TrySetResult();
            }

            graphic.OnFadeShow += Complete;
            graphic.FadeShow();

            using (cancellationToken.Register(() =>
                   {
                       graphic.OnFadeShow -= Complete;
                       completionSource.TrySetCanceled(cancellationToken);
                   }))
            {
                await completionSource.Task;
            }
        }

        public async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            CurtainGraphic graphic = GetGraphic();
            if (graphic == null)
                return;

            UniTaskCompletionSource completionSource = new UniTaskCompletionSource();

            void Complete()
            {
                graphic.OnFadeHide -= Complete;
                completionSource.TrySetResult();
            }

            graphic.OnFadeHide += Complete;
            graphic.FadeHide();

            using (cancellationToken.Register(() =>
                   {
                       graphic.OnFadeHide -= Complete;
                       completionSource.TrySetCanceled(cancellationToken);
                   }))
            {
                await completionSource.Task;
            }
        }

        public void Dispose()
        {
            SetGraphic(null);
        }

        private CurtainGraphic GetGraphic()
        {
            if (_graphic != null)
                return _graphic;

            SetGraphic(_injectedGraphic != null
                ? _injectedGraphic
                : Object.FindFirstObjectByType<CurtainGraphic>(FindObjectsInactive.Include));

            return _graphic;
        }

        private void SetGraphic(CurtainGraphic graphic)
        {
            if (_graphic == graphic)
                return;

            if (_graphic != null)
            {
                _graphic.OnFadeShow -= FadeShowComplete;
                _graphic.OnFadeHide -= FadeHideComplete;
            }

            _graphic = graphic;

            if (_graphic == null)
                return;

            _graphic.OnFadeShow += FadeShowComplete;
            _graphic.OnFadeHide += FadeHideComplete;
        }

        private void FadeShowComplete()
        {
            OnFadeShowComplete.Invoke();
        }

        private void FadeHideComplete()
        {
            OnFadeHideComplete.Invoke();
        }
    }
}
