using UnityEngine;

public class Rope : MonoBehaviour
{
    public Transform startObject; // ÷�l���_�I����
    public Transform endObject; // ÷�l�����I����
    public int segmentCount = 10; // ÷�l���`�I�ƶq
    public float segmentLength = 0.2f; // ÷�l���`�I�������Z��
    public float stiffness = 0.2f; // ÷�l���u�ʫY��
    public float damping = 0.1f; // ÷�l�������Y��

    private Vector3[] segmentPositions; // ÷�l���`�I��m�Ʋ�
    private Quaternion[] segmentRotations; // ÷�l���`�I����Ʋ�
    private float[] segmentVelocities; // ÷�l���`�I�t�׼Ʋ�
    private float[] segmentAccelerations; // ÷�l���`�I�[�t�׼Ʋ�
    private Transform[] segmentTransforms; // ÷�l���`�I����Ʋ�

    private void Start()
    {
        // ��l��÷�l���`�I��m�ƲաB����ƲաB�t�׼ƲաB�[�t�׼ƲթM����Ʋ�
        segmentPositions = new Vector3[segmentCount];
        segmentRotations = new Quaternion[segmentCount];
        segmentVelocities = new float[segmentCount];
        segmentAccelerations = new float[segmentCount];
        segmentTransforms = new Transform[segmentCount];

        // �p��÷�l���`�I��m�M����
        for (int i = 0; i < segmentCount; i++)
        {
            segmentPositions[i] = Vector3.Lerp(startObject.position, endObject.position, (float)i / (float)(segmentCount - 1));
            segmentRotations[i] = Quaternion.identity;
        }

        // �Ы�÷�l���`�I����A�ñN��]�m��÷�l���l����
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
        // �p��C�Ӹ`�I���[�t��
        for (int i = 0; i < segmentCount; i++)
        {
            if (i == 0)
            {
                // �_�I
                segmentAccelerations[i] = 0f;
            }
            else if (i == segmentCount - 1)
            {
                // ���I
                segmentAccelerations[i] = 0f;
            }
            else
            {
                // ��L�`�I�A�p��u�ʩM�����O
                Vector3 deltaPosition = segmentPositions[i + 1] - segmentPositions[i];
                float deltaLength = deltaPosition.magnitude;
                float stretchLength = deltaLength - segmentLength;
                float stretchForce = stiffness * stretchLength;
                float dampingForce = damping * segmentVelocities[i];
                float totalForce = stretchForce - dampingForce;
                segmentAccelerations[i] = totalForce;
            }
        }

        // ��s�C�Ӹ`�I���t�שM��m
        for (int i = 0; i < segmentCount; i++)
        {
            if (i == 0)
            {
                // �_�I
                segmentVelocities[i] = 0f;
                segmentPositions[i] = startObject.position;
                segmentRotations[i] = startObject.rotation;
            }
            else if (i == segmentCount - 1)
            {
                // ���I
                segmentVelocities[i] = 0f;
                segmentPositions[i] = endObject.position;
                segmentRotations[i] = endObject.rotation;
            }
            else
            {
                // ��L�`�I�A�ϥμکԿn���k�p��t�שM��m
                segmentVelocities[i] += segmentAccelerations[i] * Time.deltaTime;
                Vector3 direction = segmentPositions[i + 1] - segmentPositions[i];
                segmentPositions[i] += segmentVelocities[i] * direction.normalized * Time.deltaTime;
                segmentRotations[i] = Quaternion.LookRotation(direction);
            }
        }

        // ��s÷�l���`�I���骺��m�M����
        for (int i = 0; i < segmentCount; i++)
        {
            segmentTransforms[i].position = segmentPositions[i];
            segmentTransforms[i].rotation = segmentRotations[i];
        }
    }
}