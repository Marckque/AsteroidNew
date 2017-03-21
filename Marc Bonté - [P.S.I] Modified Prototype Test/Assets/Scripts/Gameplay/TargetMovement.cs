using UnityEngine;

public class TargetMovement : MonoBehaviour
{
    [Header("Circle path"), SerializeField]
    private bool m_Circles;
    [SerializeField]
    private TargetRotator m_ParentRotator;
    [SerializeField]
    private float m_CircleDiameter;

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