using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHP;

    public float moveSpeed = 5f;      // 前進・後退の速度
    public float rotationSpeed = 120f; // 回転速度（度/秒）

    private Rigidbody rb;
    private float moveInput;
    private float turnInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        currentHP = maxHp;
    }

    void Update()
    {
        // 入力取得
        moveInput = Input.GetAxis("Vertical");   // W/S  → +1 / -1
        turnInput = Input.GetAxis("Horizontal"); // A/D  → -1 / +1
    }

    void FixedUpdate()
    {
        // 前進・後退（物理で移動）
        Vector3 moveDirection = transform.forward * moveInput * moveSpeed;
        rb.MovePosition(rb.position + moveDirection * Time.fixedDeltaTime);

        // 回転（物理で旋回）
        float turn = turnInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log("残りのHPは " + currentHP);

        if (currentHP < 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("死んだ～");
    }

    public int GetMaxHp()
    {
        return maxHp;
    }
}