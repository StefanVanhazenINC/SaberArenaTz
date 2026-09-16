using System;

namespace _Project.Scripts._Common.GameFlow
{
    public interface IGameTimerService
    {
        event Action<float> TimeChanged;

        float ElapsedTime { get; }
        bool IsRunning { get; }

        void StartTimer();
        void StopTimer();
        void ResetTimer();
    }
}
