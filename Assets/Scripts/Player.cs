using UnityEngine;
using UnityEngine.InputSystem;

namespace TheNorth
{
    [RequireComponent(typeof(CharacterController))]
    public class Player : Entity
    {
        Vector3 _onMoveDirection;
        Vector2 _onLookRotation;
        bool _jumpedThisFrame;

        void Start()
        {
            if (!_characterController)
                _characterController = GetComponent<CharacterController>();
            LockCursor();
        }

        void Update()
        {
            CheckCharacterStates();

            if (AffectedByGravity)
                ApplyGravity();

            if (CanLook)
                CalculateLook(_onLookRotation);

            if (CanMove)
            {
                if (CanSprint)
                    _currentSpeed = CalculateSpeed();

                if (CanCrouch)
                {
                    _currentSpeed = CalculateSpeed();
                    CalculateHeight();
                }

                if (CanJump)
                    CalculateJump(_jumpedThisFrame);

                CalculateMove(_onMoveDirection, _currentSpeed);

                _characterController.Move(_moveVelocity * Time.deltaTime);
            }
        }

        void OnDrawGizmos()
        {
            if (IsMoving)
            {
                // TODO: Написать Gizmos который рисует вектор движения персонажа
                // Gizmos.color = Color.cyan;
                // Vector3 moveDirection = new Vector3(_moveVelocity.x, 0, _moveVelocity.z);
                // Vector3 ray = transform.TransformDirection(Vector3.zero - moveDirection);
                // Gizmos.DrawLine(transform.position, ray);
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _onMoveDirection = new Vector3(context.ReadValue<Vector2>().x, 0, context.ReadValue<Vector2>().y);
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            _onLookRotation += context.ReadValue<Vector2>();
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started && IsGrounded)
                _jumpedThisFrame = true;
            else
                _jumpedThisFrame = false;
        }
        public void OnSprint(InputAction.CallbackContext context)
        {
            IsSprinting = !context.canceled;
        }

        void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}