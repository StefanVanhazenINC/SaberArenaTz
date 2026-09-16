using System.Runtime.CompilerServices;

namespace _Project.Scripts._Common.Weapon.Base.Data.Context
{
    public class CompositeProvider : IWeaponContextProvider
    {
        private IWeaponContextProvider[] _layers;
        private int _count;
        
        public CompositeProvider(int capacity)
        {
            _layers = new IWeaponContextProvider[capacity];
            _count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear() => _count = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddLayer(IWeaponContextProvider provider)
        {
            _layers[_count++] = provider;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGet<T>(out T value)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_layers[i].TryGet(out value))
                    return true;
            }

            value = default!;
            return false;
        }
        
    }
}