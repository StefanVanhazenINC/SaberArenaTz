using _Project.Scripts._Common.SceneLoader;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.ProjectContext
{
    public class Bootstrap : MonoBehaviour
    {
        [SerializeField] private string _gameplaySceneName = "GameSystem";
        
        [Inject]
        private ISceneLoader _sceneLoader;

     

        private void Start()
        {
            LoadGameplayAsync().Forget();
        }

        private async UniTask LoadGameplayAsync()
        {
            if (_sceneLoader == null)
            {
                Debug.LogError("SceneLoader is not injected into Bootstrap.");
                return;
            }

            if (string.IsNullOrWhiteSpace(_gameplaySceneName))
            {
                Debug.LogError("Gameplay scene name is empty.");
                return;
            }

            await _sceneLoader.LoadSceneAsync(_gameplaySceneName, useCurtain: false);
        }
    }
}
