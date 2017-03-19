using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour
{
    public Vector3 Direction { get; set; }
    private float m_MultiplierSpaceship;
    private float m_MultiplierBigAsteroid;
    private float m_MultiplierMediumAsteroid;
    private float m_MultiplierSmallAsteroid;

    /*
    private void Start()
    {
        int lol = Random.Range(0, 4);

        if (lol == 0)
        {
            Direction = Vector3.right;
        }
        else if (lol == 1)
        {
            Direction = -Vector3.right;
        }
        else if (lol == 2)
        {
            Direction = Vector3.forward;
        }
        else
        {
            Direction = -Vector3.forward;
        }
    }
    */

    public void SetForceMultiplier(float spaceship, float bigAsteroid, float mediumAsteroid, float smallAsteroid)
    {
        m_MultiplierSpaceship = spaceship;
        m_MultiplierBigAsteroid = bigAsteroid;
        m_MultiplierMediumAsteroid = mediumAsteroid;
        m_MultiplierSmallAsteroid = smallAsteroid;
    }

    protected void OnTriggerStay(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        float directionMultiplier = 0f;

        if (entity)
        {
            if (entity is Spaceship)
            {
                directionMultiplier = m_MultiplierSpaceship;
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
        Gizmos.DrawSphere(transform.position + Vector3.up + Direction, 0.1f);
    }
}