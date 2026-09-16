using System;
using System.Threading;
using _Project.Scripts._Common.SceneLoader;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace _Project.Scripts.SceneLoaderService
{
    public sealed class SceneLoader : ISceneLoader
    {
        private readonly ILoadingCurtain _loadingCurtain;

        public event Action LoadingStarted = delegate { };
        public event Action LoadingFinished = delegate { };

        public bool IsLoading { get; private set; }
        public string CurrentSceneName { get; private set; }

        public SceneLoader([InjectOptional] ILoadingCurtain loadingCurtain = null)
        {
            _loadingCurtain = loadingCurtain;
        }

        public async UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            bool useCurtain = true,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                throw new ArgumentException("Scene name is empty.", nameof(sceneName));

            await WaitForCurrentOperationAsync(cancellationToken);

            IsLoading = true;
            LoadingStarted.Invoke();

            try
            {
                if (useCurtain && _loadingCurtain != null)
                    await _loadingCurtain.ShowAsync(cancellationToken);

                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, mode);
                if (operation == null)
                    throw new InvalidOperationException($"Scene load operation was not created: {sceneName}");

                await operation.ToUniTask(cancellationToken: cancellationToken);

                Scene loadedScene = SceneManager.GetSceneByName(sceneName);
                if (loadedScene.IsValid() && loadedScene.isLoaded)
                    SceneManager.SetActiveScene(loadedScene);

                CurrentSceneName = sceneName;

                if (useCurtain && _loadingCurtain != null)
                    await _loadingCurtain.HideAsync(cancellationToken);
            }
            finally
            {
                IsLoading = false;
                LoadingFinished.Invoke();
            }
        }

        public UniTask ReloadSceneAsync(
            string sceneName,
            bool useCurtain = true,
            CancellationToken cancellationToken = default)
        {
            return LoadSceneAsync(sceneName, LoadSceneMode.Single, useCurtain, cancellationToken);
        }

        public async UniTask UnloadSceneAsync(
            string sceneName,
            bool useCurtain = true,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
                throw new ArgumentException("Scene name is empty.", nameof(sceneName));

            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.IsValid() || !scene.isLoaded)
                return;

            await WaitForCurrentOperationAsync(cancellationToken);

            IsLoading = true;
            LoadingStarted.Invoke();

            try
            {
                if (useCurtain && _loadingCurtain != null)
                    await _loadingCurtain.ShowAsync(cancellationToken);

                AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
                if (operation == null)
                    throw new InvalidOperationException($"Scene unload operation was not created: {sceneName}");

                await operation.ToUniTask(cancellationToken: cancellationToken);

                if (CurrentSceneName == sceneName)
                    CurrentSceneName = null;

                if (useCurtain && _loadingCurtain != null)
                    await _loadingCurtain.HideAsync(cancellationToken);
            }
            finally
            {
                IsLoading = false;
                LoadingFinished.Invoke();
            }
        }

        public void AddedActionEndLoaded(Action onEndLoaded)
        {
            LoadingFinished += onEndLoaded;
        }

        public void RemoveActionEndLoaded(Action onEndLoaded)
        {
            LoadingFinished -= onEndLoaded;
        }

        public void AddedActionStartLoaded(Action onStartLoaded)
        {
            LoadingStarted += onStartLoaded;
        }

        public void RemoveActionStartLoaded(Action onStartLoaded)
        {
            LoadingStarted -= onStartLoaded;
        }

        public void Load(string sceneName)
        {
            LoadSceneAsync(sceneName).Forget();
        }

        private UniTask WaitForCurrentOperationAsync(CancellationToken cancellationToken)
        {
            return UniTask.WaitWhile(() => IsLoading, cancellationToken: cancellationToken);
        }
    }
}
