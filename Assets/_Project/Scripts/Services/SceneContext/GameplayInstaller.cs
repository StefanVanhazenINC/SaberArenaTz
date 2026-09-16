using _Project.Scripts.GameFlow;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.SceneContext
{
    public class GameplayInstaller : MonoInstaller
    {
        [SerializeField] private string _levelSceneName = "Arena";

        public override void InstallBindings()
        {
            Container.BindInstance(new GameplaySceneConfig(_levelSceneName)).AsSingle();
            Container.BindInterfacesAndSelfTo<GameFlowService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameTimerService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<MatchStatsService>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<ArenaGameMode>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameplayBootstrap>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameplaySceneController>().AsSingle().NonLazy();
        }
    }
}
