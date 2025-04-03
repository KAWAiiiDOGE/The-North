using System.Collections;
using NaughtyAttributes;
using TheNorth.Events;
using UnityEngine;
using Zenject;

namespace TheNorth
{
    /// <summary>
    /// Компонент в UnityEditor для манипуляций над игровым процессом
    /// </summary>
    public class GameFlowManagerEditor : MonoBehaviour
    {
        [SerializeField] bool _showInfo = true;
        [SerializeField, ShowIf(nameof(_showInfo))] Canvas _infoCanvas;
        [SerializeField, ContextMenuItem("Reset Time Scale", nameof(ResetTimeScale)), Range(0f, 2f)] float _timeScale = 1f;
        [SerializeField, ContextMenuItem("Reset Target FPS", nameof(ResetTargetFPS)), Range(0, 300)] int _targetFps = 0;
        [SerializeField, MinValue(0f)] float _fpsPollingRate = 0.5f;
        [SerializeField, ReadOnly] int _fps;

        [Inject] readonly GameFlowManager _gameFlowManager;

        public int FPS => _fps; // TODO: переместить в GameFlowManager

        void OnEnable()
        {
            EventBus.Events.GameFlowTimeScaleChanged += OnGameFlowTimeScaleChanged;
        }
        void OnDisable()
        {
            EventBus.Events.GameFlowTimeScaleChanged -= OnGameFlowTimeScaleChanged;
        }
        void Awake()
        {
            _infoCanvas = _infoCanvas != null ? _infoCanvas : gameObject.GetComponentInChildren<Canvas>();
        }
        void Start()
        {
            _gameFlowManager.TimeScale = _timeScale;
            _gameFlowManager.TargetFPS = _targetFps;
            StartCoroutine(FramesPerSecond());
        }
        void Update()
        {
            if (_showInfo)
                _infoCanvas.gameObject.SetActive(_showInfo);
        }
        void OnValidate()
        {
            _timeScale = Mathf.Round(_timeScale * 10f) / 10f;
            if (_gameFlowManager != null)
            {
                _gameFlowManager.TimeScale = _timeScale;
                _gameFlowManager.TargetFPS = _targetFps;
            }
        }

        [Button]
        public void ResetTimeScale()
        {
            _gameFlowManager?.ResetTimeScale();
            _timeScale = 1f;
        }
        [Button]
        public void ResetTargetFPS()
        {
            _gameFlowManager?.ResetTargetFPS();
            _targetFps = 0;
        }

        IEnumerator FramesPerSecond()
        {
            while (Application.isPlaying)
            {
                yield return new WaitForSecondsRealtime(_fpsPollingRate);
                _fps = (int)(1f / Time.unscaledDeltaTime);
            }
        }
        void OnGameFlowTimeScaleChanged(float scale)
        {
            _timeScale = scale;
        }
    }
}