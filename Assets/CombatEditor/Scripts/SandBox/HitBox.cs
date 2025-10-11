
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
	
 namespace CombatEditor
{	
	public class HitBox : MonoBehaviour
	{
        public CombatController Owner;
  
        public  void Init(CombatController _controller)
        {
            Owner = _controller;
        }
    }
}
