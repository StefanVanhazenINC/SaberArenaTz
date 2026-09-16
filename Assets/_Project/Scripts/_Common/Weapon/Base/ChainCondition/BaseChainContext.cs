using System;
using _Project.Scripts._Common.Weapon.Base.Visual;
using _Project.Scripts.Character;

namespace _Project.Scripts._Common.Weapon.Base.ChainCondition
{
    public abstract class BaseChainContext  // наследуется и прокидывается где то 
    {
        [NonSerialized] public BaseWeapon Weapon;
        /// <summary>Вызывается оружием при старте/ребилде.</summary>
        public virtual void Bind(BaseWeapon weapon)
        {
            Weapon = weapon;
        }
    }
    [System.Serializable]
    public sealed class DefaultChainContext : BaseChainContext { }
    [System.Serializable]
    public class ChainWithVisualContainerContext : BaseChainContext
    {
        //bool данные о спринте и тд 
        //хранит WeaponVisualContainer
        [NonSerialized] public BaseWeaponVisualContainer Visual; // runtime-inject
      
        public override void Bind(BaseWeapon weapon)
        {
            base.Bind(weapon);
            Visual = weapon.WeaponVisualContainer;
        }
    }

    [System.Serializable]
    public class ChainCharacterContainerContext : BaseChainContext
    {

        [NonSerialized] public Player Player;
        public ChainCharacterContainerContext(Player player)
        {
            Player = player;
        }

        public override void Bind(BaseWeapon weapon)
        {
            base.Bind(weapon);
        }
    }
}