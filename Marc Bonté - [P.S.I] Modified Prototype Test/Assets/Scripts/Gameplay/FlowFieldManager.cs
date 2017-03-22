using UnityEngine;

[System.Serializable]
public class FlowFieldsParameters
{
    [Header("Flow fields")]
    public Transform flowFieldContainer;

    [HideInInspector]
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
    none,
    random,
    gravity,
    alternative,
    clockwise,
    counterClockwise,
    towardsCenter,
    awayFromCenter,
    wave,
    wave2
};

public class FlowFieldManager : MonoBehaviour
{
    [SerializeField]
    private FlowFieldsParameters m_FlowFieldsParameters = new FlowFieldsParameters();
    [SerializeField]
    private FlowFieldsTarget m_FlowFieldsTarget = new FlowFieldsTarget();
    [SerializeField]
    private FlowFieldPreset m_CurrentPreset;

    protected void Awake()
    {
        InitialiseFlowFields();
        IsTargetActivated();
        CheckPreset();
    }

    // Add all functions that will initialise the parameters that can manually be set on flow fields
    private void InitialiseFlowFields()
    {
        m_FlowFieldsParameters.flowFields = new FlowField[m_FlowFieldsParameters.flowFieldContainer.childCount];

        for (int i = 0; i < m_FlowFieldsParameters.flowFieldContainer.childCount; i++)
        {
            m_FlowFieldsParameters.flowFields[i] = m_FlowFieldsParameters.flowFieldContainer.GetChild(i).GetComponent<FlowField>();
        }

        InitialiseFlowFieldsForceMultiplier();
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
        switch (m_CurrentPreset)
        {
            // Flow fields have a direction of zero
            case FlowFieldPreset.none:
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = Vector3.zero;
                }
                break;

            // It makes the flow fields' direction random
            case FlowFieldPreset.random:
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = ExtensionMethods.RandomVector3();
                }
                break;

            // Makes the flow fields go down
            case FlowFieldPreset.gravity:
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = -Vector3.forward;
                }
                break;

            // Defines a random direction and alternates between flow fields with this direction and its opposite
            case FlowFieldPreset.alternative:
                Vector3 randomDirection = ExtensionMethods.RandomVector3();

                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);

                    if (i % 2 == 0)
                    {
                        int randomInt = Random.Range(0, 2);
                        float direction = randomInt == 0 ? 1 : -1;

                        m_FlowFieldsParameters.flowFields[i].Direction = direction * randomDirection;
                    }
                    else
                    {
                        m_FlowFieldsParameters.flowFields[i].Direction = Vector3.zero;
                    }
                } 
                break;

            // Adds clockwise rotation to all direction of flow fields (all direction are pointing in the same direction)
            case FlowFieldPreset.clockwise:
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    randomDirection = Vector3.forward;
                    m_FlowFieldsParameters.flowFields[i].SetRotation(true, true);
                }
                break;

            // Adds counter clockwise rotation to all direction of flow fields (all direction are pointing in the same direction)
            case FlowFieldPreset.counterClockwise:
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    randomDirection = Vector3.forward;
                    m_FlowFieldsParameters.flowFields[i].SetRotation(true, false);
                }
                break;

            case FlowFieldPreset.towardsCenter:
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = (Vector3.zero - m_FlowFieldsParameters.flowFields[i].transform.position).normalized;
                }
                break;

            case FlowFieldPreset.awayFromCenter:
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = (m_FlowFieldsParameters.flowFields[i].transform.position - Vector3.zero).normalized;
                }
                break;

        case FlowFieldPreset.wave:
            float a = 0;
            Vector2 v = Vector2.zero;

            float xOff = 0f;
            float yOff = 0f;
                int dd = 0;
                float aaa = 0.03f;

            for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
            {
                    if (dd > 10)
                    {
                        dd = 0;
                        xOff += aaa;
                    }

                    dd++;
                    a = ExtensionMethods.Remap(Mathf.PerlinNoise(xOff, yOff), 0f, 1f, 0f, Mathf.PI * 2f);
                    v = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                    m_FlowFieldsParameters.flowFields[i].Direction = new Vector3(v.x, 0f, v.y);

                    yOff += aaa;
                }
            break;

            /*
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
                            m_FlowFieldsParameters.flowFields[k].Direction = test[i, j];
                            yOffset += 1f;
                        }
                        xOffset += 1f;
                    }
                }
                break;

            case FlowFieldPreset.wave2:
                float noise;
                for (int i = 0; i < 99; i++)
                {
                    noise = Mathf.PerlinNoise(Random.Range(-20, 20), Random.Range(-20, 20));
                    m_FlowFieldsParameters.flowFields[i].Direction = new Vector3(Mathf.Sin(noise), 0f, Mathf.Cos(noise)).normalized;
                }

                break;
            */

            // It makes flow fields' direction to be Vector.zero;
            default:
                break;
        }
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

    protected void OnValidate()
    {
        CheckPreset();
    }
}