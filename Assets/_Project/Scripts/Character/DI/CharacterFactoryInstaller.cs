using _Project.Scripts.Character.SpawnSystem;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Character.DI
{
    public class CharacterFactoryInstaller : MonoInstaller
    {
        [SerializeField] private PlayerMarker _characterPrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlayerSpawnPointProvider>().AsSingle();
            Container.BindFactory<PlayerMarker, CharacterFactory>().FromComponentInNewPrefab(_characterPrefab);
        }
    }
}
