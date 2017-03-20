using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField]
    private bool m_Clockwise;
    [SerializeField, Range(0f, 5f)]
    private float m_RotationSpeed;
    private bool m_IsRotating;
	
    public void SetIsRotating(bool value)
    {
        m_IsRotating = value;
    }

	protected void Update()
    {
		if (m_IsRotating)
        {
            float direction = m_Clockwise == true ? 1f : -1f;
            transform.Rotate(direction * Vector3.up * m_RotationSpeed);
        }
	}
}
