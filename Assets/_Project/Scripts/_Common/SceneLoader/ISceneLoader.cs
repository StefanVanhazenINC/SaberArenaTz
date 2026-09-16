using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace _Project.Scripts._Common.SceneLoader
{
    public interface ISceneLoader
    {
        event Action LoadingStarted;
        event Action LoadingFinished;

        bool IsLoading { get; }
        string CurrentSceneName { get; }

        UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            bool useCurtain = true,
            CancellationToken cancellationToken = default);

        UniTask ReloadSceneAsync(
            string sceneName,
            bool useCurtain = true,
            CancellationToken cancellationToken = default);

        UniTask UnloadSceneAsync(
            string sceneName,
            bool useCurtain = true,
            CancellationToken cancellationToken = default);
    }

    public interface ILoadingCurtain
    {
        event Action OnFadeShowComplete;
        event Action OnFadeHideComplete;

        void Show();
        void Hide();
        UniTask ShowAsync(CancellationToken cancellationToken = default);
        UniTask HideAsync(CancellationToken cancellationToken = default);
    }
}
