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

public enum FlowFieldPreset
{
    none = 0,
    random = 1,
    gravity = 2,
    asteroid = 3,
    alternative = 4,
    alternativeAsteroid = 5,
    clockwise = 6,
    counterClockwise = 7,
    towardsCenter = 8,
    awayFromCenter = 9,
    targetTarget = 10,
    awayFromTarget = 11,
    wave = 12,
};

public class FlowFieldManager : MonoBehaviour
{
    private const int NUMBER_OF_PRESETS = 12;

    #region Variables
    [SerializeField]
    private FlowFieldsParameters m_FlowFieldsParameters = new FlowFieldsParameters();
    [SerializeField]
    private FlowFieldPreset m_CurrentPreset;
    [SerializeField]
    private Transform m_Target;
    private bool m_UpdateDirectionInRealTime;
    #endregion Variables

    #region Initialisers
    protected void Awake()
    {
        InitialiseFlowFields();
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

    // Functions to set values (force, direction, etc...) on flow fields
    private void InitialiseFlowFieldsForceMultiplier()
    {
        foreach (FlowField flowField in m_FlowFieldsParameters.flowFields)
        {
            flowField.SetForceMultiplier(m_FlowFieldsParameters.spaceshipMultiplier, m_FlowFieldsParameters.bigAsteroidMultiplier,
                                        m_FlowFieldsParameters.mediumAsteroidMultiplier, m_FlowFieldsParameters.smallAsteroidMultiplier);
        }
    }
    #endregion Initialisers

    protected void Update()
    {
        if (m_UpdateDirectionInRealTime)
        {
            CheckPreset();
        }

        if (Input.GetButtonDown("PreviousFlowField"))
        {
            if (m_CurrentPreset > 0)
            {
                m_CurrentPreset--;
                CheckPreset();
            }
        }
        else if (Input.GetButtonDown("NextFlowField"))
        {
            if ((int)m_CurrentPreset < NUMBER_OF_PRESETS)
            {
                m_CurrentPreset++;
                CheckPreset();
            }
        }
    }

    protected void OnValidate()
    {
        CheckPreset();
    }

    private void CheckPreset()
    {
        switch (m_CurrentPreset)
        {
            // Flow fields have a direction of zero
            case FlowFieldPreset.none:
                m_UpdateDirectionInRealTime = false;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = Vector3.zero;
                }
                break;

            // It makes the flow fields' direction random
            case FlowFieldPreset.random:
                m_UpdateDirectionInRealTime = false;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = ExtensionMethods.RandomVector3();
                }
                break;

            // Makes the flow fields go down
            case FlowFieldPreset.gravity:
                m_UpdateDirectionInRealTime = false;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = -Vector3.forward;
                }
                break;


            // Targets the first asteroid of the GameManagement "CurrentAstereoid" list
            case FlowFieldPreset.asteroid:
                m_UpdateDirectionInRealTime = true;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = (m_Target.transform.position - GameManagement.Instance.CurrentAsteroids[0].transform.position).normalized;
                }
                break;

            // Targets an asteroid. One out of two flow fields will point towards it while the other will point away from it.
            case FlowFieldPreset.alternativeAsteroid:
                m_UpdateDirectionInRealTime = true;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);

                    if (i % 2 == 0)
                    {
                        m_FlowFieldsParameters.flowFields[i].Direction = (m_Target.transform.position - GameManagement.Instance.CurrentAsteroids[0].transform.position).normalized;
                    }
                    else
                    {
                        m_FlowFieldsParameters.flowFields[i].Direction = (GameManagement.Instance.CurrentAsteroids[0].transform.position - m_Target.transform.position).normalized;
                    }
                }
                break;

            // Defines a random direction and alternates between flow fields with this direction and its opposite
            case FlowFieldPreset.alternative:
                m_UpdateDirectionInRealTime = false;
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
                m_UpdateDirectionInRealTime = false;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    randomDirection = Vector3.forward;
                    m_FlowFieldsParameters.flowFields[i].SetRotation(true, true);
                }
                break;

            // Adds counter clockwise rotation to all direction of flow fields (all direction are pointing in the same direction)
            case FlowFieldPreset.counterClockwise:
                m_UpdateDirectionInRealTime = false;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    randomDirection = Vector3.forward;
                    m_FlowFieldsParameters.flowFields[i].SetRotation(true, false);
                }
                break;
            
            // Goes towards the center of the screen
            case FlowFieldPreset.towardsCenter:
                m_UpdateDirectionInRealTime = false;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = (Vector3.zero - m_FlowFieldsParameters.flowFields[i].transform.position).normalized;
                }
                break;

            // Goes away from the center of the screen
            case FlowFieldPreset.awayFromCenter:
                m_UpdateDirectionInRealTime = false;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = (m_FlowFieldsParameters.flowFields[i].transform.position - Vector3.zero).normalized;
                }
                break;

            // Targets a transform and follows it
            case FlowFieldPreset.targetTarget:
                m_UpdateDirectionInRealTime = true;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = (m_Target.transform.position - m_FlowFieldsParameters.flowFields[i].transform.position).normalized;
                }
                break;

            // Selects a transform and goes away from it
            case FlowFieldPreset.awayFromTarget:
                m_UpdateDirectionInRealTime = true;
                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);
                    m_FlowFieldsParameters.flowFields[i].Direction = (m_FlowFieldsParameters.flowFields[i].transform.position - m_Target.transform.position).normalized;
                }
                break;

            // Everytime a new wave. Would work better with a larger scale. It works, but it is hard to see the wave form yet... !
            case FlowFieldPreset.wave:
                m_UpdateDirectionInRealTime = false;

                float noise = 0;
                Vector2 newDirection = Vector2.zero;

                float xOffset = 0f;
                float yOffset = 0f;

                int counter = 0;
                int maxCount = Random.Range(1, 11);
                bool alterate = true;

                float increment = Random.Range(0.01f, 0.09f);
                float bonus = Random.Range(1f, 10f);
                float angle = Random.Range(1f, 10f);                

                for (int i = 0; i < m_FlowFieldsParameters.flowFields.Length; i++)
                {
                    m_FlowFieldsParameters.flowFields[i].SetRotation(false, false);

                    if (counter > maxCount)
                    {
                        if (alterate)
                        {
                            alterate = false;
                            xOffset += increment * bonus;
                        }
                        else
                        {
                            alterate = true;
                        }

                        counter = 0;
                        xOffset += increment;
                    }

                    counter++;
                    noise = ExtensionMethods.Remap(Mathf.PerlinNoise(xOffset, yOffset), 0f, 1f, 0f, Mathf.PI * angle);
                    newDirection = new Vector2(Mathf.Sin(noise), Mathf.Cos(noise));
                    m_FlowFieldsParameters.flowFields[i].Direction = new Vector3(newDirection.x, 0f, newDirection.y);

                    yOffset += increment;
                }
                break;      

            // It makes flow fields' direction to be Vector.zero;
            default:
                break;
        }
    }
}