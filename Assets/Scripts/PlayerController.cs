using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheNorth
{
    // TODO: Говнокод, нужно сделать отдельный класс для инпута игрока
    [RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        [field: Header("Character Behaviour")]
        [field: SerializeField] public bool CanLook { get; private set; } = true;
        [field: SerializeField, ShowIf("CanLook")] public Transform CameraPivot { get; private set; }
        [field: SerializeField, ShowIf("CanLook"), MinMaxSlider(-90, 90)] public Vector2Int VerticalFov { get; private set; } = new Vector2Int(-90, 90);
        [field: SerializeField, ShowIf("CanLook"), MinMaxSlider(-90, 90)] public Vector2Int HorizontalFov { get; private set; } = new Vector2Int(-90, 90);
        [field: SerializeField, ShowIf("CanLook")] public bool IsInvertX { get; private set; } = false; // TODO: В дальнейшем следует переделать: Input System Package => Look => Processors => Invert Vector2
        [field: SerializeField, ShowIf("CanLook")] public bool IsInvertY { get; private set; } = false; // TODO: В дальнейшем следует переделать: Input System Package => Look => Processors => Invert Vector2
        [field: SerializeField, ShowIf("CanLook"), Range(0f, 2.0f)] public float MouseSensitivity { get; private set; } = 0.5f; // TODO: В дальнейшем следует переделать: Input System Package => Look => Processors => Scale Vector2

        [field: SerializeField, Space(10)] public bool CanMove { get; private set; } = true;
        [field: SerializeField, ShowIf("CanMove"), Min(0f)] public float MoveSpeed { get; private set; } = 8f;
        [field: SerializeField, ShowIf("CanMove"), Range(0f, 20f)] public float SpeedLerpTime { get; private set; } = 10.0f;
        [field: SerializeField, ShowIf("CanMove")] public bool CanSprint { get; private set; } = true;
        [field: SerializeField, ShowIf(EConditionOperator.And, "CanMove", "CanSprint"), Min(1f)] public float SprintSpeedMultiplier { get; private set; } = 2f;

        [field: SerializeField, Space(10)] public bool CanJump { get; private set; } = true;
        [field: SerializeField, ShowIf("CanJump"), Min(0f)] public float JumpHeight { get; private set; } = 8.0f;

        [field: SerializeField, Space(10)] public bool CanCrouch { get; private set; } = true;
        [field: SerializeField, ShowIf("CanCrouch"), Range(0f, 1f)] public float CrouchHeightMultiplier { get; private set; } = 0.5f;
        [field: SerializeField, ShowIf(EConditionOperator.And, "CanMove", "CanCrouch"), Range(0f, 1f)] public float CrouchSpeedMultiplier { get; private set; } = 0.5f;
        [field: SerializeField, ShowIf("CanMove")] public float HeightLerpTime { get; private set; } = 10.0f;

        [field: SerializeField, Space(10)] public bool IsAffectedByGravity { get; private set; } = true;
        [field: SerializeField, ShowIf("IsAffectedByGravity"), Range(-20, 0)] public int Gravity { get; private set; } = -10;

        [field: Header("Audio Clips")]
        [field: SerializeField] public AudioClip LandingAudioClip { get; private set; }
        [field: SerializeField] public AudioClip[] FootstepAudioClips { get; private set; }
        [field: SerializeField, Range(0, 1)] public float FootstepAudioVolume { get; private set; } = 0.5f;

        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public Vector3 CurrentVelocity { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public float CurrentSpeed { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsMoving { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsSprinting { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsGrounded { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsCrouching { get; private set; }

        CharacterController _characterController;
        Vector3 _moveVelocity;
        Vector3 _defaultCenter;
        Vector3 _defaultCameraPivotPosition;
        Vector2 _onLookRotation;
        Vector2 _inputDirection;
        float _defaultHeight;
        float _targetRotation;
        float _rotationX = 0f;
        float _speedMultiplier = 1f;
        float _speed;
        bool _isSprint;
        bool _isJump;
        bool _isCrouch;

        void Start()
        {
            if (CameraPivot == null)
            {
                foreach (Transform child in transform.GetComponentsInChildren<Transform>())
                {
                    if (child.CompareTag("CinemachineTarget"))
                    {
                        CameraPivot = child;
                        break;
                    }
                }
            }

            _characterController = GetComponent<CharacterController>();
            _defaultHeight = _characterController.height;
            _defaultCenter = _characterController.center;
            _defaultCameraPivotPosition = CameraPivot.localPosition;

            LockCursor();
        }
        void Update()
        {
            CheckStates();

            if (IsAffectedByGravity)
                ApplyGravity();

            if (CanCrouch)
                ApplyHeight();

            if (CanJump)
                ApplyJump();
            
            if (CanMove)
                ApplyMove();

            _characterController.Move(_moveVelocity * Time.deltaTime);
        }
        void LateUpdate()
        {
            if (CanLook)
                ApplyLook();
        }
        void OnDrawGizmos()
        {
            //if (IsMoving)
            //{
            //    // TODO: Написать Gizmos который рисует вектор движения персонажа
            //    Gizmos.color = Color.cyan;
            //    Vector3 moveDirection = new Vector3(_moveVelocity.x, 0, _moveVelocity.z);
            //    Vector3 ray = transform.TransformDirection(Vector3.zero - moveDirection);
            //    Gizmos.DrawLine(transform.position, ray);
            //}
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _inputDirection = context.ReadValue<Vector2>();
        }
        public void OnLook(InputAction.CallbackContext context)
        {
            _onLookRotation = context.ReadValue<Vector2>();
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            _isJump = context.ReadValueAsButton();
        }
        public void OnSprint(InputAction.CallbackContext context)
        {
            _isSprint = context.ReadValueAsButton();
        }
        public void OnCrouch(InputAction.CallbackContext context)
        {
            _isCrouch = context.ReadValueAsButton();
        }

        void CheckStates()
        {
            CurrentVelocity = _characterController.velocity;
            CurrentSpeed = CurrentVelocity.magnitude;
            IsGrounded = _characterController.isGrounded;
            IsMoving = CurrentVelocity.magnitude != 0f;
            IsSprinting = CanSprint && IsMoving && _isSprint && !IsCrouching;
            IsCrouching = CanCrouch && _isCrouch;
        }
        void ApplyGravity()
        {
            // Stop vertical velocity dropping when grounded
            if (IsGrounded && _moveVelocity.y < 0f)
                _moveVelocity.y = Gravity * 0.5f;

            if (!IsGrounded)
                _moveVelocity.y += Gravity * Time.deltaTime;
        }
        void ApplyHeight()
        {
            //// UNDONE: Доделать детекцию потолка, возможно нужен статус IsInCrouching
            //if (IsCrouching && Physics.Raycast(transform.position + _defaultCenter, Vector3.up, _defaultHeight + 4f))
            //{
            //    Debug.Log("Obstacle Above!");
            //    return;
            //}

            float targetHeight = IsCrouching ? CrouchHeightMultiplier * _defaultHeight : _defaultHeight;
            float currentHeight = _characterController.height;
            Vector3 targetCenter = IsCrouching ? _defaultCenter - new Vector3(0f, _defaultCenter.y * CrouchHeightMultiplier, 0f) : _defaultCenter;
            Vector3 currentCenter = _characterController.center;
            Vector3 targetCameraPivotPosition = IsCrouching ? _defaultCameraPivotPosition - new Vector3(0f, _defaultCameraPivotPosition.y * CrouchHeightMultiplier, 0f) : _defaultCameraPivotPosition;
            Vector3 curentCameraPivotPosition = CameraPivot.localPosition;
            float offset = 0.01f;
            
            if (currentHeight < targetHeight - offset
                || currentHeight > targetHeight + offset)
            {
                _characterController.height = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * HeightLerpTime);
                _characterController.center = Vector3.Lerp(currentCenter, targetCenter, Time.deltaTime * HeightLerpTime);
                CameraPivot.localPosition = Vector3.Lerp(curentCameraPivotPosition, targetCameraPivotPosition, Time.deltaTime * HeightLerpTime);
            }
            else
            {
                _characterController.height = targetHeight;
                _characterController.center = targetCenter;
                CameraPivot.localPosition = targetCameraPivotPosition;
            }
        }
        void ApplyJump()
        {
            if (IsGrounded && _isJump)
            {
                // Sqrt(H * -2 * G) = how much velocity needed to reach desired height
                _moveVelocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }
        float ApplySpeed()
        {
            if (CanCrouch && IsCrouching)
                return CrouchSpeedMultiplier;
            
            if (CanSprint && IsSprinting)
                return SprintSpeedMultiplier;

            return 1f;
        }
        void ApplyMove()
        {
            _speedMultiplier = ApplySpeed();

            float targetSpeed = MoveSpeed * _speedMultiplier;
            if (_inputDirection == Vector2.zero)
                targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(CurrentVelocity.x, 0.0f, CurrentVelocity.z).magnitude;
            float offset = 0.1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - offset
                || currentHorizontalSpeed > targetSpeed + offset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * _inputDirection.magnitude, Time.deltaTime * SpeedLerpTime);
                // round 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) * 0.001f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_inputDirection.x, 0.0f, _inputDirection.y);
            if (_inputDirection != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + CameraPivot.eulerAngles.y;
                //float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                //    RotationSmoothTime);

                //// rotate to face input direction relative to camera position
                //transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }
            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _moveVelocity = (targetDirection * _speed) + new Vector3(0, _moveVelocity.y, 0);

        }
        void ApplyLook()
        {
            _rotationX += (IsInvertX ? 1 : -1) * _onLookRotation.y * MouseSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, VerticalFov.x, VerticalFov.y);
            CameraPivot.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, _onLookRotation.x * MouseSensitivity, 0);
        }
        void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
