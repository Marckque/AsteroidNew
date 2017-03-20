using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    public float m_MinimumRotationSpeed = 0.25f;
    public float m_MaximumRotationSpeed = 0.75f;
    private float m_Speed;
    private Vector3 m_RotationDirection;

	protected void Start()
    {
        m_RotationDirection = ExtensionMethods.RandomVector3();
        m_Speed = Random.Range(m_MinimumRotationSpeed, m_MaximumRotationSpeed);
    }

	protected void Update()
    {
        //transform.Rotate(m_RotationDirection * m_Speed);
	}
}