using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    [SerializeField]
    private Transform m_ArrowGraphics;

    private const float SPACESHIP_IS_USING_FORWARD_INPUT_MULTIPLIER = 0.25f;

    private float m_MultiplierSpaceship;
    private float m_MultiplierBigAsteroid;
    private float m_MultiplierMediumAsteroid;
    private float m_MultiplierSmallAsteroid;

    public Vector3 Direction { get; set; }

    // If direction keeps on rotating
    private bool m_IsRotating;
    private bool m_Clockwise;

    protected void Update()
    {
        if (m_IsRotating)
        {
            if (m_Clockwise)
            {
                Direction = new Vector3(Mathf.Cos(Time.time), 0f, -Mathf.Sin(Time.time));
            }
            else
            {
                Direction = new Vector3(Mathf.Cos(Time.time), 0f, Mathf.Sin(Time.time));
            }
        }

        if (Direction != Vector3.zero)
        {
            if (!m_ArrowGraphics.gameObject.activeInHierarchy)
            {
                m_ArrowGraphics.gameObject.SetActive(true);
            }

            m_ArrowGraphics.transform.forward = Direction;
        }
        else
        {
            m_ArrowGraphics.gameObject.SetActive(false);
        }
    }

    public void SetForceMultiplier(float spaceship, float bigAsteroid, float mediumAsteroid, float smallAsteroid)
    {
        m_MultiplierSpaceship = spaceship;
        m_MultiplierBigAsteroid = bigAsteroid;
        m_MultiplierMediumAsteroid = mediumAsteroid;
        m_MultiplierSmallAsteroid = smallAsteroid;
    }

    public void SetRotation(bool value, bool direction)
    {
        m_IsRotating = value;
        m_Clockwise = direction;
    }

    protected void OnTriggerStay(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        float directionMultiplier = 0f;

        if (entity)
        {
            if (entity is Spaceship)
            {
                Spaceship spaceship = entity as Spaceship;

                if (spaceship.IsUsingForwardInput)
                {
                    directionMultiplier = m_MultiplierSpaceship * SPACESHIP_IS_USING_FORWARD_INPUT_MULTIPLIER;
                }
                else
                {
                    directionMultiplier = m_MultiplierSpaceship;
                }
            }
            else if (entity is Asteroid)
            {
                Asteroid asteroid = entity as Asteroid;

                if (asteroid.AsteroidType == AsteroidType.big)
                {
                    directionMultiplier = m_MultiplierBigAsteroid;
                }
                else if (asteroid.AsteroidType == AsteroidType.medium)
                {
                    directionMultiplier = m_MultiplierMediumAsteroid;
                }
                else if (asteroid.AsteroidType == AsteroidType.small)
                {
                    directionMultiplier = m_MultiplierSmallAsteroid;
                }
            }
            else if (entity is Bullet)
            {
                return;
            }

            entity.SetAcceleration(Direction * directionMultiplier);
            entity.ApplyForces(false);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position + Vector3.up, Direction);
        Gizmos.DrawSphere(transform.position + Vector3.up + Direction, 0.2f);
    }
}