using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace TheNorth
{
    public class InputHandler : MonoBehaviour
    {
        [Header("Character Input Values")]
        public CallbackContext onMove;
        public CallbackContext onLook;
        public CallbackContext onJump;
        public CallbackContext onSprint;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        public bool cursorLocked = true;
        public bool cursorInputForLook = true;

        public void OnMove(CallbackContext value)
        {
            onMove = value;
        }

        public void OnLook(CallbackContext value)
        {
            onMove = value;
        }

        public void OnJump(CallbackContext value)
        {
            onMove = value;
        }

        public void OnSprint(CallbackContext value)
        {
            onSprint = value;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            SetCursorState(cursorLocked);
        }

        private void SetCursorState(bool newState)
        {
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
