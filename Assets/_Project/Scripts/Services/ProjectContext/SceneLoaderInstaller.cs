using _Project.Scripts.SceneLoaderService;
using _Project.Scripts.Services.LoadingScreen;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Services.ProjectContext
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        [SerializeField] private CurtainGraphic _curtainGraphic;

        public override void InstallBindings()
        {
            if (_curtainGraphic != null)
                Container.Bind<CurtainGraphic>().FromInstance(_curtainGraphic).AsSingle();

            Container.BindInterfacesAndSelfTo<LoadingCurtain>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<SceneLoader>().AsSingle().NonLazy();
        }
    }
}
