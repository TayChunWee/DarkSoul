using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimation : MonoBehaviour
{
    private Animator _animator = null;

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _animator.SetTrigger("TriggerTest");
            Debug.Log("MouseLeft Click");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            _animator.SetBool("TrunLeft", true);
        }
        else if(Input.GetKeyUp(KeyCode.A))
        {
            _animator.SetBool("TrunLeft", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            _animator.SetBool("TrunRight", true);
        }
        else if(Input.GetKeyUp(KeyCode.D))
        {
            _animator.SetBool("TrunRight", false);
        }
    }
}
