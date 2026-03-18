using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace EventBus
{
    public class InputController : MonoBehaviour
    {
        public void OnMove(CallbackContext context)
        {
            if (context.canceled) return;

            Vector2 delta = context.ReadValue<Vector2>();
            if (delta == Vector2.zero) return;

            EventBus.Trigger(new MoveInputEvent(delta));
        }
    }
}