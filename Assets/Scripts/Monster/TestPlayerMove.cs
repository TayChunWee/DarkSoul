using UnityEngine;

public class TestPlayerMove : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;     // 前後移動の速度
    [SerializeField] private float rotationSpeed = 100f; // 回転速度（度/秒）

    void Update()
    {
        // 入力取得
        float moveInput = Input.GetAxis("Vertical");   // W: +1, S: -1
        float rotateInput = Input.GetAxis("Horizontal"); // A: -1, D: +1

        // 前後移動
        transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);

        // 回転
        transform.Rotate(Vector3.up * rotateInput * rotationSpeed * Time.deltaTime);
    }
}