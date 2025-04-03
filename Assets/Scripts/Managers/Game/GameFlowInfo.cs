using TMPro;
using UnityEngine;
using Zenject;

namespace TheNorth
{
    public class GameFlowInfo : MonoBehaviour
    {
        public TextMeshProUGUI TimeScaleTextValue;
        public TextMeshProUGUI FrameRateTextValue;

        [Inject] readonly GameFlowManager _gameFlowManager;
        [SerializeField] GameFlowManagerEditor _gameFlowManagerEditor;

        void Update()
        {
            TimeScaleTextValue.text = $"{_gameFlowManager.TimeScale}";
            FrameRateTextValue.text = $"{_gameFlowManagerEditor.FPS}";
        }
    }
}
