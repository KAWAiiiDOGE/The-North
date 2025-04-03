using NaughtyAttributes;
using TheNorth.Events;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace TheNorth
{
    public class PauseMenu : MonoBehaviour
    {
        [Scene] public int MainMenuScene = 0;
        [SerializeField] Canvas _pauseCanvas;
        [SerializeField, Range(0f, 1f)] float _pauseTimeScale = 0f;

        [Inject] readonly InputActions _input;
        [Inject] readonly GameFlowManager _gameFlowManager;

        float _timeScaleBeforePause = 1f;
        bool _isInPause = false;

        void Start()
        {
            _timeScaleBeforePause = _gameFlowManager.TimeScale;
            PauseGame(false);
        }
        void Update()
        {
            if (_input.Global.Pause.WasReleasedThisFrame())
                PauseGame(!_isInPause);
        }

        public void PauseGame(bool isPause)
        {
            _isInPause = isPause;
            InputManager.SetCursorState(!isPause);
            _pauseCanvas.gameObject.SetActive(isPause);

            _timeScaleBeforePause = isPause ? _gameFlowManager.TimeScale : _timeScaleBeforePause;
            _gameFlowManager.TimeScale = isPause ? _pauseTimeScale : _timeScaleBeforePause;

            EventBus.Events.GamePaused?.Invoke();
        }

        public void Continue_Button()
        {
            PauseGame(false);
        }
        public void Settings_Button()
        {

        }
        public void Menu_Button()
        {
            SceneManager.LoadScene(MainMenuScene);
        }
        public void Exit_Button()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}