using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatEditor
{
    public partial class Example2DFightingController : MonoBehaviour
    {
        public enum CharacterState { Idle, Run, Attack1, Attack2, Attack3, OnHit }
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

        Rigidbody2D rg;
        CombatController _combatController;

        /// <summary>
        /// The Configurable Parameters.
        /// </summary>
        public float RunSpeed;

        private void Start()
        {
            _combatController = GetComponent<CombatController>();
            rg = GetComponent<Rigidbody2D>();
            OnEnterState_Idle();
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
                UpdateStateWithInput(CharacterState.Idle, UpdateStateWithInput_Idle);
                UpdateStateWithInput(CharacterState.Run, UpdateStateWithInput_Run);
                UpdateStateWithInput(CharacterState.Attack1, UpdateStateWithInput_Attack1);
                UpdateStateWithInput(CharacterState.Attack2, UpdateStateWithInput_Attack2);
                UpdateStateWithInput(CharacterState.Attack3, UpdateStateWithInput_Attack3);
                UpdateStateWithInput(CharacterState.OnHit, UpdateStateWithInput_OnHit);
            }
            UpdateState(CharacterState.Idle, UpdateState_Idle);
            UpdateState(CharacterState.Run, UpdateState_Run);
            UpdateState(CharacterState.Attack1, UpdateState_Attack1);
            UpdateState(CharacterState.Attack2, UpdateState_Attack2);
            UpdateState(CharacterState.Attack3, UpdateState_Attack3);
            UpdateState(CharacterState.OnHit, UpdateState_OnHit);
        }
        private void FixedUpdate()
        {
            FixedUpdateAllStates();
        }
        void FixedUpdateAllStates()
        {
            if (team == Team.Player)
            {
                FixedUpdateStateWithInput(CharacterState.Idle, FixedUpdateWithInput_Idle);
                FixedUpdateStateWithInput(CharacterState.Run, FixedUpdateWithInput_Run);
                FixedUpdateStateWithInput(CharacterState.Attack1, FixedUpdateWithInput_Attack1);
                FixedUpdateStateWithInput(CharacterState.Attack2, FixedUpdateWithInput_Attack2);
                FixedUpdateStateWithInput(CharacterState.Attack3, FixedUpdateWithInput_Attack3);
                FixedUpdateStateWithInput(CharacterState.OnHit, FixedUpdateWithInput_OnHit);
            }
            FixedUpdateState(CharacterState.Idle, FixedUpdate_Idle);
            FixedUpdateState(CharacterState.Run, FixedUpdate_Run);
            FixedUpdateState(CharacterState.Attack1, FixedUpdate_Attack1);
            FixedUpdateState(CharacterState.Attack2, FixedUpdate_Attack2);
            FixedUpdateState(CharacterState.Attack3, FixedUpdate_Attack3);
            FixedUpdateState(CharacterState.OnHit, FixedUpdate_OnHit);
        }

        public void SwitchEnterStateAction(CharacterState state)
        {
            if (state == CharacterState.Idle) OnEnterState_Idle();
            if (state == CharacterState.Run) OnEnterState_Run();
            if (state == CharacterState.Attack1) OnEnterState_Attack1();
            if (state == CharacterState.Attack2) OnEnterState_Attack2();
            if (state == CharacterState.Attack3) OnEnterState_Attack3();
            if (state == CharacterState.OnHit) OnEnterState_OnHit();
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
            rg.MovePosition(transform.position + Pos);
        }
    }
    public partial class Example2DFightingController
    {
        void OnEnterState_Idle()
        {

        }
        void UpdateStateWithInput_Idle()
        {
            if(Input.GetAxisRaw("Horizontal")!=0)
            {
                ChangeState(CharacterState.Run);
            }
            if (Input.GetKeyDown(AttackKey))
            {
                ChangeState(CharacterState.Attack1);
            }
        }
        void UpdateState_Idle()
        {
            TargetAnimator.Play("Idle");
        }

        void FixedUpdateWithInput_Idle()
        {

        }

        void FixedUpdate_Idle()
        {

        }
    }
    public partial class Example2DFightingController
    {
        void OnEnterState_Run()
        {

        }
        void UpdateStateWithInput_Run()
        {
            if(Input.GetAxisRaw("Horizontal") == 0)
            {
                ChangeState( CharacterState.Idle );
            }
            if (Input.GetKeyDown(AttackKey))
            {
                ChangeState(CharacterState.Attack1);
            }
        }
        void UpdateState_Run()
        {
            TargetAnimator.Play("Run");
        }
        void FixedUpdateWithInput_Run()
        {
            //HandleRotation;
            var Direction = new Vector3(Input.GetAxisRaw("Horizontal"), 0,0);
            if (Direction != Vector3.zero)
            {
                TargetAnimator.transform.forward = Direction;
            }
            //HandleTranslation;
            transform.Translate(TargetAnimator.transform.forward * RunSpeed * Time.deltaTime, Space.World);
        }
        void FixedUpdate_Run()
        {

        }
    }
    public partial class Example2DFightingController
    {
        void OnEnterState_Attack1()
        {
            TargetAnimator.Play("Attack1", 0, 0f);
        }
        void UpdateStateWithInput_Attack1()
        {
            if (TargetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") && TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                ChangeState(CharacterState.Idle);
            }
        }
        void UpdateState_Attack1()
        {
            if (AttackBufferIsActive() && _combatController.IsInState("Attack1Recover"))
            {
                ReleaseAttackBuffer();
                ChangeState(CharacterState.Attack2);
            }

        }
        void FixedUpdateWithInput_Attack1()
        {

        }
        void FixedUpdate_Attack1()
        {
            SimpleMove(_combatController.GetCurrentRootMotion());
        }
    }
    public partial class Example2DFightingController
    {
        void OnEnterState_Attack2()
        {
            TargetAnimator.CrossFade("Attack2", 0.2f);
        }
        void UpdateStateWithInput_Attack2()
        {
            if (TargetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") && TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                ChangeState(CharacterState.Idle);
            }
        }
        void UpdateState_Attack2()
        {
            if (AttackBufferIsActive() && _combatController.IsInState("Attack2Recover"))
            {
                ReleaseAttackBuffer();
                ChangeState(CharacterState.Attack3);
            }
        }
        void FixedUpdateWithInput_Attack2()
        {

        }
        void FixedUpdate_Attack2()
        {
            SimpleMove(_combatController.GetCurrentRootMotion());
        }
    }
    public partial class Example2DFightingController
    {
        void OnEnterState_Attack3()
        {
            TargetAnimator.CrossFade("Attack3", 0.2f);
        }
        void UpdateStateWithInput_Attack3()
        {

        }
        void UpdateState_Attack3()
        {
            if (TargetAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack3") && TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                ChangeState(CharacterState.Idle);
            }
        }
        void FixedUpdateWithInput_Attack3()
        {

        }
        void FixedUpdate_Attack3()
        {
            SimpleMove(_combatController.GetCurrentRootMotion());
        }
    }
    public partial class Example2DFightingController
    {
        void OnEnterState_OnHit()
        {
            TargetAnimator.Play("OnHit", 0, 0.2f);
        }
        void UpdateStateWithInput_OnHit()
        {

        }
        void UpdateState_OnHit()
        {
            if (TargetAnimator.GetCurrentAnimatorStateInfo(0).IsName("OnHit") && TargetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
            {
                ChangeState(CharacterState.Idle);
            }
         
        }
        void FixedUpdateWithInput_OnHit()
        {

        }

        /// <summary>
        /// The rootmotion is too small. I change the motion to the combatcontroller.
        /// </summary>
        void FixedUpdate_OnHit()
        {
            //SimpleMove(_combatController.GetCurrentRootMotion());
        }
    }
}
