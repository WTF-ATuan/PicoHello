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

    public Vector2 m_HintBlendingRange;
    public HintBendDirection m_HintBendDirection = HintBendDirection.Right;
    public Vector2 m_ElbowRotateLimit;
    public float m_LerpSpeed;
    public float m_HintOffset;
    [ReadOnly][SerializeField] private float angle;
    [FoldoutGroup("Gizmos")] public float m_RayLength;

    private Vector3 hintPos;
    private float shoulderWristDist;
    private Vector3 armDir;

    [ReadOnly]public bool WithinRange;
    [ReadOnly]public float Dist;

    int dir = 1;
    Vector3 currentWristRight;
    private void Awake()
    {
        currentWristRight = m_WristRoationChecker.right;
    }
    public bool _InvertDir = false;
    public float _PlayerYAngle = 0;
    private void Update()
    {
        if (m_HintBendDirection == HintBendDirection.Right)
            dir = 1;
        else if (m_HintBendDirection == HintBendDirection.Left)
            dir = -1;

        _PlayerYAngle = m_Player.localEulerAngles.y;

        //if (m_Player.localEulerAngles.y >= 90 && m_Player.localEulerAngles.y < 270)
        //{ 
        //    dir = -dir;
        //    _InvertDir = true;
        //}
        //else
        //    _InvertDir = false;


        m_WristRoationChecker.position = m_WristJoint.position;
        m_WristRoationChecker.localEulerAngles = new Vector3(0,0, m_WristJoint.eulerAngles.z) * -dir;
        //m_WristRoationChecker.Rotate(Vector3.up, m_Player.localEulerAngles.y);

        shoulderWristDist = Vector3.Distance(m_ShoulderJoint.position, m_WristJoint.position);
        armDir = (m_WristJoint.position - m_ShoulderJoint.position).normalized;

        var Pivotpos = m_ShoulderJoint.position + armDir * shoulderWristDist / 2;        
                
        float WristRoationCheckerAngleZ = m_WristRoationChecker.localEulerAngles.z;

        if (WristRoationCheckerAngleZ > 180) WristRoationCheckerAngleZ -= 360;
        if (WristRoationCheckerAngleZ < -180) WristRoationCheckerAngleZ += 360;

        if (WristRoationCheckerAngleZ > m_ElbowRotateLimit.x && WristRoationCheckerAngleZ < m_ElbowRotateLimit.y)
        {
            WithinRange = true;
            hintPos = Pivotpos + m_WristRoationChecker.right * dir * m_HintOffset;
            currentWristRight = m_WristRoationChecker.right;
            //m_HintIK.position = hintPos;
        }
        else {
            WithinRange = false;
            hintPos = Pivotpos + currentWristRight * dir * m_HintOffset;
        }
        
        m_HintIK.position = Vector3.Lerp(m_HintIK.position, hintPos, Time.deltaTime * m_LerpSpeed);

        Dist = shoulderWristDist;
        m_ElbowIKSettings.data.hintWeight =  1 - Mathf.Clamp(shoulderWristDist / Mathf.Abs(m_HintBlendingRange.x - m_HintBlendingRange.y),0,1);
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
