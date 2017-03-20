using UnityEngine;
using System.Collections.Generic;

public enum AsteroidType
{
    small = 0,
    medium = 1,
    big = 2
};

public class Asteroid : Entity
{
    [SerializeField, Range(0f, 1f)]
    protected float m_AccelerationMultiplier = 1f;
    [SerializeField]
    private int m_Points = 100;

    private List<Asteroid> m_OtherAsteroidsInRange = new List<Asteroid>();

    public AsteroidType AsteroidType { get; set; }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected void OnTriggerEnter(Collider other)
    {
        Bullet bullet = other.GetComponent<Bullet>();
        if (bullet)
        {
            OnCollisionWithBullet(bullet.EntityRigidbody.velocity.normalized);
            Destroy(bullet.gameObject);

            GameManagement.Instance.RemoveAsteroidFromList(this);

            return;
        }

        Spaceship spaceShip = other.GetComponent<Spaceship>();
        if (spaceShip)
        {
            spaceShip.ResetSpaceship();

            return;
        }

        Asteroid asteroid = other.GetComponent<Asteroid>();
        if (asteroid)
        {
            m_OtherAsteroidsInRange.Add(asteroid);
            return;
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        Asteroid asteroid = other.GetComponent<Asteroid>();

        if (asteroid)
        {
            if (m_OtherAsteroidsInRange.Contains(asteroid))
            {
                m_OtherAsteroidsInRange.Remove(asteroid);
            }
        }
    }

    protected virtual void InitialiseVelocity()
    {
        // Use in child
    }

    protected virtual void OnCollisionWithBullet(Vector3 direction)
    {
        GameManagement.Instance.ScoreManager.AddScore(m_Points);
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        base.OnDrawGizmos();
    }
}