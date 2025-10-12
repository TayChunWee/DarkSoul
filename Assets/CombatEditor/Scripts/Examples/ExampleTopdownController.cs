using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





namespace CombatEditor
{
    public partial class ExampleTopdownController : MonoBehaviour
    {
        // Every State must correspond to an AnimationClip.
        public enum CharacterState { Idle, Run, Attack1, Attack2, Attack3,OnHit };
        public CharacterState CurrentState;
        public Animator TargetAnimator;
        //Input buffer. If you press attack, in 0.1f the character will try to attack until attack successfully triggered.
        //If we dont use attackbuffer in a action game, the connection between combo may lose because player press attack too early.
        static KeyCode AttackKey = KeyCode.J;
        static float AttackBufferTime = 0.1f;
        float LastPressAttackTime = -Mathf.Infinity;
        /// <summary>
        /// Player will receive Input, Enemy will not.
        /// </summary>
        public enum Team { Player, Enemy };
        public Team team;

        Rigidbody rg;
        CombatController _combatController;

        /// <summary>
        /// The Configurable Parameters.
        /// </summary>
        public float RunSpeed;

        private void Start()
        {
            _combatController = GetComponent<CombatController>();
            rg = GetComponent<Rigidbody>();
            EnterIdle();
        }
        private void Update()
        {
            SaveAttackBufferOnInput();
            UpdateAllStates();
        }
        void UpdateAllStates()
        {
            if (team == Team.Player)
            {
                UpdateStateWithInput(CharacterState.Idle, UpdateIdleWithInput);
                UpdateStateWithInput(CharacterState.Run, UpdateRunWithInput);
                UpdateStateWithInput(CharacterState.Attack1, UpdateAttack1WithInput);
                UpdateStateWithInput(CharacterState.Attack2, UpdateAttack2WithInput);
                UpdateStateWithInput(CharacterState.Attack3, UpdateAttack3WithInput);
                UpdateStateWithInput(CharacterState.OnHit, UpdateOnHitWithInput);
            }
            UpdateState(CharacterState.Idle, UpdateIdle);
            UpdateState(CharacterState.Run, UpdateRun);
            UpdateState(CharacterState.Attack1, UpdateAttack1);
            UpdateState(CharacterState.Attack2, UpdateAttack2);
            UpdateState(CharacterState.Attack3, UpdateAttack3);
            UpdateState(CharacterState.OnHit, UpdateOnHit);
        }
        private void FixedUpdate()
        {
            FixedUpdateAllStates();
        }
        void FixedUpdateAllStates()
        {
            if (team == Team.Player)
            {
                FixedUpdateStateWithInput(CharacterState.Idle, FixedUpdateIdleWithInput);
                FixedUpdateStateWithInput(CharacterState.Run, FixedUpdateRunWithInput);
                FixedUpdateStateWithInput(CharacterState.Attack1, FixedUpdateAttack1WithInput);
                FixedUpdateStateWithInput(CharacterState.Attack2, FixedUpdateAttack2WithInput);
                FixedUpdateStateWithInput(CharacterState.Attack3, FixedUpdateAttack3WithInput);
                FixedUpdateStateWithInput(CharacterState.OnHit, FixedUpdateOnHitWithInput);
            }
            FixedUpdateState(CharacterState.Idle, FixedUpdateIdle);
            FixedUpdateState(CharacterState.Run, FixedUpdateRun);
            FixedUpdateState(CharacterState.Attack1, FixedUpdateAttack1);
            FixedUpdateState(CharacterState.Attack2, FixedUpdateAttack2);
            FixedUpdateState(CharacterState.Attack3, FixedUpdateAttack3);
            FixedUpdateState(CharacterState.OnHit, FixedUpdateOnHit);
        }

        public void SwitchEnterStateAction(CharacterState state)
        {
            if (state == CharacterState.Idle) EnterIdle();
            if (state == CharacterState.Run) EnterRun();
            if (state == CharacterState.Attack1) EnterAttack1();
            if (state == CharacterState.Attack2) EnterAttack2();
            if (state == CharacterState.Attack3) EnterAttack3();
            if (state == CharacterState.OnHit) EnterOnHit();
        }

        public void ChangeState(CharacterState state)
        {
            CurrentState = state;
            SwitchEnterStateAction(state);
        }

        public void UpdateState(CharacterState state, Action stateUpdateAction)
        {
            if (CurrentState == state) stateUpdateAction.Invoke();
        }
        public void UpdateStateWithInput(CharacterState state, Action stateTranslationAction)
        {
            if (CurrentState == state) stateTranslationAction.Invoke();
        }
        public void FixedUpdateState(CharacterState state, Action stateFixedUpdateAction)
        {
            if (CurrentState == state) stateFixedUpdateAction.Invoke();
        }

        public void FixedUpdateStateWithInput(CharacterState state, Action stateFixedUpdateAction)
        {
            if (CurrentState == state) stateFixedUpdateAction.Invoke();
        }
        void SaveAttackBufferOnInput()
        {
            if (Input.GetKey(AttackKey))
            {
                LastPressAttackTime = Time.time;
            }
        }
        bool AttackBufferIsActive()
        {
            if (Time.time - LastPressAttackTime < AttackBufferTime) return true;
            return false;
        }
        void ReleaseAttackBuffer()
        {
            LastPressAttackTime = -Mathf.Infinity;
        }

        public void SimpleMove(Vector3 Pos)
        {
            //transform.Translate(Pos,Space.World);
            rg.MovePosition(transform.position + Pos);
        }

    }
    /// <summary>
    /// StateBehaviors;
    /// </summary>
    public partial class ExampleTopdownController : MonoBehaviour
    {
        public void UpdateIdle() { TargetAnimator.Play("Idle"); }
        public void UpdateRun() { TargetAnimator.Play("Run"); }
        public void UpdateAttack1()
        {
            if (TargetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                ChangeState(CharacterState.Idle);
            }
        }
        public void UpdateAttack2()
        {
            if (TargetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                ChangeState(CharacterState.Idle);
            }
        }
        public void UpdateAttack3()
        {
            if (TargetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") && TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                ChangeState(CharacterState.Idle);
            }
        }
        public void UpdateOnHit()
        {
            if (TargetAnimator.GetCurrentAnimatorStateInfo(0).IsName("OnHit") && TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                ChangeState(CharacterState.Idle);
            }
        }
        public void UpdateIdleWithInput()
        {
            if (Input.GetKeyDown(AttackKey))
            {
                ChangeState(CharacterState.Attack1);
            }
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                ChangeState(CharacterState.Run);
            }
        }
        public void UpdateRunWithInput()
        {
            if (AttackBufferIsActive())
            {
                ReleaseAttackBuffer();
                ChangeState(CharacterState.Attack1);
            }
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                ChangeState(CharacterState.Idle);
            }
        }
        public void UpdateAttack1WithInput()
        {
            if (AttackBufferIsActive() && _combatController.IsInState("Attack1Recover"))
            {
                ReleaseAttackBuffer();
                ChangeState(CharacterState.Attack2);
            }
        }
        public void UpdateAttack2WithInput()
        {
            if (team == Team.Enemy) return;
            if (AttackBufferIsActive() && _combatController.IsInState("Attack2Recover"))
            {
                ReleaseAttackBuffer();
                ChangeState(CharacterState.Attack3);
            }
        }
        public void UpdateAttack3WithInput() { }
        public void UpdateOnHitWithInput() { }
        public void FixedUpdateIdle() { }
        public void FixedUpdateOnHit()
        {
            SimpleMove( _combatController.GetCurrentRootMotion());
        }
        public void FixedUpdateRun() { }
        public void FixedUpdateAttack1()
        {
            SimpleMove(_combatController.GetCurrentRootMotion());
        }
        public void FixedUpdateAttack2()
        {
            SimpleMove(_combatController.GetCurrentRootMotion());
        }
        public void FixedUpdateAttack3()
        {
            SimpleMove(_combatController.GetCurrentRootMotion());
        }
        public void FixedUpdateIdleWithInput() { }
        public void FixedUpdateOnHitWithInput()
        {
        }
        public void FixedUpdateRunWithInput()
        {
            //HandleRotation;
            var Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            if (Direction != Vector3.zero)
            {
                TargetAnimator.transform.forward = Direction;
            }
            //HandleTranslation;
            transform.Translate(TargetAnimator.transform.forward * RunSpeed * Time.deltaTime, Space.World);
        }
        public void FixedUpdateAttack1WithInput() { }
        public void FixedUpdateAttack2WithInput() { }
        public void FixedUpdateAttack3WithInput() { }
        public void EnterIdle()
        {
        }
        public void EnterRun()
        {
        }
        public void EnterAttack1()
        {
            TargetAnimator.Play("Attack1",0,0f);
        }
        public void EnterAttack2()
        {
            TargetAnimator.CrossFade("Attack2",0.2f);
        }
        public void EnterAttack3()
        {
            TargetAnimator.CrossFade("Attack3",0.2f,0,0.2f);
        }
        public void EnterOnHit()
        {
            TargetAnimator.Play("OnHit",0,0.2f);
        }
    }
}

