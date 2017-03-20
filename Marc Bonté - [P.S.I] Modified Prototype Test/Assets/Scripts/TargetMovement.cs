using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [Header("Circle path")]
    public bool m_Circles;
    public Rotator m_ParentRotator;
    public float m_CircleDiameter;

    [ExecuteInEditMode]
    protected void Start()
    {
        if (m_Circles)
        {   
            m_ParentRotator.SetIsRotating(true);
        }
    }

    protected void OnValidate()
    {
        UpdateTargetDiameter();
    }

    private void UpdateTargetDiameter()
    {
        transform.position = Vector3.forward * m_CircleDiameter;
    }

    protected void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}