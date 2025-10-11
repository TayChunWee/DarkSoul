using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatEditor
{
    public class RootMotionReceiver : MonoBehaviour
    {
        Animator _animator;
        public Vector3 CurrentRootMotion;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }
        private void OnAnimatorMove()
        {
            CurrentRootMotion = _animator.deltaPosition;
        }

    }
}
