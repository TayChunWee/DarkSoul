using UnityEngine;

namespace DS
{
    public class PlayerLocomotion : MonoBehaviour
    {
        PlayerManager   playerManager;
        Transform       cameraObject;
        InputHandler    inputHandler;
        AnimatorHandler animatorHandler;

        [HideInInspector] public Transform myTransform;
        public  new Rigidbody rigidbody;

        [Header("Ground & Air Stats")]
        [Tooltip("从角色中心向上抬起多少米作为地面射线/球投的起点（避免从地面内部发射导致误判）。")]
        [SerializeField] float groundDetectionRayStartPoint = 0.5f;

        [Tooltip("向下探测地面的最大距离（米）。值越大越容易认为脚下有地面；0.45~0.60 更适合平台/台阶。")]
        [SerializeField] float minimumDistanceNeededToBeginFall = 1.0f;

        [Tooltip("地面探测时，按当前移动方向向前偏移的距离（米）。用于站在边缘时更稳地判定脚下地面。")]
        [SerializeField] float groundDirectionRayDistance = 0.2f;

        [Tooltip("把刚体 Y 轴吸附到地面的速度（米/秒）。数值大可减少“脚陷/抖动”，通常 15~30。")]
        [SerializeField] float groundSnapSpeed = 20f;

        [Tooltip("起跳/掉落在空中至少这么久才播放明显的落地动作（太小会频繁播，太大会显得拖沓）。")]
        [SerializeField] float landMinAirTime = 0.25f;

        LayerMask ignoreForGroundCheck;

        [Tooltip("调试查看：角色在空中的累计时间（秒）。运行时自动更新，不建议在 Inspector 修改。")]
        public float inAirTimer;

        [HideInInspector] public Vector3 moveDirection;
        Vector3 normalVector = Vector3.up;

        [Header("Stats")]
        [Tooltip("普通移动速度（米/秒）。")]
        [SerializeField] float movementSpeed = 5f;

        [Tooltip("冲刺速度（米/秒）。")]
        [SerializeField] float sprintSpeed   = 7f;

        [Tooltip("角色朝向追随移动方向的插值速度。数值越大，转身越干脆。")]
        [SerializeField] float rotationSpeed = 10f;

        [Tooltip("空中向下的加速度（代替旧的 fallingSpeed）。调高会下坠更快更“重”。")]
        [SerializeField] float fallingAccel  = 55f;

        [Header("Air Control")]
        [Tooltip("空中对水平速度的轻微控制力度（加速度）。0 关闭；建议 0~3，太大会出现“空中前冲”。")]
        [SerializeField] float airControlAccel = 0f;

        [Header("Roll (code-driven)")]
        [Tooltip("翻滚总位移（米）。")]
        [SerializeField] float rollDistance = 2.5f;

        [Tooltip("翻滚总时长（秒）。")]
        [SerializeField] float rollDuration = 0.35f;

        [Tooltip("翻滚速度曲线（0~1 时间 → 速度倍率）。留空则匀速。")]
        [SerializeField] AnimationCurve rollSpeedCurve;

        [Header("Roll Cooldown")]
        [Tooltip("翻滚结束后的冷却时间（秒）。0 表示无冷却。")]
        [SerializeField] float rollCooldown = 0.40f;

        [Tooltip("“土狼时间”（秒）：短暂离地也不立刻进入 Falling，超过该时间才算真的掉落。")]
        [SerializeField] float fallCoyoteTime = 0.12f;

        [Tooltip("地面探测的左右侧探头偏移（米）。越大越不易在边缘误判掉落，但太大会误踩相邻面。")]
        [SerializeField] float groundProbeSide = 0.18f;

        [Tooltip("探测球半径系数：以胶囊半径×该系数作为 SphereCast 的半径。0.85~0.95 较稳。")]
        [SerializeField] float groundProbeRadiusFactor = 0.9f;

        float ungroundedTimer = 0f;

        bool   isRolling;
        float  rollTimer;
        Vector3 rollDir;
        float  rollCooldownTimer = 0f;

        void Start()
        {
            playerManager   = GetComponent<PlayerManager>();
            rigidbody       = GetComponent<Rigidbody>();
            inputHandler    = GetComponent<InputHandler>();
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            cameraObject    = Camera.main ? Camera.main.transform : null;
            myTransform     = transform;

            animatorHandler.Initialize();

            // 忽略层（按你的项目来）
            ignoreForGroundCheck = ~(1 << 8 | 1 << 11);
            if (playerManager) playerManager.isGrounded = true;
        }

        bool ProbeGround(Vector3 moveDir, out RaycastHit bestHit)
        {
            var col = GetComponent<CapsuleCollider>();
            float colRadius = col ? col.radius : 0.25f;
            float radius    = Mathf.Max(0.02f, colRadius * groundProbeRadiusFactor);
            float maxDist   = minimumDistanceNeededToBeginFall;

            // 脚踝上方一点
            float ankle     = colRadius + 0.05f;
            Vector3 baseOrg = rigidbody.position + Vector3.up * ankle;

            // 按移动方向前探
            Vector3 ahead = (moveDir.sqrMagnitude > 1e-4f)
                ? moveDir.normalized * groundDirectionRayDistance
                : Vector3.zero;
            Vector3 center = baseOrg + ahead;

            Vector3[] origins =
            {
                center,
                center + myTransform.right *  groundProbeSide,
                center + myTransform.right * -groundProbeSide,
            };

            bool hitAny = false;
            bestHit = new RaycastHit();
            float bestY = float.NegativeInfinity;

            foreach (var o in origins)
            {
                if (Physics.SphereCast(o, radius, Vector3.down, out var h, maxDist,
                                       ignoreForGroundCheck, QueryTriggerInteraction.Ignore))
                {
                    hitAny = true;
                    if (h.point.y > bestY) { bestY = h.point.y; bestHit = h; }
                }
            }
            return hitAny;
        }

        public void HandleMovement(float delta)
        {
            if (isRolling) return;

            if (inputHandler.isInteracting)
            {
                rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f);
                animatorHandler.UpdateAnimatorValues(0f, 0f, false);
                return;
            }

            moveDirection = Vector3.zero;
            if (cameraObject)
            {
                moveDirection  = cameraObject.forward * inputHandler.vertical;
                moveDirection += cameraObject.right   * inputHandler.horizontal;
                moveDirection.y = 0f;
                moveDirection.Normalize();
            }

            float speed = (inputHandler.sprintFlag ? sprintSpeed : movementSpeed);
            Vector3 v = moveDirection * speed;
            rigidbody.velocity = new Vector3(v.x, rigidbody.velocity.y, v.z);

            animatorHandler.UpdateAnimatorValues(inputHandler.moveAmount, inputHandler.horizontal, inputHandler.sprintFlag);

            if (inputHandler.moveAmount > 0.01f)
                HandleRotation(delta);
        }

        public void HandleRollingAndSprinting(float delta)
        {
            if (animatorHandler.anim.GetBool("isInteracting"))
                return;

            if (isRolling)
            {
                ApplyRollMovement(delta);
                return;
            }

            if (inputHandler.rollFlag && CanRoll())
            {
                StartRoll(myTransform.forward, "Roll_Front"); // 只前滚
            }
        }

        public void HandleFalling(float delta, Vector3 moveDir)
        {
            // —— 只在 FixedUpdate 调用 —— //
            RaycastHit hit;

            // 前向探测：贴墙时不要向前加力
            Vector3 origin = myTransform.position;
            origin.y += groundDetectionRayStartPoint;
            if (Physics.Raycast(origin, myTransform.forward, out hit, 0.4f,
                                ignoreForGroundCheck, QueryTriggerInteraction.Ignore))
            {
                moveDir = Vector3.zero;
            }

            // 空中：只加垂直加速度 + 可选极小水平控制
            if (playerManager.isInAir)
            {
                rigidbody.AddForce(Vector3.down * fallingAccel, ForceMode.Acceleration);
                if (airControlAccel > 0f && moveDir.sqrMagnitude > 0f)
                    rigidbody.AddForce(moveDir.normalized * airControlAccel, ForceMode.Acceleration);
            }

            // 多探头判地
            if (ProbeGround(moveDir, out hit))
            {
                ungroundedTimer = 0f;

                // 贴地：只修 Y
                float targetY = hit.point.y;
                Vector3 pos   = rigidbody.position;
                pos.y = Mathf.MoveTowards(pos.y, targetY, groundSnapSpeed * delta);
                rigidbody.MovePosition(pos);

                if (!playerManager.isGrounded) playerManager.isGrounded = true;

                // 从空中回地面
                if (playerManager.isInAir)
                {
                    if (inAirTimer > landMinAirTime)
                        animatorHandler.PlayTargetAnimation("Land", true);
                    else
                        animatorHandler.PlayTargetAnimation("Locomotion", false);

                    inAirTimer = 0f;
                    playerManager.isInAir = false;
                }
            }
            else
            {
                if (playerManager.isGrounded) playerManager.isGrounded = false;

                // 土狼时间后才进 Falling
                ungroundedTimer += delta;
                if (!playerManager.isInAir && ungroundedTimer >= fallCoyoteTime)
                {
                    if (!animatorHandler.anim.GetBool("isInteracting"))
                        animatorHandler.PlayTargetAnimation("Falling", true);

                    playerManager.isInAir = true;
                    inAirTimer = 0f;
                }
            }
        }

        void HandleRotation(float delta)
        {
            if (!cameraObject) return;

            Vector3 targetDir = cameraObject.forward * inputHandler.vertical +
                                cameraObject.right   * inputHandler.horizontal;
            targetDir.y = 0f;

            if (targetDir.sqrMagnitude < 1e-6f)
                targetDir = myTransform.forward;

            Quaternion tr = Quaternion.LookRotation(targetDir.normalized);
            myTransform.rotation = Quaternion.Slerp(myTransform.rotation, tr, rotationSpeed * delta);
        }

        bool CanRoll() => rollCooldownTimer <= 0f && !animatorHandler.anim.GetBool("isInteracting");

        void StartRoll(Vector3 dir, string clip)
        {
            dir.y = 0f;
            rollDir   = dir.sqrMagnitude > 1e-4f ? dir.normalized : myTransform.forward;
            rollTimer = 0f;
            isRolling = true;

            rigidbody.velocity = new Vector3(0f, rigidbody.velocity.y, 0f);
            animatorHandler.PlayTargetAnimation(clip, true);
            myTransform.rotation = Quaternion.LookRotation(rollDir);
        }

        void ApplyRollMovement(float delta)
        {
            rollTimer += delta;
            float t = Mathf.Clamp01(rollTimer / Mathf.Max(rollDuration, 1e-4f));

            float baseSpeed = rollDistance / Mathf.Max(rollDuration, 1e-4f);
            float mul = (rollSpeedCurve != null) ? rollSpeedCurve.Evaluate(t) : 1f;

            Vector3 v = rollDir * baseSpeed * mul;
            rigidbody.velocity = new Vector3(v.x, rigidbody.velocity.y, v.z);

            if (t >= 1f)
            {
                isRolling = false;
                animatorHandler.EndInteraction();
                rollCooldownTimer = Mathf.Max(0f, rollCooldown);
            }
        }

        public void SetRollCooldown(float s) => rollCooldown = Mathf.Max(0f, s);
        public void SetRollParams(float dist, float dur)
        {
            rollDistance = dist;
            rollDuration = Mathf.Max(0.01f, dur);
        }
    }
}
