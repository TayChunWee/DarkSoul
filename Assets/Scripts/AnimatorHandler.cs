using UnityEngine;

namespace DS
{
    public class AnimatorHandler : MonoBehaviour
    {
        public Animator anim;
        public InputHandler inputHandler;
        public PlayerLocomotion playerLocomotion;
        int vertical;
        int horizontal;
        public bool canRotate;

        public void Initialize()
        {
            anim = GetComponentInChildren<Animator>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();
            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            const float Damp = 0.05f; // 降低阻尼减轻拖尾
            float v = 0f, h = 0f;

            if (verticalMovement > 0 && verticalMovement < 0.55f) v = 0.5f;
            else if (verticalMovement >= 0.55f) v = 1f;
            else if (verticalMovement < 0 && verticalMovement > -0.55f) v = -0.5f;
            else if (verticalMovement <= -0.55f) v = -1f;

            if (horizontalMovement > 0 && horizontalMovement < 0.55f) h = 0.5f;
            else if (horizontalMovement >= 0.55f) h = 1f;
            else if (horizontalMovement < 0 && horizontalMovement > -0.55f) h = -0.5f;
            else if (horizontalMovement <= -0.55f) h = -1f;

            if (isSprinting) { v = 2f; h = horizontalMovement; }

            anim.SetFloat(vertical, v, Damp, Time.deltaTime);
            anim.SetFloat(horizontal, h, Damp, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting, int layer = 0)
        {
            if (!anim) return;

            anim.applyRootMotion = false;
            anim.SetBool("isInteracting", isInteracting);

            int hash = Animator.StringToHash(targetAnim);

            if (layer < 0 || layer >= anim.layerCount || !anim.HasState(layer, hash))
            {
                Debug.LogError($"Animator state '{targetAnim}' not found on layer {layer}. " +
                               $"Check the STATE NAME (not the clip name).");
                return;
            }

            // 显式指定 0 层，避免 -1
            anim.CrossFade(hash, 0.20f, layer, 0f, 0f);
        }


        public void EndInteraction()
        {
            anim.applyRootMotion = false;
            anim.SetBool("isInteracting", false);
        }

        private void LateUpdate()
        {
            if (!anim) return;
            if (!anim.GetBool("isInteracting")) return;
            if (anim.IsInTransition(0)) return;

            var s0 = anim.GetCurrentAnimatorStateInfo(0);
            // Land 0.6 以后即可解锁，其它状态仍 0.98
            if (s0.IsName("Land"))
            {
                if (s0.normalizedTime >= 0.75f) EndInteraction();
            }
            else
            {
                if (s0.normalizedTime >= 0.98f) EndInteraction();
            }
        }


        private void OnAnimatorMove() { /* 非root：不改刚体 */ }
    }
}
