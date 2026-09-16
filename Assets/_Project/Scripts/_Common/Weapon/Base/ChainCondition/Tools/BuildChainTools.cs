using System.Collections.Generic;

namespace _Project.Scripts._Common.Weapon.Base.ChainCondition.Tools
{
    public class BuildChainTools
    {
        public static IChainCheck BuildChain(List<IChainCheck> listChain )
        {
            for (int i = 0; i < listChain.Count; i++)
                listChain[i]?.ResetLinks();

            IChainCheck head = null;
            IChainCheck tail = null;

            for (int i = 0; i < listChain.Count; i++)
            {
                var check = listChain[i];
                if (check == null) continue;

                if (head == null)
                {
                    head = check;
                    tail = check;
                }
                else
                {
                    tail = tail.SetNext(check);
                }
            }

            return head;
        }
    }
}