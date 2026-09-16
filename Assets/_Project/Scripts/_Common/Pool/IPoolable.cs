using UnityEngine;

namespace _Project.Scripts._Common.Pool
{
    public interface IPoolable
    {
        GameObject GameObject { get; }
        void OnTakeFromPool();
        void OnReturnToPool();
    }
}