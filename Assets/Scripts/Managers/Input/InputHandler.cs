using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using static TheNorth.InputActions;

namespace TheNorth
{
    /// <summary>
    /// Содержит контекст и значения InputActions
    /// <para>
    /// Потом стоит переделать, чтобы можно было обращаться как к статику или синглтону
    /// </para>
    /// </summary>
    /// TODO: Сделать обращение к вводу со статического класса
    [RequireComponent(typeof(PlayerInput))]
    public class InputHandler : MonoBehaviour, IPlayerActions
    {
        CallbackContext _move;
        CallbackContext _look;
        CallbackContext _jump;
        CallbackContext _crouch;
        CallbackContext _sprint;
        CallbackContext _attack;
        CallbackContext _interact;
        CallbackContext _next;
        CallbackContext _previous;

        public CallbackContext Move => _move;
        public CallbackContext Look => _look;
        public CallbackContext Jump => _jump;
        public CallbackContext Crouch => _crouch;
        public CallbackContext Sprint => _sprint;
        public CallbackContext Attack => _attack;
        public CallbackContext Interact => _interact;
        public CallbackContext Next => _next;
        public CallbackContext Previous => _previous;
        public Vector2 MoveValue => _move.ReadValue<Vector2>();
        public Vector2 LookValue => _look.ReadValue<Vector2>();
        public bool JumpValue => _jump.ReadValueAsButton();
        public bool CrouchValue => _crouch.ReadValueAsButton();
        public bool SprintValue => _sprint.ReadValueAsButton();
        public bool AttackValue => _attack.ReadValueAsButton();
        public bool InteractValue => _interact.ReadValueAsButton();
        public bool NextValue => _next.ReadValueAsButton();
        public bool PreviousValue => _previous.ReadValueAsButton();

        public void OnMove(CallbackContext context) => _move = context;
        public void OnLook(CallbackContext context) => _look = context;
        public void OnJump(CallbackContext context) => _jump = context;
        public void OnCrouch(CallbackContext context) => _crouch = context;
        public void OnSprint(CallbackContext context) => _sprint = context;
        public void OnAttack(CallbackContext context) => _attack = context;
        public void OnInteract(CallbackContext context) => _interact = context;
        public void OnNext(CallbackContext context) => _next = context;
        public void OnPrevious(CallbackContext context) => _previous = context;
    }
}
