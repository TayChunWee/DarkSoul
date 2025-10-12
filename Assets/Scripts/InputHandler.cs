using UnityEngine;
using UnityEngine.InputSystem;

namespace DS
{
    public class InputHandler : MonoBehaviour
    {
        // === public（与教程一致的快照变量） ===
        public float horizontal;
        public float vertical;
        public float moveAmount;
        public float mouseX;
        public float mouseY;

        // 教程里的 “b_Input/rollInputTimer/sprintFlag/rollFlag”
        public bool  b_Input;          // 当前是否按着冲刺键（Shift）
        public bool  rollFlag;         // 本帧是否触发了翻滚（LateUpdate 清）
        public bool  sprintFlag;       // 本帧是否满足“可冲刺”
        public float rollInputTimer;   // 仅保留计时用，不改变你的逻辑
        public bool  isInteracting;    // 由 PlayerManager 同步

        // 私有
        PlayerControls inputActions;
        Vector2 movementInput;
        Vector2 cameraInput;
        InputAction rollAction;
        InputAction sprintAction;

        // === 生命周期（与教程一致） ===
        public void OnEnable()
        {
            if (inputActions == null)
            {
                inputActions = new PlayerControls();

                inputActions.PlayerMovement.Movement.performed += ctx => movementInput = ctx.ReadValue<Vector2>();
                inputActions.PlayerMovement.Movement.canceled  += ctx => movementInput = Vector2.zero;

                inputActions.PlayerMovement.Camera.performed   += ctx => cameraInput  = ctx.ReadValue<Vector2>();
                inputActions.PlayerMovement.Camera.canceled    += ctx => cameraInput  = Vector2.zero;

                rollAction   = inputActions.PlayerActions.Roll;
                sprintAction = inputActions.PlayerActions.Sprint;
            }
            inputActions.Enable();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible   = false;
        }

        private void OnDisable()
        {
            inputActions?.Disable();
        }

        // === 教程式入口：由 PlayerManager.Update 调用 ===
        public void TickInput(float delta)
        {
            MoveInput(delta);
            HandleRollInput(delta);
        }

        // === 与截图一致的方法名 ===
        public void MoveInput(float delta)
        {
            horizontal = movementInput.x;
            vertical   = movementInput.y;
            moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            mouseX     = cameraInput.x;
            mouseY     = cameraInput.y;
        }

        // Shift=冲刺（b_Input/sprintFlag），右键=翻滚（rollFlag）
        public void HandleRollInput(float delta)
        {
            // 冲刺键是否按着（教程里的 b_Input）
            b_Input =
                (sprintAction != null && sprintAction.IsPressed()) ||
                (Keyboard.current != null &&
                 (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed));

            // 你原来的“移动时才冲刺”的限制仍然在（不改内容）
            sprintFlag = b_Input && moveAmount > 0.1f && !isInteracting;

            // 右键触发翻滚（保留 rollInputTimer 计时，但翻滚用“刚按下”就触发）
            bool rmbDown = Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame;
            bool rmbHeld = Mouse.current != null && Mouse.current.rightButton.isPressed;

            if (rmbDown && !isInteracting) rollFlag = true;
            rollInputTimer = rmbHeld ? (rollInputTimer + delta) : 0f;
        }

        // 与教程相同：LateUpdate 后清一次性标志
        public void ConsumeOneFrameFlags()
        {
            rollFlag   = false;
            sprintFlag = false;
        }
    }
}
