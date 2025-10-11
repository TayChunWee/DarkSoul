using UnityEngine;

namespace DS
{
    /// <summary>
    /// 远端玩家的插值器：吃服务器快照 -> 平滑到目标
    /// 不做决策，不读输入；只负责表现层（Transform + 简单动画参数）
    /// </summary>
    [DisallowMultipleComponent]
    public class RemoteGhost : MonoBehaviour
    {
        [Header("Interpolation")]
        [SerializeField] float posLerpSpeed = 12f;    // 位置插值速度
        [SerializeField] float rotLerpSpeed = 18f;    // 旋转插值速度（度/秒）
        [SerializeField] float snapDistance = 3.0f;   // 超过直接贴近
        [SerializeField] float velocitySmoothing = 10f;

        private Vector3 targetPos;
        private Quaternion targetRot;
        private Vector3 targetVel;
        private bool hasSnapshot = false;

        private AnimatorHandler animatorHandler;
        private Rigidbody rb;

        private void Awake()
        {
            animatorHandler = GetComponentInChildren<AnimatorHandler>();
            rb = GetComponent<Rigidbody>();
        }

        public void PushSnapshot(Vector3 pos, Quaternion rot, Vector3 velocity, bool isSprinting)
        {
            targetPos = pos;
            targetRot = rot;
            targetVel = velocity;
            hasSnapshot = true;

            // 更新动画（远端用 moveAmount 估算；你也可以把服务端发来的精确参数塞这里）
            float moveAmount = Mathf.Clamp01(new Vector2(velocity.x, velocity.z).magnitude / 7f); // 粗略估计
            animatorHandler?.UpdateAnimatorValues(moveAmount, 0f, isSprinting);
        }

        public void FixedTick(float fixedDelta)
        {
            if (!hasSnapshot) return;

            Vector3 curPos = transform.position;
            float dist = Vector3.Distance(curPos, targetPos);

            if (dist > snapDistance)
            {
                transform.position = targetPos;
            }
            else
            {
                transform.position = Vector3.MoveTowards(curPos, targetPos, posLerpSpeed * fixedDelta);
            }

            // 旋转插值（角速度）
            if (rotLerpSpeed > 0f)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotLerpSpeed * fixedDelta * 60f);
            }
            else
            {
                transform.rotation = targetRot;
            }

            // 给刚体一个平滑的“表现速度”（可选）
            if (rb)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, targetVel, 1f - Mathf.Exp(-velocitySmoothing * fixedDelta));
            }
        }
    }
}
