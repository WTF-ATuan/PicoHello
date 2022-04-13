using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class BubbleGun : MonoBehaviour
{
    XRGrabInteractable m_InteractableBase;
    
    
    [SerializeField] ParticleSystem m_BubbleParticleSystem = null;
    [SerializeField] Transform shootBall;
    bool isGet = false;
    float m_getTime;
    float m_desTime=5.0f;
    Vector3 pos;

    SphereCollider m_scollider;
    const string k_AnimTriggerDown = "TriggerDown";
    const string k_AnimTriggerUp = "TriggerUp";
    const float k_HeldThreshold = 0.1f;

    float m_TriggerHeldTime;
    bool m_TriggerDown;

    protected void Start()
    {
        m_InteractableBase = GetComponent<XRGrabInteractable>();
        m_InteractableBase.selectExited.AddListener(DroppedGun);
        m_InteractableBase.activated.AddListener(TriggerPulled);
        m_InteractableBase.deactivated.AddListener(TriggerReleased);

        m_scollider = GetComponent<SphereCollider>();
        m_getTime = m_desTime;
    }

    protected void Update()
    {
        if (m_TriggerDown)
        {
            m_TriggerHeldTime += Time.deltaTime;

            if (m_TriggerHeldTime >= k_HeldThreshold)
            {
                if (!m_BubbleParticleSystem.isPlaying)
                {
                    m_BubbleParticleSystem.Play();

                    Vector3 pos = new Vector3(transform.position.x,transform.position.y,transform.position.z);
                    Instantiate(shootBall, pos, Quaternion.identity);
                    transform.localScale = new Vector3(transform.localScale.x * 0.6f, transform.localScale.y * 0.6f, transform.localScale.z * 0.6f);
                    if (transform.localScale.x < 0.3f)
                    {
                        transform.localPosition = Vector3.up * m_TriggerHeldTime;
                        Destroy(gameObject,1f);
                    }

                }
            }
        }
    }
    private void FixedUpdate()
    {

        if (isGet == false)
        {
            m_getTime -= Time.deltaTime;
            if (m_getTime < 0)
            {
                Destroy(gameObject);
            }
        }
    }

    void TriggerReleased(DeactivateEventArgs args)
    {
        m_TriggerDown = false;
        m_TriggerHeldTime = 0f;
        m_BubbleParticleSystem.Stop();
    }

    void TriggerPulled(ActivateEventArgs args)
    {
        m_TriggerDown = true;
        
    }

    void DroppedGun(SelectExitEventArgs args)
    {
        // In case the gun is dropped while in use.
        

        m_TriggerDown = false;
        m_TriggerHeldTime = 0f;
        m_BubbleParticleSystem.Stop();
        
    }

    public void ShootEvent()
    {
        m_BubbleParticleSystem.Emit(1);
    }
    public void getacBall()
    {
        m_scollider.radius = 0.001f;
        isGet = true;
        m_getTime = m_desTime;
    }
    public void disgetacBall()
    {
        m_scollider.radius = 0.1f;
        isGet = false;
    }
}
