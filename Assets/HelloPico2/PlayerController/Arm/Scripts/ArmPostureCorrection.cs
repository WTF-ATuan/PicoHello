using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Sirenix.OdinInspector;

public class ArmPostureCorrection : MonoBehaviour
{
    public enum HintBendDirection { Right,Left}
    public Transform m_Player;
    [FoldoutGroup("Joint Settings")] public Transform m_ArmJoint;
    [FoldoutGroup("Joint Settings")] public Transform m_ShoulderJoint;
    [FoldoutGroup("Joint Settings")] public Transform m_WristJoint;
    [FoldoutGroup("Joint Settings")] public Transform m_WristRoationChecker;
    [FoldoutGroup("IK Settings")] public Transform m_HintIK;
    [FoldoutGroup("IK Settings")] public TwoBoneIKConstraint m_ElbowIKSettings;
    public HintBendDirection m_HintBendDirection = HintBendDirection.Right;
    public Vector2 m_ElbowRotateLimit;
    public float m_LerpSpeed;
    public float m_HintOffset;
    [ReadOnly][SerializeField] private float angle;
    [FoldoutGroup("Gizmos")] public float m_RayLength;

    private Vector3 hintPos;
    private float shoulderWristDist;
    private Vector3 armDir;
    private void Update()
    {
        m_WristRoationChecker.position = m_WristJoint.position;
        m_WristRoationChecker.localEulerAngles = new Vector3(0,0, m_WristJoint.localEulerAngles.z);

        shoulderWristDist = Vector3.Distance(m_ShoulderJoint.position, m_WristJoint.position);
        armDir = (m_WristJoint.position - m_ShoulderJoint.position).normalized;

        var Pivotpos = m_ShoulderJoint.position + armDir * shoulderWristDist / 2;
        int dir = 1;

        if (m_HintBendDirection == HintBendDirection.Right)
            dir = 1;
        else if (m_HintBendDirection == HintBendDirection.Left)
            dir = -1;

        var pivotToPlayerDir = (new Vector3(m_Player.position.x, Pivotpos.y, m_Player.position.z) - Pivotpos).normalized;
        angle = Vector3.Angle(pivotToPlayerDir, m_WristRoationChecker.right * dir);

        if (angle >= m_ElbowRotateLimit.x && angle <= m_ElbowRotateLimit.y)
        {
            hintPos = Pivotpos + m_WristJoint.right * dir * m_HintOffset;
        m_HintIK.position = Vector3.Lerp(m_HintIK.position, hintPos, Time.deltaTime * m_LerpSpeed);        
        }

    }   
    private void OnDrawGizmos()
    {        
        if (angle >= m_ElbowRotateLimit.x && angle < m_ElbowRotateLimit.y)        
            Gizmos.color = Color.blue;
        else
            Gizmos.color = Color.red;
        if (m_HintBendDirection == HintBendDirection.Right)
        {
            Gizmos.DrawRay(m_ArmJoint.position, -m_Player.right * m_RayLength);
            Gizmos.DrawRay(m_ArmJoint.position, m_WristRoationChecker.right * m_RayLength);
        }
        else if (m_HintBendDirection == HintBendDirection.Left)
        {
            Gizmos.DrawRay(m_ArmJoint.position, m_Player.right * m_RayLength);
            Gizmos.DrawRay(m_ArmJoint.position, -m_WristRoationChecker.right * m_RayLength);
        }
    }
}
