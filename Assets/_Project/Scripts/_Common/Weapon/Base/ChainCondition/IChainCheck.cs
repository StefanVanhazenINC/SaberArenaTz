using System;

namespace _Project.Scripts._Common.Weapon.Base.ChainCondition
{
    public interface IChainCheck
    {
        IChainCheck SetNext(IChainCheck next);
        ChainCheckResult Check(BaseChainContext ctx);
        public void ResetLinks();

        public Action Feedback { get; set; }
        // bool CanPass(BaseChainContext ctx, out string reason);
        //как то сохронять последний результат проверки 
    }
}