using UnityEngine;

public class Rope : MonoBehaviour
{
    public Transform startObject; // 繩子的起點物體
    public Transform endObject; // 繩子的終點物體
    public int segmentCount = 10; // 繩子的節點數量
    public float segmentLength = 0.2f; // 繩子的節點之間的距離
    public float stiffness = 0.2f; // 繩子的彈性係數
    public float damping = 0.1f; // 繩子的阻尼係數

    private Vector3[] segmentPositions; // 繩子的節點位置數組
    private Quaternion[] segmentRotations; // 繩子的節點旋轉數組
    private float[] segmentVelocities; // 繩子的節點速度數組
    private float[] segmentAccelerations; // 繩子的節點加速度數組
    private Transform[] segmentTransforms; // 繩子的節點物體數組

    private void Start()
    {
        // 初始化繩子的節點位置數組、旋轉數組、速度數組、加速度數組和物體數組
        segmentPositions = new Vector3[segmentCount];
        segmentRotations = new Quaternion[segmentCount];
        segmentVelocities = new float[segmentCount];
        segmentAccelerations = new float[segmentCount];
        segmentTransforms = new Transform[segmentCount];

        // 計算繩子的節點位置和旋轉
        for (int i = 0; i < segmentCount; i++)
        {
            segmentPositions[i] = Vector3.Lerp(startObject.position, endObject.position, (float)i / (float)(segmentCount - 1));
            segmentRotations[i] = Quaternion.identity;
        }

        // 創建繩子的節點物體，並將其設置為繩子的子物體
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segmentObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            segmentObject.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            segmentObject.transform.SetParent(transform);
            segmentTransforms[i] = segmentObject.transform;
        }
    }

    private void LateUpdate()
    {
        // 計算每個節點的加速度
        for (int i = 0; i < segmentCount; i++)
        {
            if (i == 0)
            {
                // 起點
                segmentAccelerations[i] = 0f;
            }
            else if (i == segmentCount - 1)
            {
                // 終點
                segmentAccelerations[i] = 0f;
            }
            else
            {
                // 其他節點，計算彈性和阻尼力
                Vector3 deltaPosition = segmentPositions[i + 1] - segmentPositions[i];
                float deltaLength = deltaPosition.magnitude;
                float stretchLength = deltaLength - segmentLength;
                float stretchForce = stiffness * stretchLength;
                float dampingForce = damping * segmentVelocities[i];
                float totalForce = stretchForce - dampingForce;
                segmentAccelerations[i] = totalForce;
            }
        }

        // 更新每個節點的速度和位置
        for (int i = 0; i < segmentCount; i++)
        {
            if (i == 0)
            {
                // 起點
                segmentVelocities[i] = 0f;
                segmentPositions[i] = startObject.position;
                segmentRotations[i] = startObject.rotation;
            }
            else if (i == segmentCount - 1)
            {
                // 終點
                segmentVelocities[i] = 0f;
                segmentPositions[i] = endObject.position;
                segmentRotations[i] = endObject.rotation;
            }
            else
            {
                // 其他節點，使用歐拉積分法計算速度和位置
                segmentVelocities[i] += segmentAccelerations[i] * Time.deltaTime;
                Vector3 direction = segmentPositions[i + 1] - segmentPositions[i];
                segmentPositions[i] += segmentVelocities[i] * direction.normalized * Time.deltaTime;
                segmentRotations[i] = Quaternion.LookRotation(direction);
            }
        }

        // 更新繩子的節點物體的位置和旋轉
        for (int i = 0; i < segmentCount; i++)
        {
            segmentTransforms[i].position = segmentPositions[i];
            segmentTransforms[i].rotation = segmentRotations[i];
        }
    }
}