using System;
using UnityEngine;

namespace TheNorth.Events
{
    /// <summary>
    /// Возможно временная реализация шины событий, пока не найдётся решение лучше
    /// </summary>
    public class EventBus
    {
        #region CORE

        static EventBus s_instance;

        public EventBus()
        {
            //Debug.Log("Events init");
        }

        public static EventBus Events { get => s_instance ??= new EventBus(); }

        [RuntimeInitializeOnLoadMethod]
        static void Init() => s_instance = Events;
        
        #endregion

        // Добавьте внизу свои глобальные ивенты
        // Делайте ивенты автоматическими свойствами, чтобы отображались ссылки в VS
        #region EVENTS

        // Game
        public Action GameStarted { get; set; }
        public Action GameStopped { get; set; }
        public Action GameFailed { get; set; }
        public Action GameEnded { get; set; }
        public Action GamePaused { get; set; }
        public Action GameContinued { get; set; }

        // GameFlow
        public Action<float> GameFlowTimeScaleChanged { get; set; }

        // Player
        public Action PlayerDied { get; set; }
        public Action PlayerReborn { get; set; }


        #endregion
    }
}
