using System;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

namespace TheNorth
{
    [RequireComponent(typeof(CinemachineOrbitalFollow))]
    public class CinemachineOrbitalFollowRotator : MonoBehaviour
    {
        [SerializeField] CinemachineOrbitalFollow _cinemachineOrbitalFollow;

        [field: Header("Horizontal Axis")]
        [field: SerializeField, Range(0f, 360f)] public float HorizontalSpeed { get; private set; } = 10f;
        [field: SerializeField] public bool IsHorizontalClockwise { get; private set; } = true;

        float _horizontalStartPosition;

        void Start()
        {
            if (_cinemachineOrbitalFollow == null)
                _cinemachineOrbitalFollow = GetComponent<CinemachineOrbitalFollow>();

            _horizontalStartPosition = _cinemachineOrbitalFollow.HorizontalAxis.Value;

            _cinemachineOrbitalFollow.HorizontalAxis.Value = _horizontalStartPosition;
        }
        void LateUpdate()
        {
            HorizontalRotate();
        }

        void HorizontalRotate()
        {
            _cinemachineOrbitalFollow.HorizontalAxis.Value += (IsHorizontalClockwise ? 1 : -1) * HorizontalSpeed * Time.deltaTime;
            _cinemachineOrbitalFollow.HorizontalAxis.Value = WrapValue(_cinemachineOrbitalFollow.HorizontalAxis.Value, _cinemachineOrbitalFollow.HorizontalAxis.Range);
        }
        float WrapValue(float currentValue, Vector2 clampRange)
        {
            if (currentValue <= clampRange.x)
                return clampRange.y;

            if (currentValue >= clampRange.y)
                return clampRange.x;

            return currentValue;
        }
    }
}
