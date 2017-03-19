using UnityEngine;

[System.Serializable]
public class FlowFieldsParameters
{
    [Header("Flow fields"), HideInInspector]
    public FlowField[] flowFields;

    [Range(0f, 3f)]
    public float spaceshipMultiplier = 1f;
    [Range(0f, 3f)]
    public float bigAsteroidMultiplier = 1f;
    [Range(0f, 3f)]
    public float mediumAsteroidMultiplier = 1f;
    [Range(0f, 3f)]
    public float smallAsteroidMultiplier = 1f;
}

[System.Serializable]
public class FlowFieldsTarget
{
    [Header("Target parameters")]
    public Transform flowFieldsTarget;
    [Tooltip("If this variable is false, all booleans below will be false")]
    public bool useTarget;
    public bool updateDirectionInRealTime;
}

[System.Serializable]
public class FlowFieldsCalculation
{
    [Header("Flow calculation")]
    public bool setFlowAccordingToMethodX;
    public bool setFlowAccordingToMethodY;
}

public class FlowFieldManager : MonoBehaviour
{
    [SerializeField]
    private FlowFieldsParameters m_FlowFieldsParameters = new FlowFieldsParameters();
    [SerializeField]
    private FlowFieldsTarget m_FlowFieldsTarget = new FlowFieldsTarget();

    protected void Awake()
    {
        IsTargetActivated();
        InitialiseFlowFields();
    }

    private void IsTargetActivated()
    {
        // Deactivate all other variables if "m_UseTarget" is false
        if (!m_FlowFieldsTarget.useTarget)
        {
            m_FlowFieldsTarget.updateDirectionInRealTime = false;
        }
        else
        {
        }
    }

    // Add all functions that will initialise the parameters that can manually be set on flow fields
    private void InitialiseFlowFields()
    {
        m_FlowFieldsParameters.flowFields = new FlowField[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            m_FlowFieldsParameters.flowFields[i] = transform.GetChild(i).GetComponent<FlowField>();
        }

        InitialiseFlowFieldsForceMultiplier();
    }

    // Functions to set values (force, direction, etc...) on flow fields
    private void InitialiseFlowFieldsForceMultiplier()
    {
        foreach (FlowField flowField in m_FlowFieldsParameters.flowFields)
        {
            flowField.SetForceMultiplier(m_FlowFieldsParameters.spaceshipMultiplier, m_FlowFieldsParameters.bigAsteroidMultiplier,
                                        m_FlowFieldsParameters.mediumAsteroidMultiplier, m_FlowFieldsParameters.smallAsteroidMultiplier);
        }
    }

    private void SetTarget()
    {
        foreach (FlowField flowField in m_FlowFieldsParameters.flowFields)
        {
            flowField.Direction = (m_FlowFieldsTarget.flowFieldsTarget.position - flowField.transform.position).normalized;
        }
    }

    protected void Update()
    {
        if (m_FlowFieldsTarget.updateDirectionInRealTime)
        {
            SetTarget();
        }
    }
}