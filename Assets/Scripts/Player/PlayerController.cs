using NaughtyAttributes;
using UnityEngine;
using Zenject;

namespace TheNorth
{
    [SelectionBase]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [field: Header("Look Settings")]
        [field: SerializeField] public bool CanLook { get; set; } = true;
        [field: SerializeField, ShowIf(nameof(CanLook))] public CinemachineCameraTarget CameraPivot { get; private set; }
        [field: SerializeField, ShowIf(nameof(CanLook)), MinMaxSlider(-90, 90)] public Vector2Int VerticalFov { get; private set; } = new Vector2Int(-90, 90);
        [field: SerializeField, ShowIf(nameof(CanLook)), MinMaxSlider(-90, 90)] public Vector2Int HorizontalFov { get; private set; } = new Vector2Int(-90, 90);
        [field: SerializeField, ShowIf(nameof(CanLook))] public bool IgnoreTimeScale { get; private set; } = false;
        [field: SerializeField, ShowIf(nameof(CanLook))] public bool IsInvertX { get; private set; } = false; // TODO: В дальнейшем следует переделать: Input System Package => Look => Processors => Invert Vector2
        [field: SerializeField, ShowIf(nameof(CanLook))] public bool IsInvertY { get; private set; } = false; // TODO: В дальнейшем следует переделать: Input System Package => Look => Processors => Invert Vector2
        [field: SerializeField, ShowIf(nameof(CanLook)), Range(0f, 2.0f)] public float MouseSensitivity { get; private set; } = 0.5f; // TODO: В дальнейшем следует переделать: Input System Package => Look => Processors => Scale Vector2

        [field: Header("Movement Settings")]
        [field: SerializeField] public bool CanMove { get; set; } = true;
        [field: SerializeField, ShowIf(nameof(CanMove)), Min(0f)] public float MoveSpeed { get; private set; } = 8f;
        [field: SerializeField, ShowIf(nameof(CanMove)), Range(0.1f, 20f)] public float MoveAcceleration { get; private set; } = 10.0f;
        [field: SerializeField, ShowIf(nameof(CanMove))] public bool CanMoveInAir { get; private set; } = true;
        [field: SerializeField, ShowIf(nameof(CanMove)), Space(10)] public bool CanSprint { get; private set; } = true;
        [field: SerializeField, ShowIf(EConditionOperator.And, nameof(CanMove), nameof(CanSprint)), Min(1f)] public float SprintSpeedMultiplier { get; private set; } = 2f;

        [field: SerializeField, Space(10)] public bool CanJump { get; set; } = true;
        [field: SerializeField, ShowIf(nameof(CanJump)), Min(0f)] public float JumpHeight { get; private set; } = 8.0f;

        [field: SerializeField, Space(10)] public bool CanCrouch { get; set; } = true;
        [field: SerializeField, ShowIf(nameof(CanCrouch)), Range(0f, 1f)] public float CrouchHeightMultiplier { get; private set; } = 0.5f;
        [field: SerializeField, ShowIf(EConditionOperator.And, nameof(CanMove), nameof(CanCrouch)), Range(0f, 1f)] public float CrouchSpeedMultiplier { get; private set; } = 0.5f;
        [field: SerializeField, ShowIf(nameof(CanMove))] public float HeightAcceleration { get; private set; } = 10.0f;

        [field: Header("Physics Settings")]
        [field: SerializeField] public bool IsAffectedByGravity { get; set; } = true;
        [field: SerializeField, ShowIf(nameof(IsAffectedByGravity)), Range(-20, 0)] public int Gravity { get; private set; } = -10;

        [field: SerializeField, Space(10)] public bool CanPush { get; set; } = true;
        [field: SerializeField, ShowIf(nameof(CanPush)), Range(0, 50)] public int PushPower { get; private set; } = 25;

        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public Vector3 CurrentVelocity { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public float CurrentSpeed { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsMoving { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsSprinting { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsCrouching { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsGrounded { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsCeiling { get; private set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsCollideSides { get; private set; }

        [Inject] readonly InputActions _input;

        CharacterController _characterController;
        Vector3 _moveVelocity;
        Vector3 _moveVelocityVertical;
        Vector3 _moveVelocityHorizontal;
        Vector3 _moveVelocityHorizontalInAir;
        Vector3 _targetDirection;
        Vector3 _defaultCenter;
        Vector3 _defaultCameraPivotPosition;
        float _defaultHeight;
        float _targetRotation;
        float _cameraPitch;
        float _cameraYaw;
        float _speed;
        float _speedMultiplier;
        float _speedMultiplierInAir;
        bool _canStandHere;

        void Awake()
        {
            if (CameraPivot == null)
            {
                foreach (CinemachineCameraTarget child in GetComponentsInChildren<CinemachineCameraTarget>())
                {
                    CameraPivot = child;
                    break;
                }
            }
            _characterController = _characterController != null ? _characterController : GetComponent<CharacterController>();
        }
        void Start()
        {
            _defaultHeight = _characterController.height;
            _defaultCenter = _characterController.center;
            _defaultCameraPivotPosition = CameraPivot.transform.localPosition;
        }
        void Update()
        {
            CheckStates();

            if (CanLook)
                ApplyLook();

            if (IsAffectedByGravity)
                ApplyGravity();

            if (CanCrouch)
                ApplyHeight();

            _speedMultiplier = GetSpeedMultiplier();

            if (!IsGrounded && _moveVelocityHorizontalInAir == Vector3.zero)
                _moveVelocityHorizontalInAir = _moveVelocityHorizontal;

            if (CanJump)
                ApplyJump();

            if (CanMove)
                ApplyMove();

            FinalizeMove();
            _characterController.Move(_moveVelocity * Time.deltaTime);
        }
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!CanPush)
                return;

            Rigidbody body = hit.rigidbody;

            // No rigidbody
            if (body == null || body.isKinematic)
                return;

            // We dont want to push objects below us
            if (hit.moveDirection.y < -0.3f)
                return;

            // Calculate push direction from move direction
            // We only push objects to the sides never up and down
            Vector3 pushDir = new(hit.moveDirection.x, 0f, hit.moveDirection.z);

            // Apply the push
            body.AddForce(pushDir * PushPower);
        }
        void OnDrawGizmos()
        {
            if (IsMoving)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawRay(transform.position + _characterController.center, _moveVelocityHorizontal * 0.5f);
            }
            if (IsCrouching)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawRay(transform.position + new Vector3(0f, _defaultHeight * CrouchHeightMultiplier, 0f), Vector3.up * (_defaultHeight - (_defaultHeight * CrouchHeightMultiplier)));
            }
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + new Vector3(0f, _defaultHeight, 0f), 0.02f);
        }

        void CheckStates()
        {
            CurrentVelocity = _characterController.velocity;
            CurrentSpeed = CurrentVelocity.magnitude;
            IsGrounded = _characterController.isGrounded;
            IsCeiling = (_characterController.collisionFlags & CollisionFlags.Above) != 0;
            IsCollideSides = (_characterController.collisionFlags & CollisionFlags.Sides) != 0;
            IsMoving = CurrentSpeed != 0f;
            IsSprinting = CanSprint && IsGrounded && IsMoving && _input.Player.Sprint.IsPressed() && !IsCrouching;
            IsCrouching = CanCrouch && _input.Player.Crouch.IsPressed();
        }
        void ApplyGravity()
        {
            // Stop vertical velocity dropping when grounded
            if (IsGrounded && _moveVelocityVertical.y < 0f)
            {
                _moveVelocityVertical.y = Gravity * 0.5f;
                _moveVelocityHorizontalInAir = Vector3.zero;
            }

            // Applying gravity in flight
            if (!IsGrounded && _moveVelocityVertical.y > 0f)
                _moveVelocityVertical.y += Gravity * Time.deltaTime;

            // Applying more gravity on landing
            // It makes jump less floaty and more controllable when landing
            if (!IsGrounded && _moveVelocityVertical.y <= 0f)
                _moveVelocityVertical.y += 2f * Gravity * Time.deltaTime;
        }
        void ApplyHeight()
        {
            // UNDONE: В присяде с препятствием над головой, если отжать присяд, меняется _speedMultiplier
            // Checking for overhead obstacles
            Vector3 originInCrouchHeight = transform.position + new Vector3(0f, _defaultHeight * CrouchHeightMultiplier, 0f);
            float distanceToDefaultHeight = _defaultHeight - (_defaultHeight * CrouchHeightMultiplier);
            if (Physics.Raycast(originInCrouchHeight, Vector3.up, distanceToDefaultHeight))
            {
                _canStandHere = false;
                return;
            }
            else
            {
                _canStandHere = true;
            }

            // Accelerate or decelerate to target height
            float currentHeight = _characterController.height;
            Vector3 currentCenter = _characterController.center;
            Vector3 curentCameraPivotPosition = CameraPivot.transform.localPosition;
            float targetHeight;
            Vector3 targetCenter;
            Vector3 targetCameraPivotPosition;
            if (!IsCrouching && _canStandHere)
            {
                targetHeight = _defaultHeight;
                targetCenter = _defaultCenter;
                targetCameraPivotPosition = _defaultCameraPivotPosition;
            }
            else
            {
                targetHeight = CrouchHeightMultiplier * _defaultHeight;
                targetCenter = _defaultCenter - new Vector3(0f, _defaultCenter.y * CrouchHeightMultiplier, 0f);
                targetCameraPivotPosition = _defaultCameraPivotPosition - new Vector3(0f, _defaultCameraPivotPosition.y * CrouchHeightMultiplier, 0f);
            }

            float offset = 0.01f;
            if (currentHeight < targetHeight - offset
                || currentHeight > targetHeight + offset)
            {
                _characterController.height = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * HeightAcceleration);
                _characterController.center = Vector3.Lerp(currentCenter, targetCenter, Time.deltaTime * HeightAcceleration);
                CameraPivot.transform.localPosition = Vector3.Lerp(curentCameraPivotPosition, targetCameraPivotPosition, Time.deltaTime * HeightAcceleration);
            }
            else
            {
                _characterController.height = targetHeight;
                _characterController.center = targetCenter;
                CameraPivot.transform.localPosition = targetCameraPivotPosition;
            }
        }
        void ApplyJump()
        {
            if (IsGrounded)
            {
                if (_input.Player.Jump.IsPressed())
                {
                    // Sqrt(-2 * g * h) = how much velocity needed to reach desired height
                    _moveVelocityVertical.y = Mathf.Sqrt(-2f * Gravity * JumpHeight);
                    _speedMultiplierInAir = _speedMultiplier;

                }
                else
                {
                    _speedMultiplierInAir = GetSpeedMultiplier();
                }
            }
        }
        float GetSpeedMultiplier()
        {
            if (IsCrouching)
                return CrouchSpeedMultiplier;

            if (IsSprinting)
                return SprintSpeedMultiplier;

            return 1f;
        }
        void ApplyMove()
        {
            // Calculating target speed
            float targetSpeed;
            if (IsGrounded)
                targetSpeed = MoveSpeed * _speedMultiplier;
            else
                targetSpeed = MoveSpeed * _speedMultiplierInAir;

            if (_input.Player.Move.ReadValue<Vector2>() == Vector2.zero)
                targetSpeed = 0f;

            // Accelerate or decelerate to target speed
            float currentHorizontalSpeed = new Vector3(CurrentVelocity.x, 0f, CurrentVelocity.z).magnitude;
            float offset = 0.1f;
            if (currentHorizontalSpeed < targetSpeed - offset
                || currentHorizontalSpeed > targetSpeed + offset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * _input.Player.Move.ReadValue<Vector2>().magnitude, Time.deltaTime * MoveAcceleration);
            }
            else
            {
                _speed = targetSpeed;
            }

            // Applying movement
            if (_input.Player.Move.ReadValue<Vector2>() != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(_input.Player.Move.ReadValue<Vector2>().x, _input.Player.Move.ReadValue<Vector2>().y) * Mathf.Rad2Deg + CameraPivot.transform.eulerAngles.y;
            }
            _targetDirection = Quaternion.Euler(0f, _targetRotation, 0f) * Vector3.forward;

            // Finalize horizontal velocity
            _moveVelocityHorizontal = _targetDirection * _speed;
        }
        void FinalizeMove()
        {
            // Keep horizonal velocity in air
            if (!CanMoveInAir && _moveVelocityHorizontalInAir != Vector3.zero)
                _moveVelocityHorizontal = _moveVelocityHorizontalInAir;

            // Check if player under the ceiling
            if (IsCeiling && _moveVelocityVertical.y > 0f)
                _moveVelocityVertical.y = 0f;

            _moveVelocity = _moveVelocityHorizontal + _moveVelocityVertical;

        }
        void ApplyLook()
        {
            _cameraPitch += (IsInvertY ? 1 : -1) * _input.Player.Look.ReadValue<Vector2>().y * MouseSensitivity * (IgnoreTimeScale ? 1f : Time.timeScale);
            _cameraPitch = Mathf.Clamp(_cameraPitch, VerticalFov.x, VerticalFov.y);
            CameraPivot.transform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);

            _cameraYaw += (IsInvertX ? -1 : 1) * _input.Player.Look.ReadValue<Vector2>().x * MouseSensitivity * (IgnoreTimeScale ? 1f : Time.timeScale);
            transform.rotation = Quaternion.Euler(0f, _cameraYaw, 0f);
        }
    }
}
