using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TheNorth
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class Entity : MonoBehaviour
    {
        [field: Header("Entity Behaviour")]
        [field: SerializeField] public bool CanLook { get; protected set; } = true;
        [field: SerializeField, ShowIf("CanLook"), MinMaxSlider(-90f, 90f)] public Vector2Int VerticalFov { get; protected set; } = new Vector2Int(-90, 90);
        [field: SerializeField, ShowIf("CanLook"), MinMaxSlider(-90f, 90f)] public Vector2Int HorizontalFov { get; protected set; } = new Vector2Int(-90, 90);

        [field: SerializeField, Space(5)] public bool CanMove { get; protected set; } = true;
        [field: SerializeField, ShowIf("CanMove"), Min(0f)] public float MoveSpeed { get; protected set; } = 8f;

        [field: SerializeField, Space(5), EnableIf("CanMove")] public bool CanSprint { get; protected set; } = true;
        [field: SerializeField, ShowIf(EConditionOperator.And, "CanMove", "CanSprint"), Min(1f)] public float SprintSpeedMultiplier { get; protected set; } = 2f;

        [field: SerializeField, Space(5)] public bool CanJump { get; protected set; } = true;
        [field: SerializeField, ShowIf("CanJump"), Min(0f)] public float JumpHeight { get; protected set; } = 8.0f;

        [field: SerializeField, Space(5)] public bool CanCrouch { get; protected set; } = true;
        [field: SerializeField, ShowIf("CanCrouch"), Range(0f, 1f)] public float CrouchHeightMultiplier { get; protected set; } = 0.5f;
        [field: SerializeField, ShowIf(EConditionOperator.And, "CanMove", "CanCrouch"), Range(0f, 1f)] public float CrouchSpeedMultiplier { get; protected set; } = 0.5f;

        [field: SerializeField, Space(5)] public bool CanAttack { get; protected set; } = true; // ? мейби лучше описать в другом классе

        [field: SerializeField, Space(5)] public bool CanInteract { get; protected set; } = true; // ? мейби лучше описать в другом классе

        [field: SerializeField, Space(5)] public bool AffectedByGravity { get; protected set; } = true;
        [field: SerializeField, ShowIf("AffectedByGravity"), Range(-20, 0)] public int GravityValue { get; protected set; } = -10;

        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public Vector3 CurrentVelocity { get; protected set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsMoving { get; protected set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsSprinting { get; protected set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsGrounded { get; protected set; }
        [field: SerializeField, Foldout("Dynamic Parameters"), ReadOnly] public bool IsCrouching { get; protected set; }

        [SerializeField] protected Transform _eyesTransform;
        [SerializeField] protected CharacterController _characterController;
        protected Vector3 _moveVelocity;
        protected float _currentSpeed;

        protected virtual void CheckCharacterStates()
        {
            CurrentVelocity = _characterController.velocity;
            IsMoving = CurrentVelocity.magnitude != 0f;
            IsSprinting = IsMoving && IsSprinting && IsGrounded;
            IsGrounded = _characterController.isGrounded;
        }
        protected virtual void ApplyGravity()
        {
            if (IsGrounded && _moveVelocity.y < 0f)
                _moveVelocity.y = GravityValue;

            if (!IsGrounded)
                _moveVelocity.y += GravityValue * Time.deltaTime;
        }
        protected virtual float CalculateSpeed()
        {
            if (IsCrouching)
                return CrouchSpeedMultiplier * MoveSpeed;

            return IsSprinting ? SprintSpeedMultiplier * MoveSpeed : MoveSpeed;
        }
        protected virtual void CalculateHeight()
        {

        } // UNDONE: CalculateHeight
        protected virtual void CalculateJump(bool jumpedThisFrame)
        {
            if (IsGrounded && jumpedThisFrame)
                _moveVelocity.y = JumpHeight;
        }
        protected virtual void CalculateMove(Vector3 localDirection, float speed)
        {
            Vector3 worldX = transform.TransformDirection(Vector3.right);
            Vector3 worldY = new(0, _moveVelocity.y, 0);
            Vector3 worldZ = transform.TransformDirection(Vector3.forward);

            float curSpeedX = speed * localDirection.x;
            float curSpeedZ = speed * localDirection.z;

            _moveVelocity = (worldZ * curSpeedZ) + worldY + (worldX * curSpeedX);
        }
        protected virtual void CalculateLook(Vector2 rotation)
        {
            float _rotationX = -1 * rotation.y;
            _rotationX = Mathf.Clamp(_rotationX, -VerticalFov.y, VerticalFov.x);
            _eyesTransform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, rotation.x, 0);
        }
    }
}