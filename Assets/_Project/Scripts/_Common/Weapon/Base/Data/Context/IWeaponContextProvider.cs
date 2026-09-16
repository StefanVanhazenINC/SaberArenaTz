namespace _Project.Scripts._Common.Weapon.Base.Data.Context
{
    public interface IWeaponContextProvider
    {
        bool TryGet<T>(out T value);    
    }
}