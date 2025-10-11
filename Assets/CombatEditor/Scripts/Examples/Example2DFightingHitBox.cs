using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatEditor
{
    public class Example2DFightingHitBox : HitBox
    {
        /// <summary>
        /// The Effects on Hit.
        /// </summary>
        public Example2DFightingController.Team Team;
        public CharacterNode.NodeType OnHitParticlesNode;
        public ParticleSystem OnHitParticles;
        public AudioClip OnHitSFX;
        public float FreezeFrameTime;
        public Example2DFightingController.CharacterState TargetState;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            var controller = collision.GetComponent<Example2DFightingController>();
            if (controller == null) return;
            if (controller.team != Team)
            {
                return;
            }
            controller.ChangeState(TargetState);
            //StartEffectGroup.
          
            if (OnHitSFX != null)
            {
                OnHitSFX.PlayClip(1);
            }
            TryHitController(controller.GetComponent<CombatController>());
        }
     

        public void TryHitController(CombatController _controller)
        {

            var _combatController = _controller;
            if (_combatController != null)
            {
                // ChangeDirection
                var AttackDir = (Owner.transform.position - _combatController.transform.position).normalized;
                _combatController._animator.transform.forward = new Vector3(AttackDir.x, 0, AttackDir.z);

                //Freeze Frame.
                _combatController._animSpeedExecutor.AddSpeedModifiers(0, FreezeFrameTime);
                Owner._animSpeedExecutor.AddSpeedModifiers(0, FreezeFrameTime);

                //CreateParticles
                if (OnHitParticles != null)
                {
                    var NodeTransform = _combatController.GetNodeTranform(OnHitParticlesNode);
                    if ( NodeTransform != null)
                    {
                        var Particles = Instantiate(OnHitParticles);
                        Particles.transform.position = NodeTransform.transform.position;
                    }
                }
            }

        }
    }
}
