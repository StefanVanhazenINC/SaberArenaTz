using System;
using UnityEngine;

namespace _Project.Scripts._Common.Weapon.Base.ChainCondition
{
    public abstract class ChainCheckBase : IChainCheck//<TCtx> : IChainCheck where TCtx : BaseChainContext
    {
        [System.NonSerialized] private IChainCheck _next;
        public Action Feedback { get; set; }
        public IChainCheck SetNext(IChainCheck next)
        {
            _next = next;
            return next;
        }

        public ChainCheckResult Check(BaseChainContext ctx)
        {
            //if(CanPass(ctx, return r)) return ;
            var r = Evaluate(ctx);
            if (!r.Ok)
            {
                return r;
            }
            Feedback?.Invoke();
            // провал — стоп цепочки
            return _next?.Check(ctx) ?? ChainCheckResult.Pass();
        }

        // public bool CanPass(BaseChainContext ctx, out string reason)
        // {
        //     if (ctx is not TCtx typed)
        //     {
        //         reason = null;
        //         return true; // не применимо
        //     }
        //     return CanPassTyped(typed, out reason);
        // }
        // protected abstract bool CanPassTyped(TCtx ctx, out string reason);

        public void ResetLinks() => _next = null;
        
        protected abstract ChainCheckResult Evaluate(BaseChainContext ctx); 
    }
}
