using UnityEngine;

public class Spring : MonoBehaviour
{
    public Transform target;   // 弹簧的目标位置
    public float stiffness = 0.5f;  // 弹簧的硬度
    public float damping = 0.1f;    // 弹簧的阻尼
    public float mass = 2f;      //質量

    private Vector3 velocity;   // 物体的速度

    private void Update()
    {
        Vector3 offset = target.position - transform.position;  // 计算目标位置和当前位置之间的距离差
        Vector3 force = offset * stiffness;  // 计算弹簧的力量
        velocity += mass * force * Time.deltaTime; // 根据力量计算速度
        velocity *= 1 - damping * Time.deltaTime;  // 计算阻尼
        transform.position += velocity * Time.deltaTime;  // 根据速度移动物体
    }
}