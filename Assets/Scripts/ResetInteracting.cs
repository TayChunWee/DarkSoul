// ResetInteracting.cs  —— 挂到需要自动退出交互的动画状态（翻滚/攻击等）
using UnityEngine;

public class ResetInteracting : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("isInteracting", false);
    }
}
