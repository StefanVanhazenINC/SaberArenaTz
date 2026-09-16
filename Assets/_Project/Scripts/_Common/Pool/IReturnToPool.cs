using System;

namespace _Project.Scripts._Common.Pool
{
    public interface IReturnToPool<T>
    {
        event Action<T> ReturnRequested;
    }
}