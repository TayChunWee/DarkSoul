using UnityEngine;

namespace DS
{
    public class PlayerManager : MonoBehaviour
    {
        InputHandler     inputHandler;
        Animator         anim;
        CameraHandler    cameraHandler;
        PlayerLocomotion playerLocomotion;

        public bool isInteracting;
        [Header("Player Flags")]
        public bool isSprinting;
        public bool isInAir;
        public bool isGrounded;

        void Start()
        {
            inputHandler     = GetComponent<InputHandler>();
            anim             = GetComponentInChildren<Animator>();
            playerLocomotion = GetComponent<PlayerLocomotion>();
            cameraHandler    = FindObjectOfType<CameraHandler>();
            if (cameraHandler) cameraHandler.SetTarget(transform);

            // 刚体插值/碰撞模式（防抖 + 防穿透）
            var rb = GetComponent<Rigidbody>();
            if (rb)
            {
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.freezeRotation = true; // 物理别把角色掀翻
            }
        }

        void Update()
        {
            float delta = Time.deltaTime;

            isInteracting = anim.GetBool("isInteracting");
            inputHandler.isInteracting = isInteracting;

            // 1) 采样输入
            inputHandler.TickInput(delta);

            // 2) 水平移动/冲刺/翻滚（非物理）
            playerLocomotion.HandleMovement(delta);
            playerLocomotion.HandleRollingAndSprinting(delta);

            // 冲刺 UI 标志沿用教程：是否按着 Shift
            isSprinting = inputHandler.b_Input;
        }

        void FixedUpdate()
        {
            // 3) 物理帧做落地/贴地与重力
            playerLocomotion.HandleFalling(Time.fixedDeltaTime, playerLocomotion.moveDirection);
        }

        void LateUpdate()
        {
            float delta = Time.deltaTime;

            // 4) 相机统一在 LateUpdate，避免卡顿与闪屏
            if (cameraHandler)
            {
                cameraHandler.FollowTarget(delta);
                cameraHandler.HandleCameraRotation(delta, inputHandler.mouseX, inputHandler.mouseY);
            }

            // 清一次性输入
            inputHandler.ConsumeOneFrameFlags();

            if (isInAir)
                playerLocomotion.inAirTimer += Time.deltaTime;
        }
    }
}
