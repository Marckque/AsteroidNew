using UnityEngine;

[System.Serializable]
public class EntityParameters
{
    public float accelerationScalar = 1f;
    public float maxVelocityMagnitude = 1f;
}

[System.Serializable]
public class EntityEffects
{
    public TrailRenderer trail;
}

[RequireComponent(typeof(Rigidbody))]
public class Entity : MonoBehaviour
{
    [SerializeField]
    protected EntityParameters m_EntityParameters = new EntityParameters();
    public EntityParameters EntityParameters { get { return m_EntityParameters; } }

    [SerializeField]
    protected EntityEffects m_EntityEffects = new EntityEffects();

    private const float BORDER_MARGIN = 0.5f;

    protected Vector3 m_Acceleration;
    protected Rigidbody m_EntityRigidbody;
    public Rigidbody EntityRigidbody { get { return m_EntityRigidbody; } }

    protected virtual void Awake()
    {
        InitialiseRigidbody();
    }

    private void InitialiseRigidbody()
    {
        m_EntityRigidbody = GetComponent<Rigidbody>();
        m_EntityRigidbody.useGravity = false;
    }

    protected virtual void Update()
    {
        ConstrainPositionToCamera();
    }

    protected virtual void FixedUpdate()
    {
        ApplyForces(false);
    }

    private void ConstrainPositionToCamera()
    {
        // Horizontal constrain
        if (transform.position.x < -Camera.main.orthographicSize)
        {
            if (m_EntityEffects.trail) m_EntityEffects.trail.Clear();
            transform.position = new Vector3(Camera.main.orthographicSize - BORDER_MARGIN, 0f, transform.position.z);
        }
        else if (transform.position.x > Camera.main.orthographicSize)
        {
            if (m_EntityEffects.trail) m_EntityEffects.trail.Clear();
            transform.position = new Vector3(-Camera.main.orthographicSize + BORDER_MARGIN, 0f, transform.position.z);
        }

        // Vertical constrain
        if (transform.position.z < -Camera.main.orthographicSize)
        {
            if (m_EntityEffects.trail) m_EntityEffects.trail.Clear();
            transform.position = new Vector3(transform.position.x, 0f, Camera.main.orthographicSize - BORDER_MARGIN);
        }
        else if (transform.position.z > Camera.main.orthographicSize)
        {
            if (m_EntityEffects.trail) m_EntityEffects.trail.Clear();
            transform.position = new Vector3(transform.position.x, 0f, -Camera.main.orthographicSize + BORDER_MARGIN);
        }
    }

    public void SetAcceleration(Vector3 force)
    {
        m_Acceleration += force * m_EntityParameters.accelerationScalar;
    }

    public void ApplyForces(bool conversion)
    {
        if (conversion)
        {
            m_EntityRigidbody.AddForce(transform.TransformDirection(m_Acceleration));
        }
        else
        {
            m_EntityRigidbody.AddForce(m_Acceleration);
        }
        m_EntityRigidbody.velocity = Vector3.ClampMagnitude(m_EntityRigidbody.velocity, m_EntityParameters.maxVelocityMagnitude);

        m_Acceleration = Vector3.zero;
    }

    protected virtual void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.DrawRay(transform.position, m_EntityRigidbody.velocity);
            Gizmos.DrawSphere(transform.position + m_EntityRigidbody.velocity, 0.2f);
        }
    }
}