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

public enum FlowFieldPreset
{
    none = 0,
    circle = 1,
    wave = 2
};

public class FlowFieldManager : MonoBehaviour
{
    [SerializeField]
    private FlowFieldsParameters m_FlowFieldsParameters = new FlowFieldsParameters();
    [SerializeField]
    private FlowFieldsTarget m_FlowFieldsTarget = new FlowFieldsTarget();
    [SerializeField]
    private FlowFieldPreset m_CurrentPreset;

    private Vector3[,] test;

    protected void Awake()
    {
        IsTargetActivated();
        CheckPreset();
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
            m_CurrentPreset = FlowFieldPreset.none;
        }
    }

    private void CheckPreset()
    {
        switch(m_CurrentPreset)
        {
            case FlowFieldPreset.circle:

                break;

            case FlowFieldPreset.wave:

                test = new Vector3[10, 10];

                    
                for (int k = 0; k < m_FlowFieldsParameters.flowFields.Length; k++)
                {
                    float xOffset = 0f;
                    for (int i = 0; i < 10; i++)
                    {
                        float yOffset = 0f;
                        for (int j = 0; j < 10; j++)
                        {
                            float theta = ExtensionMethods.Remap(Mathf.PerlinNoise(xOffset, yOffset), 0f, 1f, 0f, Mathf.PI * 2f);
                            test[i, j] = new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta));
                            m_FlowFieldsParameters.flowFields[k].Direction = test[i, j].normalized;
                            yOffset += 1f;
                        }
                        xOffset += 1f;
                    }
                }

                break;

            case FlowFieldPreset.none:
            default:
                break;

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