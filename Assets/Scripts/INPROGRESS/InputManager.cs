using UnityEngine;

namespace TheNorth
{
    public class InputManager : MonoBehaviour
    {
        [Header("Mouse Cursor Settings")]
        public bool IsCursorLockedAndHidden = true;

        void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(IsCursorLockedAndHidden && hasFocus);
        }

        void SetCursorState(bool isCursorLockedAndHidden)
        {
            Cursor.lockState = isCursorLockedAndHidden ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !isCursorLockedAndHidden;
        }
    }
}
