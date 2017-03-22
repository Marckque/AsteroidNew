using UnityEngine;

public class InstructionsManager : MonoBehaviour
{
    [SerializeField]
    private bool m_DeactivateInstructions;
    [SerializeField]
    private GameObject[] m_GameObjectsToActivate;

    private void Start()
    {
        if (m_DeactivateInstructions)
        {
            ActivateGameObjects();
            DeactivateSelf();
        }
    }

    protected void Update()
    {
        if (Input.GetButtonDown("Shoot") || Input.GetButtonDown("MoveForward"))
        {
            ActivateGameObjects();
            DeactivateSelf();
        }
    }

    private void ActivateGameObjects()
    {
        foreach (GameObject go in m_GameObjectsToActivate)
        {
            go.SetActive(true);
        }
    }

    private void DeactivateSelf()
    {
        gameObject.SetActive(false);
    }
}