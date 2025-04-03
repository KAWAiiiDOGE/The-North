using UnityEngine;
using Zenject;

namespace TheNorth
{
    public class InputManager : MonoBehaviour
    {
        [Header("Mouse Cursor Settings")]
        public bool IsCursorLockedAndHidden = true;

        [Inject] readonly InputActions _inputActions;

        void OnEnable()
        {
            _inputActions.Enable();
        }
        void OnDisable()
        {
            _inputActions.Disable();
        }
        void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(IsCursorLockedAndHidden && hasFocus);
        }

        public static void SetCursorState(bool isCursorLockedAndHidden)
        {
            Cursor.lockState = isCursorLockedAndHidden ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isCursorLockedAndHidden;
        }
    }
}
