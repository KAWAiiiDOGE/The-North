using TheNorth.Events;
using UnityEngine;

namespace TheNorth
{
    /// <summary>
    /// Класс для манипуляций над игровым процессом
    /// </summary>
    public class GameFlowManager
    {
        GameFlowManager() { }

        public float TimeScale
        {
            get => Time.timeScale;
            set
            {
                Time.timeScale = value >= 0 ? value : Time.timeScale;
                EventBus.Events.GameFlowTimeScaleChanged?.Invoke(Time.timeScale);
            }
        }
        public int TargetFPS { get => Application.targetFrameRate; set => Application.targetFrameRate = value; }

        public void ResetTimeScale()
        {
            TimeScale = 1f;
        }
        public void ResetTargetFPS()
        {
            TargetFPS = 0;
        }
    }
}