namespace _Project.Scripts._Common.Weapon.Base.Projectile
{
    public interface IPoolProjectile
    {
        public IProjectile GetObjectInPool(IProjectile prefab);
    }
}