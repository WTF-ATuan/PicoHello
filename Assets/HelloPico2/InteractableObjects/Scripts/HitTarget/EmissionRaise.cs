using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class EmissionRaise : MonoBehaviour
{
    public enum EmissionType {
        color, value
    }
    [System.Serializable]
    public struct Group {
        public int Index;
        public Renderer[] TargetRenderer;
    }

    public EmissionType m_CurrentType = EmissionType.color;
    public string m_ControlColorName = "_EmissionColor";
    public string m_ControlValueName = "_RimStrength";
    [ColorUsage(true, true)]
    [FoldoutGroup("EmissionRaise Target Setting"), Indent, PropertyOrder(9)]
    public Color m_ColTarget;
    [FoldoutGroup("EmissionRaise Target Setting"), Indent, PropertyOrder(9)]
    public float m_ValueTarget;
    [FoldoutGroup("Group Renderer Setting"), Indent, PropertyOrder(9)]
    [SerializeField] protected bool m_UsingGroupRenderer = false;
    [FoldoutGroup("Group Renderer Setting"), ShowIf("m_UsingGroupRenderer"), Indent, PropertyOrder(9)]
    [SerializeField] protected int m_GroupIndex;
    [FoldoutGroup("Group Renderer Setting"), ShowIf("m_UsingGroupRenderer"), Indent, PropertyOrder(9)]
    [SerializeField] protected List<Group> m_GroupRenderer;
    [SerializeField] protected Renderer[] m_TargetRenderer;
    [SerializeField] protected float m_Duration;
    [SerializeField] protected AnimationCurve m_Movement;

    public bool m_CanInterupt = false;
    [FoldoutGroup("loop control"), Indent, PropertyOrder(9)]
    public bool m_UseLoopControl = false;
    [FoldoutGroup("loop control"), ShowIf("m_UseLoopControl"), Indent, PropertyOrder(9)]
    public bool m_Loop = true;
    [FoldoutGroup("loop control"), ShowIf("m_UseLoopControl"), Indent, PropertyOrder(9)]
    public bool m_TriggerOnce = false;
    [FoldoutGroup("loop control"), ShowIf("m_UseLoopControl"), Indent, PropertyOrder(9)]
    public bool m_FlipFlop = true;
    [FoldoutGroup("loop control"), ShowIf("m_UseLoopControl"), Indent, PropertyOrder(9)]
    public UnityEngine.Events.UnityEvent WhenTriggerOnce;
    [FoldoutGroup("Events")]
    public UnityEngine.Events.UnityEvent WhenFinishedTransition;

    public Color ColOrigin { get; set; }
    public float ValueOrigin { get; set; }
    public Coroutine Process { get; set; }
    public bool Flipflop { get; set; }
    public bool TriggerOnce { get; set; }
    public Renderer[] TargetRenderer { get { return m_TargetRenderer; } set { m_TargetRenderer = value; } }
    
    public void OnEnable()
    {
        if (m_TargetRenderer == null) return;
        if(m_Loop)
            Raise(m_TargetRenderer);
    }
    public virtual void Update()
    {
        if (TriggerOnce != m_TriggerOnce)
        {
            TriggerOnce = m_TriggerOnce;

            if (TriggerOnce)
            {
                if(!m_UsingGroupRenderer)
                    Raise(m_TargetRenderer);
                else
                    Raise(m_GroupRenderer[m_GroupIndex].TargetRenderer);

                WhenTriggerOnce.Invoke();
            }
        }
    }

    public void Raise(Renderer[] targets)
    {
        if (gameObject.activeSelf)
        {
            switch (m_CurrentType)
            {
                case EmissionType.color:
                    Process = StartCoroutine(RaiseColor(targets));
                    break;
                case EmissionType.value:
                    Process = StartCoroutine(RaiseValue(targets));
                    break;
                default:
                    break;
            }
        }
        
    }
    protected void SetValue(Renderer[] targets, float value) {
        foreach (var obj in targets)
        {
            foreach (var mat in obj.materials)
            {
                mat.SetFloat(m_ControlValueName, value);
            }
        }
    }
    protected void SetColor(Renderer[] targets, Color value)
    {
        foreach (var obj in targets)
        {
            foreach (var mat in obj.materials)
            {
                mat.SetColor(m_ControlColorName, value);
            }
        }
    }
    public void Raise(Renderer[] targets, Color col, int matID = -1)
    {
        m_ColTarget = col;

        if (!m_CanInterupt && Process != null) return;
        if (m_CanInterupt && Process != null)
        {
            // TODO : Interupt not working
            StopCoroutine(Process);
            SetColor(targets, ColOrigin);
        }

        if (m_FlipFlop)
        {
            if(gameObject.activeSelf)
                Process = StartCoroutine(RaiseColor(targets, matID));
        }
        else
        {
            if(gameObject.activeSelf)
                Process = StartCoroutine(RaiseColorWithoutFlipflop(targets, matID));
        }
    }
    public void Raise(Renderer[] targets, float value, int matID = -1)
    {
        m_ValueTarget = value;

        if (Process != null) return;

        if (m_FlipFlop)
        {
            if(gameObject.activeSelf)
                Process = StartCoroutine(RaiseValue(targets, matID));
        }
        else
        {
            if(gameObject.activeSelf)
                Process = StartCoroutine(RaiseValueWithoutFlipflop(targets, matID));
        }
    }
    private IEnumerator RaiseColor(Renderer[] targets, int matID = -1) {        
        int counter = 2;
        Color Origin = (targets[0].material.HasProperty(m_ControlColorName)) ? targets[0].material.GetColor(m_ControlColorName): Color.white;
        var Target = m_ColTarget;

        while (counter > 0) { 

            var StartTime = Time.time;
            var endTime = m_Duration;

            while (Time.time - StartTime < endTime)
            {
                Color col = ColOrigin;

                if (!Flipflop)
                    col = Color.Lerp(Origin, Target, m_Movement.Evaluate( (Time.time - StartTime) / endTime) );
                if (Flipflop)
                    col = Color.Lerp(Target, Origin, m_Movement.Evaluate((Time.time - StartTime) / endTime));

                SetMatColor(targets, matID, col);              

                yield return null;
            }

            if (!Flipflop)
            {
                SetMatColor(targets, matID, Target);
            }
            else
            {
                SetMatColor(targets, matID, Origin);
            }

            Flipflop = !Flipflop;

            --counter;

            yield return null;
        }
        Process = null;

        if (m_Loop)
            StartCoroutine(RaiseColor(targets));
        else
            m_TriggerOnce = false;
    }
    private IEnumerator RaiseColorWithoutFlipflop(Renderer[] targets, int matID = -1)
    {
        Color Origin = (targets[0].material.HasProperty(m_ControlColorName)) ? targets[0].material.GetColor(m_ControlColorName): Color.white;
        var Target = m_ColTarget;

        var StartTime = Time.time;
        var endTime = m_Duration;

        while (Time.time - StartTime < endTime)
        {
            Color col = ColOrigin;

            col = Color.Lerp(Origin, Target, m_Movement.Evaluate((Time.time - StartTime) / endTime));
            
            SetMatColor(targets, matID, col);

            yield return null;
        }

        SetMatColor(targets, matID, Target);

        Process = null;

        if (m_Loop)
            StartCoroutine(RaiseColor(targets));
        else
            m_TriggerOnce = false;
    }
    private void SetMatColor(Renderer[] targets, int matID, Color col)
    {
        foreach (var obj in targets)
        {
            if (matID == -1)
            {
                foreach (var mat in obj.materials)
                {
                    mat.SetColor(m_ControlColorName, col);
                }
            }
            else
            {
                obj.materials[matID].SetColor(m_ControlColorName, col);
            }
        }
    }
    private IEnumerator RaiseValue(Renderer[] targets, int matID = -1)
    {
        int counter = 2;
        float Origin = (targets[0].material.HasProperty(m_ControlValueName)) ? targets[0].material.GetFloat(m_ControlValueName): 0;

        var Target = m_ValueTarget;

        while (counter > 0)
        {

            var StartTime = Time.time;
            var endTime = m_Duration;

            while (Time.time - StartTime < endTime)
            {
                float value = ValueOrigin;

                if (!Flipflop)
                    value = Mathf.Lerp(Origin, Target, m_Movement.Evaluate((Time.time - StartTime) / endTime));
                if (Flipflop)
                    value = Mathf.Lerp(Target, Origin, m_Movement.Evaluate((Time.time - StartTime) / endTime));

                SetMatValue(targets, matID, value);
                
                yield return null;
            }

            if (Flipflop)
            {
                SetMatValue(targets, matID, Origin);
            }

            Flipflop = !Flipflop;

            --counter;

            yield return null;
        }
        Process = null;

        WhenFinishedTransition?.Invoke();

        if (m_Loop)
            StartCoroutine(RaiseValue(targets));
        else
            m_TriggerOnce = false;
    }
    private IEnumerator RaiseValueWithoutFlipflop(Renderer[] targets, int matID = -1)
    {
        float Origin = (targets[0].material.HasProperty(m_ControlValueName))? targets[0].material.GetFloat(m_ControlValueName): 0;       
        var Target = m_ValueTarget;

        var StartTime = Time.time;
        var endTime = m_Duration;

        while (Time.time - StartTime < endTime)
        {
            float value = ValueOrigin;

            value = Mathf.Lerp(Origin, Target, m_Movement.Evaluate((Time.time - StartTime) / endTime));
            
            SetMatValue(targets, matID, value);

            yield return null;
        }

        SetMatValue(targets, matID, Target);

        Process = null;

        WhenFinishedTransition?.Invoke();

        if (m_Loop)
            StartCoroutine(RaiseValue(targets));
        else
            m_TriggerOnce = false;
    }
    private void SetMatValue(Renderer[] targets, int matID, float value) {
        foreach (var obj in targets)
        {
            if (matID == -1)
            {
                foreach (var mat in obj.materials)
                {
                    mat.SetFloat(m_ControlValueName, value);
                }
            }
            else
            {
                obj.materials[matID].SetFloat(m_ControlValueName, value);
            }
        }
    }
}
