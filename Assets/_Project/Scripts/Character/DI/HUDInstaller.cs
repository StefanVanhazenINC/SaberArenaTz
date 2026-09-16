using _Project.Scripts._Common.Weapon.Damageable.HealthSystem.UI;
using _Project.Scripts.Character.Data;
using _Project.Scripts.Character.HitMark;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Character.DI
{
    public class HUDInstaller: MonoInstaller
    {
        [SerializeField] private HealthBarView _healthBarView;
        [SerializeField] private HealthBarView _armorBarView;
        [SerializeField] private HitMarkView _hitMarkView;
        public override void InstallBindings()
        {
            CharacterData data =  Container.Resolve<CharacterData>();
            Container.Bind<HealthBarPresenter>().WithId("Health").AsCached().WithArguments(_healthBarView,data.health).NonLazy();
            Container.Bind<HealthBarPresenter>().WithId("Armor").AsCached().WithArguments(_armorBarView,data.armor).NonLazy();

            if (_hitMarkView != null)
            {
                Container.Bind<IHitMarkView>().FromInstance(_hitMarkView).AsSingle();
                Container.BindInterfacesAndSelfTo<HitMarkPresenter>().AsSingle().NonLazy();
            }
        }
    }
}
