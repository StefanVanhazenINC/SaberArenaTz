using Unity.VisualScripting;

namespace _Project.Scripts._Common.Weapon.Base.Data
{
    public interface IWeaponDataModule
    {
        //модули с дополнительными параметарми , которые будет прохожить через BaseWeaponи модифицировать его 
        public void InstallModule(WeaponConfig config,BaseWeapon weapon);

        //применить разброс или сделать жизнь снаряда рандомной или его скорость    
        //сть события ONUseWepoan, OnUseShootDir, и спавнере OnStartShoot
        /*Допустим разброс :
         * подключаемся к событию OnUseShootDir
         * Случайная жизнь снаряда
         *  подключаемя к событию OnStartShoot , снимаем копию времени , имзеняем ее и
         */

    }
}