using _Project.Scripts.GameFlow.UI;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.SceneContext
{
    public class GameplayHudInstaller: MonoInstaller
    {
        [SerializeField] private GameResultScreenView _gameResultScreenView;
        [SerializeField] private GameTimerView _gameTimerView;

        public override void InstallBindings()
        {
            Container.Bind<GameResultScreenView>()
                .FromInstance(_gameResultScreenView)
                .AsSingle();
            Container.BindInterfacesAndSelfTo<GameResultScreenPresenter>()
                .AsSingle()
                .NonLazy();

            if (_gameTimerView == null)
                return;

            Container.Bind<GameTimerView>()
                .FromInstance(_gameTimerView)
                .AsSingle();
            Container.BindInterfacesAndSelfTo<GameTimerPresenter>()
                .AsSingle()
                .NonLazy();
        }


    }
}
