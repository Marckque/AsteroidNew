using UnityEngine;

public class DeactivateOnPlay : MonoBehaviour
{
    protected void Awake()
    {
        if (Application.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}