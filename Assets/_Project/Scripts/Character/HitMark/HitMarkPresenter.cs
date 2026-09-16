using System;
using _Project.Scripts._Common.Weapon.Base;
using Common.Weapon.Damageable;

namespace _Project.Scripts.Character.HitMark
{
    public sealed class HitMarkPresenter : IDisposable
    {
        private readonly WeaponHolder _weaponHolder;
        private readonly IHitMarkView _view;

        public HitMarkPresenter(WeaponHolder weaponHolder, IHitMarkView view)
        {
            _weaponHolder = weaponHolder;
            _view = view;

            _weaponHolder.OnHitTarget += ShowHitMark;
        }

        public void Dispose()
        {
            _weaponHolder.OnHitTarget -= ShowHitMark;
        }

        private void ShowHitMark(IDamageable target)
        {
            bool isKill = target != null && !target.IsCanDamage;
            _view.Show(isKill);
        }
    }
}
